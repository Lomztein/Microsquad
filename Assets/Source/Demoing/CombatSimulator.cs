using UnityEngine;
using System.Collections;

public class CombatSimulator : MonoBehaviour {

	public GameObject character;
	public float spawnRange;

	[Range (0f, 50f)]
	public int avgSpawnsPerSec;
	public Transform[] spawnPoints;

	public int unitAmount;
	public float unitSize;

	// Update is called once per frame
	void FixedUpdate () {
		if (Random.Range (avgSpawnsPerSec, 51) == avgSpawnsPerSec) {
			int index = Random.Range (0, spawnPoints.Length);

			Vector3 spawnPos = spawnPoints[index].position + new Vector3 (Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * spawnRange + Vector3.up * 1f;
			GameObject pawn = (GameObject)Instantiate (character, spawnPos, Quaternion.identity);

			Character c = pawn.GetComponent<Character>();
			c.SetFaction ((Faction)index);
		}
	}

	void OnDrawGizmos () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			Vector3[] positions = Micromanagement.GetSpriralPositions (unitSize, unitAmount);
			foreach (Vector3 pos in positions)
				Gizmos.DrawSphere (hit.point + pos, unitSize / 2f);
		}
	}
}
