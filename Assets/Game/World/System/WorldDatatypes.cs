using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using UnityEngine;
/*
public class AddressConverter : TypeConverter {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
        return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
        var name = value as string;
        int index = 0;
        int x = 0;
        int y = 0;
        char currentChar;
        while (index < name.Length) {
            currentChar = name[index];
            if (currentChar == '(') {
                //Parse till ':'
                string xStr = "";
                while (currentChar != ':') {
                    index++;
                    currentChar = name[index];
                    xStr += currentChar;
                }
                x = int.Parse(xStr);
                //Parse till ')'
                string yStr = "";
                while (currentChar != ')') {
                    index++;
                    currentChar = name[index];
                    yStr += currentChar;
                }
                y = int.Parse(yStr);
            }
            index++;
        }
        return new Address(x, y);
    }
}*/

//[ TypeConverter(typeof(AddressConverter))]
[Serializable]
public struct Address {
    public int x;
    public int y;
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
