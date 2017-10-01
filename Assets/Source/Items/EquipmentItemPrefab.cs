using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemPrefab : ItemPrefab, IEquipable {

    public CharacterEquipment.Slot slot;

    public GameObject GetEquipmentObject() {
        return gameObject;
    }

    public CharacterEquipment.Slot GetSlotType() {
        return slot;
    }
}
