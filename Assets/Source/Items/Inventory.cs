using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {

    public int size;
	public Slot[] slots;

    public void Awake () {
        slots = new Slot[size];
        for (int i = 0; i < slots.Length; i++) {
            slots[i] = Slot.CreateSlot ();
        }
    }

    public Slot FindItemByName (string n) {
        foreach (Slot slot in slots) {
            if (slot.item.name == n)
                return slot;
        }

        return null;
    }

    public Slot FindItemByType (ItemPrefab.Type type) {
        foreach (Slot slot in slots) {
            if (slot.item.prefab.type == type)
                return slot;
        }

        return null;
    }

    public class Slot : ScriptableObject {
        public Item item;
        public int count;

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

        public void MoveItem (Slot newSlot, int transferCount = -1) {
            Item otherItem = newSlot.item;
            int otherCount = newSlot.count;

            if (transferCount == -1)
                transferCount = count;

            if (item && otherItem
                && item.prefab == otherItem.prefab
                && item.metadata == otherItem.metadata) {

                // Both sides have items, are the same item and metadata.
                int remaining = -(otherCount + transferCount - otherItem.prefab.maxStack);
                if (remaining > 0) {
                }else {
                    // Other slot does not have enough room for this slots item,
                    // so it adds what it can and subtracts those from this slot.
                    newSlot.count -= remaining;
                    count += remaining;
                }
                // Other slot has enough room for this slots items, so it adds
                // this slots item and removes item from this slot.

                Debug.Log ("Enough space!");
                count += transferCount;

                newSlot.item = null;
                newSlot.count = 0;
            } else {
                // Both sides are not the same item, or any side doesn't have an item. Simply swap spaces.
                newSlot.item = item;
                newSlot.count = count;

                item = otherItem;
                count = otherCount;
            }
        }
    }
}
