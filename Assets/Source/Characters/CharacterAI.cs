using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterAI : MonoBehaviour {

    public enum State { Idle, Moving, Attacking, Fleeing, Aimless, Interacting };
    public static float updateDeltaTime = 0.5f;

    public enum Stance { Guard, Aggressive, Passive }
    public Stance stance = Stance.Guard;

    [Header ("AI")]
    public Character character;
    public GameObject objective;
    public UnityEngine.AI.NavMeshAgent navigationAgent;
    public float fieldOfView;

    [Header ("Commands")]
    public List<Command> commands = new List<Command>();
    private Command currentCommand;
    public State state;
    public UnityEngine.AI.NavMeshPath pathToCommand;
    public int pathIndex;
    private Transform pointer;

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

        navigationAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
        GameObject p = new GameObject ("Pointer");
        pointer = p.transform;
        pointer.transform.position = transform.position;
        pointer.parent = transform;
    }

    // Update is called once per frame
    IEnumerator AIUpdate () {
        while (enabled) {
            yield return new WaitForSeconds (updateDeltaTime);
            if (commands.Count == 0) {
                switch (stance) {
                    case Stance.Guard:
                        Transform target = FindTarget ();
                        if (target) {
                            AttackTarget (target);
                        } else {
                            FindTarget ();
                        }
                        break;
                }
            }
        }
    }

    void AttackTarget ( Transform target ) {
        if (!currentCommand)
            Command.KillCommand (target, this, character.health);
    }

    Transform FindTarget () {
        return TargetFinder.FindTarget (transform, transform.position, sightRange, Game.game.all - Game.game.layers[(int)character.faction], character);
    }

    void OnDrawGizmos () {
        if (commands.Count > 0)
            Gizmos.DrawLine (transform.position, commands[0].position);
        if (pathToCommand != null) {
            for (int i = 0; i < pathToCommand.corners.Length - 1; i++) {
                Gizmos.DrawLine (pathToCommand.corners[i], pathToCommand.corners[i + 1]);
            }
        }

        Gizmos.DrawLine (transform.position, transform.position + Quaternion.Euler (0f, transform.eulerAngles.y + fieldOfView, 0f) * new Vector3 (0f, 0f, sightRange));
        Gizmos.DrawLine (transform.position, transform.position + Quaternion.Euler (0f, transform.eulerAngles.y - fieldOfView, 0f) * new Vector3 (0f, 0f, sightRange));
    }

    void OnTakeDamage ( Damage d ) {
        if (d.character) {
            AlertNearbyAllies (d.character.transform);
        }
    }

    void AlertNearbyAllies ( Transform attacker ) {
        Collider[] allies = Physics.OverlapSphere (transform.position, sightRange, Game.FactionLayer (character.faction));
        for (int i = 0; i < allies.Length; i++) {
            CharacterAI ch = allies[i].GetComponent<CharacterAI> ();
            if (ch && Utility.LineOfSight (transform.position + Vector3.up, ch.transform.position + Vector3.up))
                ch.AttackTarget (attacker);
        }
    }

    void RotateTowardsPosition (Vector3 targetPos) {
        if (Vector3.Distance (transform.position, targetPos) > (character.speed + 0.1f) * Time.fixedDeltaTime)
            transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (0f, pointer.eulerAngles.y, 0f), Time.fixedDeltaTime * character.stats.strength / character.CalcWepWeight () * 360f);
        pointer.LookAt (targetPos);
    }

    void FixedUpdate () {
		SetNextCommand ();

        // TODO: Change activeWeapons to a single weapon or tool reference.
        if (character.activeWeapon) {
            Transform tran = character.activeWeapon.transform;
            tran.position = Vector3.Lerp (tran.position, character.toolSlot.transform.position, character.stats.recoilRecovery * Time.fixedDeltaTime);
            tran.rotation = Quaternion.Slerp (tran.rotation, character.toolSlot.transform.rotation, character.stats.recoilRecovery / 5f * Time.fixedDeltaTime);
        }
    }

    private void FindPathToCommand () {
        pathToCommand = new UnityEngine.AI.NavMeshPath ();
        navigationAgent.CalculatePath (commands[0].position, pathToCommand);
        pathIndex = 1;
    }

    void MoveTowardsPosition ( Vector3 position ) {
        Vector3 dir = (position - transform.position).normalized;
        character.character.Move (dir * character.speed * Time.deltaTime);
    }

    void FireWeapons (float damageMul = 1f) {
        if (character.activeWeapon)
            if (transform.rotation == Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (0f, pointer.eulerAngles.y, 0f), 5))
                character.activeWeapon.Fire (character.faction, currentCommand.target, damageMul);

        SendMessage ("OnFireWeapons", SendMessageOptions.DontRequireReceiver);
    }

    public void CompleteCommand () {
        commands.Remove (currentCommand);
        Destroy (currentCommand);
        currentCommand = null;
        character.animator.SetFloat ("Speed", 0);
        ClearPathfindingAgent ();
        state = State.Idle;
    }

    public void AddCommand ( Command command ) {
        commands.Add (command);
    }

    public void AddCommand ( List<Command> command ) {
        for (int i = 0; i < command.Count; i++)
            commands.Add (command[i]);
    }

    public void ClearCommands ( Command command = null ) {
        StopAllCoroutines ();
        for (int i = 0; i < commands.Count; i++)
            Destroy (commands[i]);
        commands.Clear ();
        if (command != null)
            AddCommand (command);
    }

    // Returns true if it has reached the destination.
    private bool MoveTowardsCommand () {
        if (pathToCommand != null) {
            if (Vector3.Distance (transform.position, NextPathPosition ()) > (character.speed + 0.1f) * Time.fixedDeltaTime) {
                Vector3 targetPos = NextPathPosition ();
                MoveTowardsPosition (targetPos);
                RotateTowardsPosition (targetPos);
            } else {
                pathIndex++;
                if (pathIndex == pathToCommand.corners.Length)
                    return true;
            }
        }

        return false;
    }

    void SetNextCommand () {
        if (commands.Count == 0 || currentCommand != null)
            return;

        currentCommand = commands[0];
        switch (currentCommand.type) {
            case Command.Type.Move:
                StartCoroutine (DoMoveCommand ());
                Game.AddMessage (character.unitName + " moves to " + currentCommand.position.ToString ());
                break;

            case Command.Type.Kill:
                StartCoroutine (DoKillCommand ());
                Game.AddMessage (character.unitName + " goes for the kill.");
                break;

            case Command.Type.Interact:
                StartCoroutine (DoInteractCommand ());
                Game.AddMessage (character.unitName + " goes to interact.");
                break;

            case Command.Type.Execute:
                StartCoroutine (DoExecuteCommand ());
                Game.AddMessage (character.unitName + "goes to execute someone.");
                break;
        }
    }

    private Vector3 NextPathPosition () {

        if (pathToCommand != null && pathToCommand.corners.Length > 0) {
            return pathToCommand.corners[pathIndex];
        } else {
            if (commands.Count == 0) {
                return transform.position;
            } else {
                return commands[0].GetPosition ();
            }
        }
    }

    private void ClearPathfindingAgent () {
        if (pathToCommand.corners.Length != 0)
            pathToCommand.ClearCorners ();
    }

    // This function is taken directly from old attack command code, and simply checks the targets positions distance from the last pathfinding corner
    // and resets and refinds the path if they have gotten too far apart, making sure the path is on the right way even if the target has moved.
    private void CheckAndResetPath (Transform target) {
        if (pathToCommand != null && pathToCommand.corners.Length > 0) {

            Vector3 t = new Vector3 (target.position.x, 0f, target.position.z);
            Vector3 d = new Vector3 (pathToCommand.corners[pathToCommand.corners.Length - 1].x, 0f, pathToCommand.corners[pathToCommand.corners.Length - 1].z);

            if (Vector3.Distance (t, d) > 0.1f) {
                commands[0].position = target.position;
                FindPathToCommand ();
            }
        }
    }

    private bool CanSeeTarget (Transform target) {
        return character.ObjectVisibleFromHeadbone (target) && Vector3.Distance (target.position, transform.position) < character.CalcOptics ().y;
    }

    // Try and keep commands in the buttom of the class, so they at least are together.
    IEnumerator DoMoveCommand () {
        FindPathToCommand ();

        while (!MoveTowardsCommand ())
            yield return new WaitForFixedUpdate ();

        CompleteCommand ();
    }

    IEnumerator DoKillCommand () {
        // Search for the character object in the target, and find his chest.
        Character targetChar = currentCommand.target.GetComponent<Character> ();
        Transform target = targetChar.FindSlotByType (CharacterEquipment.Slot.Chest).transform;

        FindPathToCommand ();
        while (targetChar) {
            if (CanSeeTarget (target)) {
                RotateTowardsPosition (targetChar.transform.position);
                pathIndex = 1;
                FireWeapons ();
            } else {
                MoveTowardsCommand ();
            }

            CheckAndResetPath (target);

            yield return new WaitForFixedUpdate ();
        }

        CompleteCommand ();
    }

    IEnumerator DoInteractCommand () {
        float distance = currentCommand.attributes.GetAttribute<float> ("InteractRange");
        string interactCommand = currentCommand.metadata;
        Transform target = currentCommand.target;

        FindPathToCommand ();
        while (Vector3.Distance (transform.position, target.position) > distance) {
            MoveTowardsCommand ();
            yield return new WaitForFixedUpdate ();
        }

        target.SendMessage (interactCommand);
        CompleteCommand ();
    }

    // This command is very similar in code to the kill command. Perhaps find a way to generalize parts of it.
    IEnumerator DoExecuteCommand () {
        // Search for the character object in the target, and find his head.
        Character targetChar = currentCommand.target.GetComponent<Character> ();
        Transform target = targetChar.FindSlotByType (CharacterEquipment.Slot.Head).transform;

        FindPathToCommand ();

        while (targetChar) {
            if (CanSeeTarget (target)) {
                pathIndex = 1;

                bool doFire = true;
                for (int i = 0; i < 50 * 2; i++) {

                    if (!targetChar) {
                        doFire = false;
                        break;
                    }

                    RotateTowardsPosition (targetChar.transform.position);
                    yield return new WaitForFixedUpdate ();
                }

                if (doFire)
                    FireWeapons (10f);
            } else {
                MoveTowardsCommand ();
            }

            CheckAndResetPath (target);

            yield return new WaitForFixedUpdate ();
        }

        CompleteCommand ();
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

            Vector3 dirTo = (nearby[i].transform.position - character.transform.position).normalized;
            float relAngle = Vector3.Dot (character.transform.forward, dirTo);
            float view = 1 - (character.ai.fieldOfView / 90f);

			float d = Vector3.Distance (nearby[i].transform.position, start);
			if (d < distance && relAngle > view) {

				distance = d;
				nearest = nearby[i].transform;
			}
		}

		return nearest;
	}
}
