using UnityEngine;
using System.Collections;

public class CharacterAI : MonoBehaviour {

	public enum AIState { Idle, Searching, Attacking, Fleeing, Aimless };
	public static float updateDeltaTime = 0.5f;
	
	[Header ("AI")]
	public Transform target;
	public Vector3 targetPos;
	public AIState state;
	public Character character;

	/*
	 * The general gist of this AI is pretty basic, as
	 * more advanced AI will inherit from this class.
	 * Therefore, this will only contain the most basic
	 * functionality, as well as some basic AI procedures.
	 * 
	 * The target variable is an empty transform, spawned by
	 * this class. Once a proper *target* is found, the *target*
	 * will parent the target variable, and the AI will attack.
	 * If line of sight is lost, the target variable will orphaned,
	 * and as long as there is no parent target, the AI will simply
	 * chase it to, per say, the last known target location.
	 * 
	 * Clever shit yo.
	 */

	void Start () {
		StartCoroutine (AIUpdate ());
	}

	// Update is called once per frame
	IEnumerator AIUpdate () {
		while (enabled) {
			if (!target) {
				state = AIState.Idle;
				FindTarget ();
				if (target) {
					state = AIState.Searching;
				}
			}
			if (state == AIState.Idle) {
				targetPos = transform.position;
			}
			if (state == AIState.Searching) {
				targetPos = target.position;
				if (Vector3.Distance (transform.position, target.position) > character.CalcOptics ().y) {
					MoveToTarget ();
				}else{
					AttackTarget ();
				}
			}
			yield return new WaitForSeconds (updateDeltaTime);
		}
	}

	void AttackTarget () {
		Character c = target.GetComponent<Character>();
		if (c) {
			character.ClearCommands ();
			Command.KillCommand (transform.position, target, character.CalcOptics ().y, character.speed, character, character.CalcDPS (), c.health);
		}
	}

	void FindTarget () {
		target = TargetFinder.FindTarget (transform, transform.position, character.CalcOptics ().y * 1.5f, Game.game.all);
	}

	void MoveToTarget () {
		Command.MoveCommand (transform.position, targetPos, target, character.speed, character);
	}

	void OnDrawGizmos () {
		if (target) Gizmos.DrawSphere (target.position, 0.25f);
		Gizmos.DrawWireSphere (targetPos, 0.25f);
	}
}

public class TargetFinder {

	public static Transform FindTarget (Transform exclude, Vector3 start, float range, LayerMask layer) {
		Collider[] nearby = Physics.OverlapSphere (start, range, layer);
		Transform nearest = null;
		float distance = float.MaxValue;

		for (int i = 0; i < nearby.Length; i++) {
			if (nearby[i].transform == exclude)
				continue;

			float d = Vector3.Distance (nearby[i].transform.position, start);
			if (d < distance) {
				distance = d;
				nearest = nearby[i].transform;
			}
		}

		return nearest;
	}

}
