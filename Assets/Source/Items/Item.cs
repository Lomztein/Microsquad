using UnityEngine;
using System.Collections;

public class Item : ScriptableObject {

    public ItemPrefab prefab;
    public string metadata;

    public virtual Mesh GetMesh () {
        if (prefab.type == ItemPrefab.Type.Weapon) {
            return WeaponGenerator.GenerateWeaponMesh (metadata);
        }else
            return prefab.model;
    }
}
