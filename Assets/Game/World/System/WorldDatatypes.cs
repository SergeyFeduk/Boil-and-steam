using UnityEngine;

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

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }
}