using UnityEngine;
using System.Collections;

public class CharacterAI : MonoBehaviour {

	public static float updateDeltaTime = 0.5f;

    public enum Stance { Guard, Aggressive, Passive }
    public Stance stance = Stance.Guard;
	
	[Header ("AI")]
	public Character character;
	public GameObject objective;

    /// <summary>
    /// The range at which the AI will automatically engage an enemy.
    /// </summary>
    public float sightRange = 25f;

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
        character = GetComponent<Character> ();
		objective = GameObject.FindGameObjectWithTag ("Objective");
		StartCoroutine (AIUpdate ());
	}

	// Update is called once per frame
	IEnumerator AIUpdate () {
		while (enabled) {
			yield return new WaitForSeconds (updateDeltaTime);
            if (character.commands.Count == 0) {
                switch (stance) {
                    case Stance.Guard:
                        if (character.target) {
                            AttackTarget ();
                        } else {
                            FindTarget ();
                        }
                        break;
                }
            }
		}
	}

	void AttackTarget () {
        AttackTarget (character.target);
	}

    void AttackTarget (Transform target) {
        Command.KillCommand (target, character, character.health);
    }

    void FindTarget () {
		character.target = TargetFinder.FindTarget (transform, transform.position, sightRange, Game.game.all - Game.game.layers[(int)character.faction], character);
	}

	void OnDrawGizmos () {
		if (character.target) Gizmos.DrawSphere (character.target.position, 0.25f);
		Gizmos.DrawWireSphere (character.targetPos, 0.25f);
	}

    void OnTakeDamage ( Damage d ) {
        if (d.character) {
            AttackTarget (d.character.transform);
            if (character.target != d.character.transform)
                AlertNearbyAllies (d.character.transform);
        }
    }

    void AlertNearbyAllies (Transform attacker) {
        Collider[] allies = Physics.OverlapSphere (transform.position, sightRange, Game.FactionLayer (character.faction));
        for (int i = 0; i < allies.Length; i++) {
            CharacterAI ch = allies[i].GetComponent<CharacterAI> ();
            if (ch && Utility.LineOfSight (transform.position + Vector3.up, ch.transform.position + Vector3.up))
                    ch.AttackTarget (attacker);
        }
    }
}

public class TargetFinder {

	public static Transform FindTarget (Transform exclude, Vector3 start, float range, LayerMask layer, Character character) {
		Collider[] nearby = Physics.OverlapSphere (start, range, layer);
		Transform nearest = null;
		float distance = float.MaxValue;

		for (int i = 0; i < nearby.Length; i++) {
			if (nearby[i].transform == exclude || !character.ObjectVisibleFromHeadbone (nearby[i].transform))
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
