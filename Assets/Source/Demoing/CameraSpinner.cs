using UnityEngine;
using System.Collections;

public class CameraSpinner : MonoBehaviour {

	public WeaponGenerator generator;
	public Transform weapon;

	public float waitTime;
	public float distance;
	public Weapon w;
	private int genNumber;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Generate", waitTime, waitTime);
		Generate ();
	}

	void Generate () {
		int index = Random.Range (0, generator.weaponClasses.Length);
		if (weapon)
			Destroy (weapon.gameObject);
		weapon = generator.GenerateAndBuildWeapon (generator.weaponClasses[index]).transform;
		w = weapon.GetComponent<Weapon>();

		genNumber++;
		SavedWeapon.LoadFromString (w.parse).Build ().transform.position += Vector3.right * genNumber;
	}
	
	// Update is called once per frame
	void Update () {
		weapon.Rotate (0f, 360f / waitTime * Time.deltaTime , 0f);
	}

	void OnGUI () {
		if (weapon) GUI.Label (new Rect (10f, 10f, 600f, 2000f), w.weaponName + ", LID: " + w.parse  + "\n" + w.combinedStats.ToString ());
	}
}
