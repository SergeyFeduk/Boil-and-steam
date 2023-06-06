using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

class ValueComparer<T> : IComparer<T>, IEqualityComparer<T> {
    public ValueComparer(params Func<T, object>[] props) {
        Properties = new ReadOnlyCollection<Func<T, object>>(props);
    }

    public ReadOnlyCollection<Func<T, object>> Properties { get; private set; }

    public bool Equals(T x, T y) {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        //Object.Equals handles strings and nulls correctly
        return Properties.All(f => Equals(f(x), f(y)));
    }

    public int GetHashCode(T obj) {
        if (obj == null) return -42;
        unchecked {
            int hash = 17;
            foreach (var prop in Properties) {
                object value = prop(obj);
                if (value == null)
                    hash = hash * 23 - 1;
                else
                    hash = hash * 23 + value.GetHashCode();
            }
            return hash;
        }
    }

    public int Compare(T x, T y) {
        foreach (var prop in Properties) {
            //The properties can be any type including null.
            var comp = Comparer.DefaultInvariant
                .Compare(prop(x), prop(y));
            if (comp != 0)
                return comp;
        }
        return 0;
    }
}