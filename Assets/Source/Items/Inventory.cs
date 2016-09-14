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

    public Slot FindAvailableSlot (Item movingItem = null) {
        foreach (Slot s in slots) {

            // If the slot already has an item, compare to moving item and return if stackable.
            if (s.item && movingItem && s.count != movingItem.prefab.maxStack) {
                if (Item.Equals (s.item, movingItem)) {
                    return s;
                }
            }else {
                if (!s.item)
                    return s;
            }
        }

        return null;
    }

    // This will either work or go into infinite loopness.
    public void PlaceItems (params Slot[] items) {
        for (int i = 0; i < items.Length; i++) {
            Slot loc = items[i];
            while (loc.count != 0) {
                Slot s = FindAvailableSlot (loc.item);
                if (s) {
                    loc.MoveItem (s);
                    s.ForceButtonUpdate ();
                } else
                    break;
            }
        }
    }

    public void MoveAllItems ( Inventory newInventory ) {
        newInventory.PlaceItems (slots);
    }

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

            if (Item.Equals (item, otherItem)) {

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
    }
}
