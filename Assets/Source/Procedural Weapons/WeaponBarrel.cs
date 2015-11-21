using UnityEngine;
using System.Collections;

public class WeaponBarrel : WeaponPart {

	public Transform muzzle;

	public Light flash;
	public float flashTime;

	private Vector3 flashScale;

	void Start () {
		if (flash.gameObject)
			flashScale = flash.transform.localScale;
	}

	public void Flash () {
		if (!flash.gameObject.activeInHierarchy) StartCoroutine (DoFlash ());
	}

	IEnumerator DoFlash () {

		flash.intensity = 10f;
		flash.transform.localScale = flashScale;

		flash.gameObject.SetActive (true);
		flash.transform.localRotation = Quaternion.Euler (Random.Range (0f, 360f), 90f, 0f);
		flash.transform.localScale = flashScale + Random.insideUnitSphere / 10f * flashScale.magnitude;

		while (flash.intensity > 0.1f) {
			yield return new WaitForFixedUpdate ();
			flash.intensity /= flashTime;
			flash.transform.localScale /= flashTime;
		}
		flash.gameObject.SetActive (false);
	}
	
}
