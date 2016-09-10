using UnityEngine;
using System.Collections;

public class PhysicalItem : MonoBehaviour {

	public Inventory.Slot singleSlot;
    public GameObject model;
    public BoxCollider boxCollider;

    [Header ("Spawn with")]
    public ItemPrefab prefab;
    public int count;

    void Start () {
        if (prefab) {
            singleSlot = Inventory.Slot.CreateSlot ();
            singleSlot.item = (Item)prefab;
            singleSlot.count = count;
            UpdateMesh ();
        }
    }

	public static PhysicalItem Create (Item item, int count, Vector3 position, Quaternion rotation) {
		GameObject newItem = (GameObject)Instantiate (Game.game.physicalItemPrefab, position, rotation);
        newItem.GetComponent<PhysicalItem> ().singleSlot = Inventory.Slot.CreateSlot ();
		newItem.GetComponent<PhysicalItem> ().singleSlot.item = item;
		newItem.GetComponent<PhysicalItem> ().singleSlot.count = count;

        newItem.GetComponent<PhysicalItem>().UpdateMesh ();
        return newItem.GetComponent<PhysicalItem>();
    }

    void UpdateMesh () {
        model = singleSlot.item.GetModel ();

        model.transform.parent = transform;
        model.transform.position = transform.position;
        model.transform.rotation = transform.rotation;

        boxCollider.size = ItemRender.GetObjectBounds (model).size;
    }
}