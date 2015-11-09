using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

	public string weaponName;
	// Name consists of the individual parts name mods combined.
	// First the barrel, then scope, then body.

	public WeaponBody body;
	public WeaponStats combinedStats;
	public Transform[] muzzles;
	public List<WeaponPart> weaponParts;
	public string parse;

	private bool chambered = true;

	void Start () {
		CombineData ();
	}

	public float CalcDPS () {
		float damage = body.bullet.GetComponent<Projectile>().damage;
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

	public void Fire (Character.Faction faction, Transform target) {
		if (chambered &&
		    body.magazine.currentAmmo > 0) {
			StartCoroutine (DoFire (faction, target));
		}else if (body.magazine.currentAmmo == 0) {
			Invoke ("Reload", body.magazine.reloadTime);
		}
	}

	void Reload () {
		body.magazine.currentAmmo = body.magazine.maxAmmo;
	}

	IEnumerator DoFire (Character.Faction faction, Transform target) {
		chambered = false;
		float f = combinedStats.firerate / (float)muzzles.Length;
		for (int i = 0; i < muzzles.Length; i++) {
			for (int j = 0; j < combinedStats.bulletAmount; j++) {

				if (target) muzzles[i].LookAt (new Vector3 (target.position.x, transform.position.y, target.position.z));
				GameObject bul = (GameObject)Instantiate (body.bullet, muzzles[i].position, muzzles[i].rotation);
				Projectile pro = bul.GetComponent<Projectile>();
				FeedBulletData (pro, muzzles[i], faction);
			}
			body.magazine.currentAmmo--;
			if (i != muzzles.Length - 1) yield return new WaitForSeconds (f);
		}
		Invoke ("Rechamber", combinedStats.firerate);
	}

	void Rechamber () {
		chambered = true;
	}

	void FeedBulletData (Projectile pro, Transform muzzle, Character.Faction faction) {

		pro.velocity = muzzle.forward * Random.Range (0.9f, 1.1f) * combinedStats.speed * pro.speed
			+ muzzle.up * Random.Range (-combinedStats.spread, combinedStats.spread)
				+ muzzle.right * Random.Range (-combinedStats.spread, combinedStats.spread);

		pro.damage = (int)combinedStats.damage;
		pro.faction = faction;
		pro.range = GetOpticSpeed ().y;
	}
}
