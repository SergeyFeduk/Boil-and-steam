using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
class Generator {
    private BinaryFormatter binaryFormatter;
    public Generator(BinaryFormatter binaryFormatter) {
        this.binaryFormatter = binaryFormatter;
    }
    public object GenerateInstance(Parser.Node inputNode, Type type) {
        object typedObject = Activator.CreateInstance(type);
        List<string> identifiers = new List<string>();
        HandleParentNode((Parser.ParentNode)inputNode, type, typedObject, identifiers, true);
        return typedObject;
    }

    private void HandleParentNode(Parser.ParentNode inputNode, Type type, object typedObject, List<string> identifiers, bool isFirst) {
        Parser.ParentNode parentNode = inputNode;
        if (isFirst) {

        } else {
            identifiers.Add(parentNode.identifier);
        }
        
        for (int i = 0; i < parentNode.nodes.Count; i++) {
            Parser.Node node = parentNode.nodes[i];
            if (node == null) continue;
            if (node.GetType() == typeof(Parser.ValueNode)) {
                HandleSerializedNode((Parser.ValueNode)node, type, typedObject, identifiers);
            }
            if (node.GetType() == typeof(Parser.ParentNode)) {
                HandleParentNode((Parser.ParentNode)node, type, typedObject, identifiers, false);
            }
        }
    }

    private void HandleSerializedNode(Parser.ValueNode valueNode, Type type, object typedObject, List<string> identifiers) {
        PropertyInfo propertyInfo = null;
        object nextTypedObject = typedObject;
        Type nextType = type;
        if (identifiers.Count == 0) {
            propertyInfo = type.GetProperty(valueNode.identifier);
        } else {
            for (int i = 0; i < identifiers.Count; i++) {

                if (nextType.IsGenericType && nextType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                    Dictionary<Type, Component> components = (Dictionary<Type, Component>)type.GetProperty("components").GetValue(typedObject);
                    Type componentType = Type.GetType(identifiers[i]);
                    nextTypedObject = components[componentType];
                    propertyInfo = componentType.GetProperty(valueNode.identifier);
                } else {
                    propertyInfo = nextType.GetProperty(identifiers[i]);
                    nextType = propertyInfo.PropertyType;
                    nextTypedObject = propertyInfo.GetValue(nextTypedObject, null);
                }
            }
        }
        
        switch (valueNode.tag) {
            case Lexer.Tag.VAL:
                HandleValueNode(valueNode, nextTypedObject, propertyInfo);
                break;
            case Lexer.Tag.REF:
                HandleReferencedNode(valueNode, nextTypedObject, propertyInfo);
                break;
            case Lexer.Tag.LIST:
                HandleListNode(valueNode, nextTypedObject, propertyInfo);
                break;
        }
    }

    private void HandleValueNode(Parser.ValueNode valueNode, object typedObject, PropertyInfo propertyInfo) {
        byte[] bytes = Convert.FromBase64String(valueNode.value);
        using (MemoryStream stream = new MemoryStream(bytes)) {
            object obj = binaryFormatter.Deserialize(stream);
            propertyInfo.SetValue(typedObject, obj, null);
        }
    }

    private void HandleReferencedNode(Parser.ValueNode valueNode, object typedObject, PropertyInfo propertyInfo) {
        if (valueNode.value == "null") {
            propertyInfo.SetValue(typedObject, null, null);
        } else {
            int key = int.Parse(valueNode.value);
            object[] objects = DAssetDatabase.GetAssetsByKey(key);
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i].GetType() == propertyInfo.PropertyType) {
                    propertyInfo.SetValue(typedObject, objects[i], null);
                }
            }
        }
        
    }

    private void HandleListNode(Parser.ValueNode valueNode, object typedObject, PropertyInfo propertyInfo) {
        Type genericType = propertyInfo.PropertyType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(genericType);
        IList objList = (IList)Activator.CreateInstance(listType);
        string currentObj = "";
        for (int k = 0; k < valueNode.value.Length; k++) {
            char currentChar = valueNode.value[k];
            if (currentChar == '|' || k >= valueNode.value.Length) {
                byte[] bytes = Convert.FromBase64String(currentObj);
                using (MemoryStream stream = new MemoryStream(bytes)) {
                    object obj = binaryFormatter.Deserialize(stream);
                    objList.Add(obj);
                }
                currentObj = "";
                continue;
            }
            currentObj += currentChar;
        }
        propertyInfo.SetValue(typedObject, objList, null);
    }
}
