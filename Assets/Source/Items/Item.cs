using UnityEngine;
using System.Collections;

public class Item : ScriptableObject {

    public ItemPrefab prefab;
    public string metadata;

    // With items, attributes should only be used if metadata is already used, since it's likely faster to use the metadata.
    public ObjectAttribute attributes = new ObjectAttribute ();

    private string metaOnSave;
    private Texture2D cachedIcon = null;

    public virtual GameObject GetModel () {
        if (prefab.model)
            return Instantiate (prefab.model);

        // To get a representative model of the object, the gameObject is spawned, and all except for rendering and transform is removed from it.
        GameObject obj = Instantiate (prefab.gameObject);
        obj.SendMessage ("OnEquip", new CharacterEquipment.Slot.EquipMessage (null, metadata, null), SendMessageOptions.DontRequireReceiver);
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

    public static bool Equals ( Item slotOne, Item slotTwo ) {
        return (slotOne && slotTwo && slotOne.prefab == slotTwo.prefab && slotOne.metadata == slotTwo.metadata);
    }

    public static implicit operator ItemPrefab(Item item) {
        return item.prefab;
    }
}
