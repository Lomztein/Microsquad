using UnityEngine;
using System.Collections;

public class WeaponSpawn : PhysicalTool {

	public void OnEquip (string data) {
		GameObject wep = SavedWeapon.LoadFromString (data).Build ();
		wep.transform.position = transform.position;
		wep.transform.rotation = transform.rotation;
		wep.transform.parent = transform;
	}
}
