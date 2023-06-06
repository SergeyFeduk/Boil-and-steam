using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[Serializable]
public class Address {
    public int x;
    public int y;
    public Address() {
        x = 0;
        y = 0;
    }

    public Address(int x, int y) {
        this.x = x;
        this.y = y;
    }
    public Address(Vector2Int position) {
        this.x = position.x;
        this.y = position.y;
    }

    public Vector2Int AsIntVector() {
        return new Vector2Int(x, y);
    }

    public Vector2 AsVector() {
        return new Vector2(x, y);
    }

    public static Address operator *(Address address, int number) => new Address(address.x * number, address.y * number);
    public static Address operator +(Address addressA, Address addressB) => new Address(addressA.x + addressB.x, addressA.y + addressB.y);
    public static Address operator -(Address addressA, Address addressB) => new Address(addressA.x - addressB.x, addressA.y - addressB.y);

    public static bool operator ==(Address addressA, Address addressB) => addressA.x == addressB.x && addressA.y == addressB.y;
    public static bool operator !=(Address addressA, Address addressB) => addressA.x != addressB.x || addressA.y != addressB.y;

    public override string ToString() {
        return "(" + x.ToString() + ":" + y.ToString() + ")";
    }

    public static implicit operator Address(string line) {
        //String looks like: (x:y)
        int index = 0;
        int x = 0;
        int y = 0;
        char currentChar;
        while (index < line.Length) {
            currentChar = line[index];
            if (currentChar == '(') {
                //Parse till ':'
                string xStr = "";
                while (currentChar != ':') {
                    index++;
                    currentChar = line[index];
                    xStr += currentChar;
                }
                x = int.Parse(xStr);
                //Parse till ')'
                string yStr = "";
                while (currentChar != ')') {
                    index++;
                    currentChar = line[index];
                    yStr += currentChar;
                }
                y = int.Parse(yStr);
            }
            index++;
        }
        return new Address(x, y);
    }

    public override bool Equals(object obj) {
        if (obj == null) return false;
        Address address = (Address)obj;
        return x == address.x && y == address.y;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }

}
[CustomPropertyDrawer(typeof(Address))]
public class AddressDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var container = new VisualElement();
        container.style.justifyContent = Justify.SpaceBetween;
        container.style.flexDirection = FlexDirection.Row;
        Label label = new Label(((GenericProperty)property.serializedObject.targetObject).Name);
        PropertyField xField = new PropertyField(property.FindPropertyRelative("x"),"");
        PropertyField yField = new PropertyField(property.FindPropertyRelative("y"),"");
        xField.style.flexGrow = new StyleFloat(800);
        xField.style.width = container.style.width.value.value / 2 - 5;
        yField.style.flexGrow = new StyleFloat(800);
        yField.style.width = container.style.width.value.value / 2 - 5;
        container.Add(label);
        container.Add(xField);
        container.Add(yField);
        return container;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        Rect xRect = new Rect(position.x, position.y, position.width / 2 - 5, position.height);
        Rect yRect = new Rect(position.x + position.width / 2, position.y, position.width / 2 - 5, position.height);
        EditorGUI.PropertyField(xRect, property.FindPropertyRelative("x"), GUIContent.none);
        EditorGUI.PropertyField(yRect, property.FindPropertyRelative("y"), GUIContent.none);
        EditorGUI.EndProperty();
    }
}

//Get this somewhere else
public class Vector2IntSerializationSurrogate : ISerializationSurrogate {
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        Vector2Int v3 = (Vector2Int)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
        Vector2Int v3 = (Vector2Int)obj;
        v3.x = (int)info.GetValue("x", typeof(int));
        v3.y = (int)info.GetValue("y", typeof(int));
        obj = v3;
        return obj;
    }
}
public class Vector2SerializationSurrogate : ISerializationSurrogate {
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        Vector2 v3 = (Vector2)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
        Vector2 v3 = (Vector2)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        obj = v3;
        return obj;
    }
}
// Should be somewhere else
public class GenericProperty : ScriptableObject {
    [SerializeReference] public object property;
    [SerializeReference] public string Name;
}