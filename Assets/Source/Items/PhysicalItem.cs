using UnityEngine;
using System.Collections;

public class PhysicalItem : MonoBehaviour {

	public Inventory.Slot singleSlot;
    public GameObject model;
    public BoxCollider boxCollider;

    public Light rarityLight;
    public ParticleSystem rarityParticle;

    [Header ("Spawn with")]
    public ItemPrefab prefab;
    public int count;

    void Start () {
        if (prefab) {
            singleSlot = Inventory.Slot.CreateSlot ();
            singleSlot.item = prefab.GetItem ();
            singleSlot.count = count;
            UpdateGraphics ();
        }
    }

	public static PhysicalItem Create (Item item, int count, Vector3 position, Quaternion rotation) {
		GameObject newItem = (GameObject)Instantiate (Game.game.physicalItemPrefab, position, rotation);
        PhysicalItem thisItem = newItem.GetComponent<PhysicalItem> ();

        thisItem.singleSlot = Inventory.Slot.CreateSlot ();
        thisItem.singleSlot.item = item;
        thisItem.singleSlot.count = count;

        thisItem.UpdateGraphics ();
        return newItem.GetComponent<PhysicalItem>();
    }

    public void PlaceInSquadInventory () {
        PlaceInInventory (Squad.activeSquad.sharedInventory);
    }

    public void PlaceInInventory (Inventory inventory) {
        inventory.PlaceItems (singleSlot);
        Destroy (gameObject);
    }

    void LateUpdate () {
        rarityParticle.transform.rotation = Quaternion.Euler (90f, 0f, 0f);
        rarityParticle.transform.position = transform.position + Vector3.up;
    }

    void UpdateGraphics () {
        model = singleSlot.item.GetModel ();
        boxCollider.size = ItemRender.GetObjectBounds (model).size;

        model.transform.parent = transform;
        model.transform.position = transform.position;
        model.transform.rotation = transform.rotation;

        rarityLight.color = Game.RarityColor (singleSlot.item.prefab.rarity);
        rarityParticle.startColor = Game.RarityColor (singleSlot.item.prefab.rarity);
    }
}