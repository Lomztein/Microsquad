using UnityEngine;
using System.Collections;

public class Armor : EquippedItem {

    // Armor rating defines how much the incoming damage is reduced.
    public int armorRating;

    // Armor hardness defines the armors resistance against armor piercing rounds.
    public float hardness;

    public void OnEquip (CharacterEquipment.Equipment.EquipMessage message) {
        message.character.armorPieces.Add (this);
    }

    public void UnEquip (CharacterEquipment.Equipment.EquipMessage message) {
        message.character.armorPieces.Remove (this);
    }
}