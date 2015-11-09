using UnityEngine;
using System.Collections;

public class CombatSimulator : MonoBehaviour {

	public GameObject character;
	public float spawnRange;

	[Range (0f, 50f)]
	public int avgSpawnsPerSec;

	// Update is called once per frame
	void FixedUpdate () {

		if (Random.Range (avgSpawnsPerSec, 51) == avgSpawnsPerSec) {
			Vector3 spawnPos = new Vector3 (Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * spawnRange + Vector3.up * 1f;
			Instantiate (character, spawnPos, Quaternion.identity);
		}
	
	}
}
