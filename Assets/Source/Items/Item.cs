using UnityEngine;
using System.Collections;

public class Item : ScriptableObject {

    public ItemPrefab prefab;
    public string metadata;

    private string metaOnSave;
    private Mesh cachedMesh = null;
    private Texture2D cachedIcon = null;

    public virtual Mesh GetMesh () {
        Mesh mesh = null;
        if (prefab.type == ItemPrefab.Type.Weapon) {
            mesh = WeaponGenerator.GenerateWeaponMesh (metadata);
        }else
            mesh = prefab.model;

        // If change is detected or no cached exists, cache new mesh.
        if (HasChanged (cachedMesh)) {
            metaOnSave = metadata;
            cachedMesh = mesh;
        }

        return cachedMesh;
    }

    private bool HasChanged (Object cachedObject) {
        return (metadata != metaOnSave || !cachedObject);
    }

    public virtual Texture2D GetIcon () {
        Texture2D tex = ItemRender.RenderItem (this);

        if (HasChanged (cachedIcon)) {
            cachedIcon = tex;
            metaOnSave = metadata;
        }

        return cachedIcon;
    }
}
