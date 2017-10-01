using UnityEngine;
using System.Collections;

public class WeaponItemPrefab : ItemPrefab, IEquipable {

    public static CharacterEquipment.Slot weaponSlot;

    public GameObject GetEquipmentObject() {
        return gameObject;
    }

    public CharacterEquipment.Slot GetSlotType() {
        return weaponSlot;
    }
}
