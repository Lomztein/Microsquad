using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemPrefab : ItemPrefab, IEquipable {

    public CharacterEquipment.Type slot;

    public GameObject GetEquipmentObject() {
        return gameObject;
    }

    public CharacterEquipment.Type GetSlotType() {
        return slot;
    }
}
