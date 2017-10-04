using UnityEngine;
using System.Collections;

public class WeaponItemPrefab : ItemPrefab, IEquipable {

    public CharacterEquipment.Type type = CharacterEquipment.Type.Hand;

    public GameObject GetEquipmentObject() {
        return gameObject;
    }

    public CharacterEquipment.Type GetSlotType() {
        return type;
    }

    public override Item GetItem() {
        Item baseItem = base.GetItem ();
        baseItem.attributes.AddAttribute ("AmmoSlot", Inventory.Slot.CreateSlot ());
        return baseItem;
    }
}
