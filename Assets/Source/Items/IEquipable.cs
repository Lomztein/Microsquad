using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipable {

    GameObject GetEquipmentObject();

    CharacterEquipment.Slot GetSlotType();

}
