using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ObjectAttribute {

    public Dictionary<string, object> data = new Dictionary<string, object>();
    
    public bool AttributeExists (string name) {
        return data.ContainsKey (name);
    }

    public T GetAttribute<T> (string name) {
        if (AttributeExists (name))
            return (T)data[name];

        return default (T);
    }

    public void AddAttribute (string name, object value) {
        data.Add (name, value);
    }

    public void RemoveAttribute (string name) {
        if (AttributeExists (name))
            data.Remove (name);
    }
}
