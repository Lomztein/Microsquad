﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour, IContainsItem {

	public string weaponName;
    // Name consists of the individual parts name mods combined.
    // First the barrel, then scope, then body.

    public Character character;
	public WeaponBody body;
	public WeaponStats combinedStats;
	public Transform[] muzzles;
	public WeaponBarrel[] barrels;
	public List<WeaponPart> weaponParts;
	public string parse;

    public Inventory.Slot ammoSlot;

    private Projectile currentProjectile;

	private bool chambered = true;
    private bool isReloading = false;

    public Inventory.Slot Slot {
        get {
            return ammoSlot;
        }
    }

    void Start () {
		CombineData ();
	}

	public float CalcDPS () {
		float damage = 0f;
        if (body.currentAmmoPrefab)
            body.currentAmmoPrefab.gameObject.GetComponent<Projectile> ().CalcDamage ();

        return damage / combinedStats.firerate * muzzles.Length;
	}

	public Vector2 GetOpticSpeed () {
		if (body.optic) 
			return new Vector2 (body.optic.aimSpeed, body.optic.maxSightRange);
		return new Vector2 (0.2f, 15f);
	}

	public void CombineData () {
		string newName = "";
		if (body.barrel)
			newName += body.barrel.nameMod;
		if (body.optic)
			newName += " " + body.optic.nameMod;
		if (body.stock)
			newName += " " + body.stock.nameMod;
		newName += " " + body.nameMod;
		if (body.underBarrel)
			newName += " with " + body.underBarrel.nameMod;

		weaponName = newName;
		WeaponStats stats = body.stats.Clone ();

		for (int i = 0; i < weaponParts.Count; i++) {
			weaponParts[i].stats.CombineWith (stats);
		}

		combinedStats = stats;
	}

	public void Fire (Faction faction, Transform target, float damageMultiplier = 1f) {
        bool hasAmmo = Slot.count != 0;

		if (chambered && hasAmmo) {
			StartCoroutine (DoFire (faction, target, damageMultiplier));
		}else if (!hasAmmo && !isReloading) {
            Game.AddMessage (character.unitName + " is reloading!");

            isReloading = true;
            InvokeReload ();
        }
	}

    public void InvokeReload () {
        Invoke ("Reload", body.magazine.reloadTime);
    }

    public void UpdateAmmunition () {
        if (!character)
            return;

        if (Slot.item) {
            body.currentAmmoPrefab = Slot.item.prefab;
            currentProjectile = body.currentAmmoPrefab.gameObject.GetComponent<Projectile> ();
        }else {
            body.currentAmmoPrefab = null;
            currentProjectile = null;
        }
    }

    public void Reload () {
        // When the weapon needs to reload, it first searches its parent character for an ammo slot.
        // If no ammo slot is found, it simply reloads all bullets, so that certain characters can have infinite ammo if needed.

        if (character.inventory) {
            for (int i = 0; i < character.inventory.size; i++) {
                Inventory.Slot slot = character.inventory.slots [ i ];
                if (slot.item) {

                    IAmmo ammo = slot.item.prefab as IAmmo;
                    if (ammo != null) {
                        if (ammo.GetAmmoType () == body.ammoType) {
                            slot.MoveItem (Slot);
                        }
                    }
                }
            }
        }

        isReloading = false;
        chambered = true;
    }

	IEnumerator DoFire (Faction faction, Transform target, float damageMul = 1f) {

		chambered = false;

		float f = combinedStats.firerate / (float)muzzles.Length;

		for (int i = 0; i < muzzles.Length; i++) {
            float recoil = 0f;
			for (int j = 0; j < currentProjectile.bulletAmount; j++) {

                if (target) muzzles[i].LookAt (new Vector3 (target.position.x, transform.position.y, target.position.z));
				GameObject bul = (GameObject)Instantiate (body.currentAmmoPrefab.gameObject, muzzles[i].position, muzzles[i].rotation);
				Projectile pro = bul.GetComponent<Projectile>();
				FeedBulletData (pro, muzzles[i], faction);
                pro.weight *= damageMul;
				barrels[i].Flash ();
                recoil += combinedStats.recoil;

				if (body.fireSound)
					body.audioSource.PlayOneShot (body.fireSound, Game.soundVolume);

			}
            character.WeaponRecoil (transform.parent, recoil);

			Slot.ChangeCount (-1);

            if (body.casingEjector)
                body.casingEjector.Emit (1);

            if (i != muzzles.Length - 1) yield return new WaitForSeconds (f);
		}
		Invoke ("Rechamber", combinedStats.firerate);
    }

	void Rechamber () {
		chambered = true;
	}

	void FeedBulletData (Projectile pro, Transform muzzle, Faction faction) {

		pro.velocity = muzzle.forward * Random.Range (0.9f, 1.1f) * combinedStats.speed * pro.speed
			+ muzzle.up * Random.Range (-combinedStats.spread, combinedStats.spread)
				+ muzzle.right * Random.Range (-combinedStats.spread, combinedStats.spread);

		pro.faction = faction;
		pro.range = GetOpticSpeed ().y * 1.5f;
        pro.character = character;
	}
}
