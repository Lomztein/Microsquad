using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

    public int size;
	public Slot[] slots;

    public void Awake () {
        slots = new Slot[size];
        for (int i = 0; i < slots.Length; i++) {
            slots[i] = Slot.CreateSlot ();
        }
    }

    public Slot FindItemByType (ItemPrefab.Type type) {
        foreach (Slot slot in slots) {
            if (slot.item && slot.item.prefab.type == type)
                return slot;
        }

        return null;
    }

    public Slot FindItemByPrefab ( ItemPrefab prefab ) {
        foreach (Slot slot in slots)
            if (slot.item && slot.item.prefab == prefab)
                return slot;

        return null;
    }

    /*
    /// <summary>
    /// This function finds multiple slots of items, based on the count value. This should be used if there isn't enough count in a single item, for instance if looking for ammo.
    /// Do note that this function removes the items found from the inventory.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public Slot[] FindItemsByType (ItemPrefab.Type type, int count) {

        List<Item> items = new List<Item> ();
        List<int> amount = new List<int> ();

        List<Slot> foundSlots = new List<Slot> ();

        while (count > 0) {
            Slot slot = FindItemByType (type);
            

        }
    }

    public Slot[] FindItemsByPrefab (ItemPrefab prefab, int count) {

    }
    */

    public class Slot : ScriptableObject {

        public Item item;
        public int count;

        public GameObject inventoryButton;

        public void ForceButtonUpdate () {
            if (inventoryButton)
                inventoryButton.SendMessage ("UpdateButton", SendMessageOptions.DontRequireReceiver);
        }

        public static Slot CreateSlot () {
            return ScriptableObject.CreateInstance<Slot> ();
        }

        public override string ToString () {
            if (item) {
                return item.prefab.name + ", " + count.ToString ();
            }else {
                return "Empty";
            }
        }

        public void ChangeCount (int addition) {
            count += addition;

            if (count < 0)
                Debug.LogWarning ("Tried to remove a larger count that was present.");

            if (count == 0)
                RemoveItem ();

            ForceButtonUpdate ();
        }

        public void RemoveItem () {
            item = null;
            count = 0;
        }

        public void MoveItem (Slot newSlot, int transferCount = -1, bool oppisiteStack = false) {
            // Remember: Clicking an inventory button moves the buttons slot into the hand, ergo it calls this function with the hand slot as newSlot.

            Item otherItem = newSlot.item;
            int otherCount = newSlot.count;

            if (transferCount == -1)
                transferCount = count;

            if (Equals (this, newSlot)) {

                // Both sides have items, are the same item and metadata.
                int total = otherCount + transferCount;
                int max = item.prefab.maxStack;

                if (total <= max) {
                    newSlot.ChangeCount (transferCount);
                    ChangeCount (-transferCount);
                } else {
                    // Other slot does not have enough room for this slots item,
                    // so it adds what it can and subtracts those from this slot.
                    int remaining = max - otherCount;

                    newSlot.ChangeCount (remaining);
                    ChangeCount (-remaining);
                }

                if (oppisiteStack) {
                    newSlot.MoveItem (this);
                }
                // Other slot has enough room for this slots items, so it adds
                // this slots item and removes item from this slot.
            } else {
                // Both sides are not the same item, or any side doesn't have an item. Simply swap spaces.
                // If recieving side is empty, only the transferCount should be moved, if not, and transferCount
                // isn't the entire stack, do nothing.

                if (newSlot.item == null) {
                    newSlot.count = transferCount;
                    newSlot.item = item;

                    ChangeCount (-transferCount);
                }else if (transferCount == count) {
                    newSlot.count = count;
                    newSlot.item = item;

                    item = otherItem;
                    count = otherCount;
                }
            }

            ForceButtonUpdate ();
        }

        public static bool Equals (Slot slotOne, Slot slotTwo) {
            return (slotOne.item && slotTwo.item && slotOne.item.prefab == slotTwo.item.prefab && slotOne.item.metadata == slotTwo.item.metadata);
        }
    }
}
