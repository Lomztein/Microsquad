using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICommander : MonoBehaviour {

	public static float updateTime = 0.5f;
	public Faction faction;

	public List<Character> currentUnits = new List<Character>();
	public GameObject characterPrefab;

	public float threatCompensation = 1.5f;
	public Transform objective;

	// Use this for initialization
	void Start () {
		StartCoroutine (AIUpdate ());
	}
	
	IEnumerator AIUpdate () {
		while (enabled) {
			for (int i = 0; i < currentUnits.Count; i++) {
				if (!currentUnits[i])
					currentUnits.RemoveAt (i);
			}

			GameObject c = (GameObject)Instantiate (characterPrefab, transform.position, Quaternion.identity);
			Character ch = c.GetComponent<Character>();
			ch.SetFaction (faction);
			currentUnits.Add (ch);

			int nearby = GetNearbyEnemyCount (objective.position, 10);
			Debug.Log (nearby * threatCompensation + ", " + currentUnits.Count);
			if (nearby * threatCompensation < currentUnits.Count && nearby != 0) 
				FullAttackOnObjective (nearby);

			if (nearby == 0 && currentUnits.Count > 10)
				FullAttackOnObjective (11);

			yield return new WaitForSeconds (updateTime);
		}
	}

	void FullAttackOnObjective (int amount) {
		Vector3[] positions = Micromanagement.GetSpriralPositions (1f, currentUnits.Count);
		for (int i = 0; i < currentUnits.Count; i++) {
			if (i < amount * threatCompensation) {
				
				currentUnits.RemoveAt (i);
			}
		}
	}

	int GetNearbyEnemyCount (Vector3 pos, float range) {
		return Physics.OverlapSphere (pos, range, Game.game.all - Game.FactionLayer (faction)).Length;
	}
}

