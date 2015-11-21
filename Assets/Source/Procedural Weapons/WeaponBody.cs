using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponBody : WeaponPart {

	public enum AnimType { Rifle, Pistol, DualPistol, Heavy, Length }

	[Header ("Barrel")]
	public Transform[] barrelAttachmentPoint;
	public GameObject barrelPrefab;
	public WeaponBarrel barrel;

	[Header ("Optic")]
	public Transform opticsAttachmentPoint;
	public GameObject opticPrefab;
	public WeaponOptic optic;

	[Header ("Magazine")]
	public Transform magazineAttachmentPoint;
	public GameObject magazinePrefab;
	public WeaponMagazine magazine;

	[Header ("Underbarrel")]
	public Transform underBarrelAttachmentPoint;
	public GameObject underBarrelPrefab;
	public WeaponUnderbarrelAttachment underBarrel;

	[Header ("Stock")]
	public Transform stockAttachmentPoint;
	public GameObject stockPrefab;
	public WeaponStock stock;

	[Header ("Misc")]
	public Weapon weapon;
	public GameObject bullet;
	private bool isBuild;
	public AnimType animationType;

	public void BuildWeapon () {
		// Yeah this is pretty shit. It'll do, though.

		if (isBuild) {
			Debug.LogError ("Tried building already build weapon.");
			return;
		}

		weapon = transform.parent.GetComponent<Weapon>();
		
		if (barrelPrefab) {
			weapon.muzzles = new Transform[barrelAttachmentPoint.Length];
			weapon.barrels = new WeaponBarrel[barrelAttachmentPoint.Length];
			for (int i = 0; i < barrelAttachmentPoint.Length; i++) {
				GameObject b = (GameObject)Instantiate (barrelPrefab, barrelAttachmentPoint[i].position, Quaternion.identity);
				barrel = b.GetComponent<WeaponBarrel>();
				barrel.transform.parent = barrelAttachmentPoint[i];
				weapon.barrels[i] = barrel;
				weapon.muzzles[i] = barrel.muzzle;
				weapon.weaponParts.Add (barrel);
			}
		}
		if (stockPrefab && stockAttachmentPoint) {
			GameObject s = (GameObject)Instantiate (stockPrefab, stockAttachmentPoint.position, Quaternion.identity);
			stock = s.GetComponent<WeaponStock>();
			stock.transform.parent = stockAttachmentPoint;
			weapon.weaponParts.Add (stock);
		}
		if (opticPrefab && opticsAttachmentPoint) {
			GameObject o = (GameObject)Instantiate (opticPrefab, opticsAttachmentPoint.position, Quaternion.identity);
			optic = o.GetComponent<WeaponOptic>();
			optic.transform.parent = opticsAttachmentPoint;
			weapon.weaponParts.Add (optic);
		}
		if (underBarrelPrefab && underBarrelAttachmentPoint) {
			GameObject ub = (GameObject)Instantiate (underBarrelPrefab, underBarrelAttachmentPoint.position, Quaternion.identity);
			underBarrel = ub.GetComponent<WeaponUnderbarrelAttachment>();
			underBarrel.transform.parent = underBarrelAttachmentPoint;
			weapon.weaponParts.Add (underBarrel);
		}
		if (magazinePrefab) {
			GameObject ma = (GameObject)Instantiate (magazinePrefab, magazineAttachmentPoint.position, Quaternion.identity);
			magazine = ma.GetComponent<WeaponMagazine>();
			magazine.transform.parent = magazineAttachmentPoint;
			weapon.weaponParts.Add (magazine);
		}

		weapon.CombineData ();
		isBuild = true;
	}
}
