using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class Serializer {
    private readonly BinaryFormatter binaryFormatter;
    public Serializer(BinaryFormatter binaryFormatter) {
        this.binaryFormatter = binaryFormatter;
    }
    public string Serialize(object data) {
        StringBuilder stringBuilder = new StringBuilder();
        Type type = data.GetType();

        stringBuilder.AppendLine(type.Name);

        List<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());
        foreach (PropertyInfo info in properties) {
            HandlePropertySerialization(new SerializationBlock(info, data, stringBuilder));
        }
        return stringBuilder.ToString();
    }

    private void HandlePropertySerialization(SerializationBlock block) {
        PropertyInfo info = block.info;
        object propValue = info.GetValue(block.propValue, null);
        int identLayer = 0;
        if (HandleCustomPropertySerialization(new SerializationBlock(info, propValue, block.stringBuilder, identLayer))) return;
        if (HandleComponentsSerialization(new SerializationBlock(info, propValue, block.stringBuilder, identLayer))) return;
        HandleValueSerialization(new SerializationBlock(block.info, propValue, block.stringBuilder, identLayer));
    }

    private bool HandleCustomPropertySerialization(SerializationBlock block) {
        if (HandleReferencedSerialization(block)) return true;
        if (HandleListSerialization(block)) return true;
        Type type = block.info.PropertyType;
        if (type.IsAssignableFrom(typeof(Entity)) || type == typeof(Entity)) return true;
        return false;
    }

    private void HandleValueSerialization(SerializationBlock block) {
        var memoryStream = new MemoryStream();
        if (block.propValue == null) throw new Exception(block.info.Name + " is null and cannot be serialized!");
        using (memoryStream) {
            binaryFormatter.Serialize(memoryStream, block.propValue);
        }
        string base64 = Convert.ToBase64String(memoryStream.ToArray());
        BuildProperty(block.info.Name, "val", base64, block.stringBuilder, block.identLayer);
    }

    private bool HandleReferencedSerialization(SerializationBlock block) {
        if (DAssetDatabase.IsAssetReferenceTyped(block.info.PropertyType)) {
            UnityEngine.Object obj = block.propValue as UnityEngine.Object;
            if (obj == null) {
                BuildProperty(block.info.Name, "ref", "null", block.stringBuilder, block.identLayer);
            } else {
                string path = AssetDatabase.GetAssetPath(obj);
                int key = DAssetDatabase.GetKeyByPath(path, block.info.PropertyType);
                BuildProperty(block.info.Name, "ref", key.ToString(), block.stringBuilder, block.identLayer);
            }
            return true;
        }
        return false;
    }

    private bool HandleListSerialization(SerializationBlock block) {
        if (block.info.PropertyType.IsGenericType && block.info.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
            IEnumerable enumerableValue = block.propValue as IEnumerable;
            block.stringBuilder.AppendLine();
            block.stringBuilder.Append(new string('\t', block.identLayer) + "[" + block.info.Name + ": list = {");
            foreach (object item in enumerableValue) {
                //Insert here function to make recursive
                var memoryStream = new MemoryStream();
                using (memoryStream) {
                    binaryFormatter.Serialize(memoryStream, item);
                }
                string data = Convert.ToBase64String(memoryStream.ToArray());
                block.stringBuilder.Append(data);
                block.stringBuilder.Append("|");
            }
            block.stringBuilder.Append("}]");
            return true;
        }
        return false;
    }

    private bool HandleComponentsSerialization(SerializationBlock block) {
        if (block.info.PropertyType.IsGenericType && block.info.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>) && block.info.Name == "components") {
            IEnumerable enumerableValue = block.propValue as IEnumerable;
            block.stringBuilder.AppendLine();
            block.stringBuilder.Append(new string('\t', block.identLayer) + "[" + "components" + " : compDict = {");
            block.identLayer++;
            foreach (object item in enumerableValue) {

                KeyValuePair<Type, Component> kvp = (KeyValuePair<Type, Component>)item;
                Type type = kvp.Key;
                List<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());

                block.stringBuilder.AppendLine();
                block.stringBuilder.Append(new string('\t', block.identLayer) + "[" + type.Name + " : comp = {");
                block.identLayer++;
                foreach (PropertyInfo compInfo in properties) {
                    object componentPropValue = compInfo.GetValue(kvp.Value, null);
                    if (!HandleCustomPropertySerialization(new SerializationBlock(compInfo, componentPropValue, block.stringBuilder, block.identLayer))) {
                        HandleValueSerialization(new SerializationBlock(compInfo, componentPropValue, block.stringBuilder, block.identLayer));
                    }
                }
                block.identLayer--;
                block.stringBuilder.AppendLine();
                block.stringBuilder.Append(new string('\t', block.identLayer) + "}]");
            }
            block.identLayer--;
            block.stringBuilder.AppendLine();
            block.stringBuilder.Append(new string('\t', block.identLayer) + "}]");
            return true;
        }
        return false;
    }

    private void BuildProperty(string propertyName, string propertyType, string value, StringBuilder stringBuilder, int identLayer = 0) {
        stringBuilder.AppendLine();
        stringBuilder.Append(new string('\t', identLayer) + "[" + propertyName + " : " + propertyType + " = { " + value + " }]");
    }

    private struct SerializationBlock {
        public PropertyInfo info;
        public object propValue;
        public StringBuilder stringBuilder;
        public int identLayer;
        public SerializationBlock(PropertyInfo info, object propValue, StringBuilder stringBuilder, int identLayer = 0) {
            this.info = info;
            this.propValue = propValue;
            this.stringBuilder = stringBuilder;
            this.identLayer = identLayer;
        }
    }

}