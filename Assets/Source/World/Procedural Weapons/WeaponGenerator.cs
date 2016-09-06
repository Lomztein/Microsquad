using UnityEngine;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class WeaponGenerator : MonoBehaviour {

	/*
	 * The weapon generator class contains references
	 * to every single available weapon part, as well
	 * as being able to generate weapons in the form
	 * of GameObjects, and in the form of a (future)
	 * SavedWeapons class.
	 */

	public GameObject weaponBasePrefab;
	public WeaponClass[] weaponClasses;
	public static WeaponGenerator cur;

	void Awake () {
		cur = this;
	}

	void Start () {
		Debug.Log (CalculateAvailableWeapons () + " total available weapons");
	}

	public GameObject GenerateAndBuildWeapon (WeaponClass c) {
		return GenerateWeapon (c).Build ();
	}

	public int GetClassIndex (WeaponClass c) {
		for (int i = 0; i < weaponClasses.Length; i++) {
			if (c == weaponClasses[i])
				return i;
		}
		return -1;
	}

	public string GenerateRandomWeaponData () {
		string str = "";
		int c = Random.Range (0, weaponClasses.Length);
		WeaponClass cl = weaponClasses[c];
		str += c;
		if (cl.bodies.Length > 0) {
			str += ":" + Random.Range (0, cl.bodies.Length);
		}else{
			str += ":-1";
		}
		if (cl.barrels.Length > 0) {
			str += ":" + Random.Range (0, cl.barrels.Length);
		}else{
			str += ":-1";
		}
		if (cl.optics.Length > 0) {
			str += ":" + Random.Range (0, cl.optics.Length);
		}else{
			str += ":-1";
		}
		if (cl.stocks.Length > 0) {
			str += ":" + Random.Range (0, cl.stocks.Length);
		}else{
			str += ":-1";
		}
		if (cl.underBarrels.Length > 0) {
			str += ":" + Random.Range (0, cl.underBarrels.Length);
		}else{
			str += ":-1";
		}
		if (cl.mags.Length > 0) {
			str += ":" + Random.Range (0, cl.mags.Length);
		}else{
			str += ":-1";
		}
		return str;
	}

	public SavedWeapon GenerateWeapon (WeaponClass c) {
		// Randomly choose and spawn every weapon part.
		SavedWeapon weapon = SavedWeapon.CreateInstance<SavedWeapon>();

		weapon.bodyID = Random.Range (0, c.bodies.Length);
		weapon.barrelID = Random.Range (0, c.barrels.Length);
		weapon.classID = GetClassIndex (c);

		if (c.stocks.Length > 0) {
			weapon.stockID = Random.Range (0, c.stocks.Length);
		}else{
			weapon.stockID = -1;
		}

		if (c.optics.Length > 0) {
			weapon.opticID = Random.Range (-c.optics.Length, c.optics.Length);
		}else{
			weapon.opticID = -1;
		}

		if (c.underBarrels.Length > 0) {
			weapon.underBarrelID = Random.Range (-(c.underBarrels.Length * 2), c.underBarrels.Length);
		}else{
			weapon.underBarrelID = -1;
		}

		if (c.mags.Length > 0) {
			weapon.magazineID = Random.Range (0, c.mags.Length);
		}else{
			weapon.magazineID = -1;
		}

		weapon.SaveToLongID ();
		return weapon;
	}

	int CalculateAvailableWeapons () {
		int amount = 0;
		for (int i = 0; i < weaponClasses.Length; i++) {
			WeaponClass c = weaponClasses[i];
			amount += Mathf.Max (1, c.bodies.Length) *
				Mathf.Max (1, c.barrels.Length) *
					Mathf.Max (1, c.stocks.Length) *
					Mathf.Max (1, c.optics.Length) *
					Mathf.Max (1, c.underBarrels.Length) *
					Mathf.Max (1, c.mags.Length);
		}
		return amount;
	}

}

[System.Serializable]
public class WeaponClass {

	public string className;
	public GameObject[] bodies;
	public GameObject[] barrels;
	public GameObject[] optics;
	public GameObject[] stocks;
	public GameObject[] underBarrels;
	public GameObject[] mags;

}

public class SavedWeapon : ScriptableObject {

	public string weaponName;
	public int classID;
	public int bodyID;
	public int barrelID;
	public int opticID;
	public int stockID;
	public int underBarrelID;
	public int magazineID;

