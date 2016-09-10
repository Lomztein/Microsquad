using UnityEngine;
using System.Collections;

public class WeaponSpawn : EquippedItem {

	public void OnEquip (CharacterEquipment.Equipment.EquipMessage message) {
		GameObject wep = SavedWeapon.LoadFromString (message.metadata).Build ();
		wep.transform.position = transform.position;
		wep.transform.rotation = transform.rotation;
		wep.transform.parent = transform;
        wep.GetComponent<Weapon> ().character = message.character;

        if (message.slot != null)
            message.slot.equippedItem = gameObject;
	}

    public void OnUnEquip ( CharacterEquipment.Equipment.EquipMessage message ) {
        message.character.activeWeapons.Remove (GetComponentInChildren<Weapon>());
    }
}
