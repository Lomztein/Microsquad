using UnityEngine;
using System.Collections;

public class WeaponSpawn : PhysicalTool {

	public void OnEquip (CharacterEquipment.Equipment.EquipMessage message) {
		GameObject wep = SavedWeapon.LoadFromString (message.metadata).Build ();
		wep.transform.position = transform.position;
		wep.transform.rotation = transform.rotation;
		wep.transform.parent = transform;
        message.slot.physicalItem = gameObject;
        Debug.Log ("Generating weapon");
	}
}