	public GameObject Build () {
		GameObject w = (GameObject)Instantiate (WeaponGenerator.cur.weaponBasePrefab, Vector3.zero, Quaternion.identity);
		WeaponClass c = WeaponGenerator.cur.weaponClasses[classID];
		
		GameObject body = (GameObject)Instantiate (c.bodies[bodyID], w.transform.position, Quaternion.identity);
		WeaponBody wBody = body.GetComponent<WeaponBody>();
		w.GetComponent<Weapon>().body = wBody;
		body.transform.parent = w.transform;
		
		if (barrelID >= 0)
			wBody.barrelPrefab = c.barrels[barrelID];
		if (opticID >= 0)
			wBody.opticPrefab = c.optics[opticID];
		if (stockID >= 0)
			wBody.stockPrefab = c.stocks[stockID];
		if (underBarrelID >= 0)
			wBody.underBarrelPrefab = c.underBarrels[underBarrelID];
		if (magazineID >= 0)
			wBody.magazinePrefab = c.mags[magazineID];

		w.GetComponent<Weapon>().parse = SaveToString ();
		wBody.BuildWeapon ();
		return w;
	}

	/*
	 * Whatever I did here was incredibly clever
	 * and I'll likely never have such a momemnt
	 * of genius. That is, if it even works in the end.
	 * Never the less, there should be a total of
	 * 256 supported parts per catagory. More if
	 * the data type is changed to one above 64 bit.
	 * 
	 * I just realized that I can just serialize this shit.
	 * Fuck me. Or just parse it into a string. Though I
	 * did learn something from this. Was cool.
	 */

	public string SaveToString () {
		int[] IDs = new int[] { classID, bodyID, barrelID, opticID, stockID, underBarrelID, magazineID };
		string parse = classID.ToString ();
		for (int i = 1; i < IDs.Length; i++) {
			parse += ":"+IDs[i].ToString();
		}
		return parse;
	}

	public static SavedWeapon LoadFromString (string parse) {
		string[] numbers = parse.Split (':');
		SavedWeapon wep = SavedWeapon.CreateInstance<SavedWeapon>();
		wep.classID = int.Parse (numbers[0]);
		wep.bodyID = int.Parse (numbers[1]);
		wep.barrelID = int.Parse (numbers[2]);
		wep.opticID = int.Parse (numbers[3]);
		wep.stockID = int.Parse (numbers[4]);
		wep.underBarrelID = int.Parse (numbers[5]);
		wep.magazineID = int.Parse (numbers[6]);
		return wep;
	}

	public ulong SaveToLongID () {
		int[] IDs = new int[] { classID, bodyID, barrelID, opticID, stockID, underBarrelID, magazineID };

		int bitsPerPart = Mathf.FloorToInt (64 / IDs.Length);
		ulong maxPartsPerCatagory = (ulong)Mathf.FloorToInt (Mathf.Pow (2, bitsPerPart - 1));
		ulong bits = 0;

		for (int i = 0; i < IDs.Length; i++) {
			if (IDs[i] > (int)maxPartsPerCatagory) {
				Debug.LogError ("Too many parts at index " + i + ", maximum is " + maxPartsPerCatagory);
			}else{
				if (IDs[i] >= 0) {
					bits = (bits | (ulong)IDs[i] | maxPartsPerCatagory);
				}
				bits = bits << bitsPerPart;
			}
		}

		return bits;
	}

	public static SavedWeapon LoadFromLongID (ulong LID) {
		int[] IDs = new int[7];
		
		int bitsPerPart = Mathf.FloorToInt (64 / IDs.Length);
		ulong maxPartsPerCatagory = (ulong)Mathf.FloorToInt (Mathf.Pow (2, bitsPerPart - 1));

		for (int i = 0; i < IDs.Length; i++) {
			if ((maxPartsPerCatagory & LID) == maxPartsPerCatagory) {
				Debug.Log ((int)((maxPartsPerCatagory - 1) & LID));
				IDs[i] = (int)((maxPartsPerCatagory - 1) & LID);
			}else{
				IDs[i] = -1;
			}
			LID = LID >> bitsPerPart;
		}

		SavedWeapon wep = SavedWeapon.CreateInstance<SavedWeapon>();
		wep.classID = IDs[6];
		wep.bodyID = IDs[5];
		wep.barrelID = IDs[4];
		wep.opticID = IDs[3];
		wep.stockID = IDs[2];
		wep.underBarrelID = IDs[1];
		wep.magazineID = IDs[0];

		return wep;
	}
}