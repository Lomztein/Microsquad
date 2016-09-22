using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LootChest : MonoBehaviour {

    public Inventory inventory;
    public Loot[] lootItems;
    public List<Inventory.Slot> tempSlots;
    private GameObject instance;

    public string chestName = "Loot Chest";

    // Use this for initialization
    void Start () {
        SetLoot ();
	}
	
	void CMLootAll () {
        Micromanagement.OrderInteraction (transform, "LootAll");
    }

    void CMOpenChest () {
        Micromanagement.OrderInteraction (transform, "OpenChest");
    }

    void LootAll () {
        inventory.MoveAllItems (Squad.activeSquad.sharedInventory);
    }

    void OpenChest () {
        GameObject ui = Resources.Load<GameObject> ("GUI/InventoryGUI");
        instance = (GameObject)Instantiate (ui, GetScreenPos (transform.position), Quaternion.identity);

        InventoryGUI gui = instance.GetComponent<InventoryGUI> ();
        gui.inventory = inventory;

        gui.GetComponentInChildren<Text> ().text = chestName;
        Image firstImage = gui.GetComponentInChildren<Image> ();
        gui.transform.SetParent (GUIManager.mainCanvas.transform);

        gui.CreateButtons (firstImage.transform);
    }

    void Update () {
        if (instance) {
            instance.transform.position = GetScreenPos (transform.position);
            if (Input.GetMouseButtonDown (1))
                Destroy (instance);
        }
    }

    Vector3 GetScreenPos (Vector3 pos) {
        return Camera.main.WorldToScreenPoint (pos);
    }

    void SetLoot () {
        foreach (Inventory.Slot slot in inventory.slots) {
            foreach (Loot loot in lootItems) {
                if (slot.item)
                    break;
                int chance = Random.Range (0, 100);
                if (chance < loot.spawnChance / inventory.size) {
                    GetItem (loot).MoveItem (slot);
                }
            }
        }
    }

    private Inventory.Slot GetItem ( Loot piece ) {
        Inventory.Slot tempSlot = Inventory.Slot.CreateSlot ();
        tempSlot.item = piece.prefab.GetItem ();
        tempSlot.count = piece.maxCount == -1 ? piece.prefab.maxStack : Random.Range (piece.minCount, piece.maxCount + 1);
        tempSlots.Add (tempSlot);

        if (piece.metadata.Length > 0)
            tempSlot.item.metadata = piece.metadata;
        return tempSlot;
    }

    [System.Serializable]
    public class Loot {

        public ItemPrefab prefab;
        public int minCount;
        public int maxCount;
        public string metadata;

        public int spawnChance;

    }
}
