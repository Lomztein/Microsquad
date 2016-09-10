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

    public virtual GameObject GetModel () {
        // To get a representative model of the object, the gameObject is spawned, and all except for rendering and transform is removed from it.
        GameObject obj = Instantiate (prefab.gameObject);
        obj.SendMessage ("OnEquip", new CharacterEquipment.Equipment.EquipMessage (null, metadata, null));
        Transform[] all = obj.GetComponentsInChildren<Transform> ();
        for (int i = 0; i < all.Length; i++) {
            Component[] components = all[i].GetComponents<Component> ();
            for (int j = 0; j < components.Length; j++) {
                Component c = components[j];
                if (c as Transform == null &&
                    c as MeshFilter == null &&
                    c as Renderer == null) {
                    Destroy (c);
                }
            }
        }

        return obj;
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
