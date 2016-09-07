using UnityEngine;
using System.Collections;

public class Turret : Unit {

	public Vector3 rotationSpeeds;

	void Update () {
		transform.Rotate (0f, 0f, 30f * Time.deltaTime);
	}
	
}
