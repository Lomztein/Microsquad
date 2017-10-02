﻿using UnityEngine;
using System.Collections;

public class WeaponSpawn : EquippedItem {

	public void OnEquip (CharacterEquipment.Slot.EquipMessage message) {
		GameObject wep = SavedWeapon.LoadFromString (message.metadata).Build ();
		wep.transform.position = transform.position;
		wep.transform.rotation = transform.rotation;
		wep.transform.parent = transform;

        Weapon w = wep.GetComponent<Weapon> ();
        if (message.character) {
            w.character = message.character;

            if (message.slot != null)
                message.slot.equippedItem = gameObject;

            message.character.activeWeapon = w;
            w.Reload ();
        }

        w.UpdateAmmunition ();
	}

    public void OnUnEquip ( CharacterEquipment.Slot.EquipMessage message ) {
        message.character.activeWeapon = null;
    }
}
