using UnityEngine;
using System.Collections;

public class WeaponItemPrefab : ItemPrefab, IEquipable, IContainsItem {

    public CharacterEquipment.Type type = CharacterEquipment.Type.Hand;
    public Inventory.Slot slot;

    public Inventory.Slot Slot {
        get {
            return slot;
        }
    }

    public GameObject GetEquipmentObject() {
        return gameObject;
    }

    public CharacterEquipment.Type GetSlotType() {
        return type;
    }
}
