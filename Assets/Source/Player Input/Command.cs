using UnityEngine;
using System.Collections.Generic;

public class Command : ScriptableObject {

	public enum Type { Move, Kill, Defend }
	public static Color[] colors = new Color[] { Color.green, Color.red, Color.blue };

	public Vector3 position;
	public Transform target;
	public float time;
	public Type type;

	void OnDestroy () {
		if (target && target.tag == "CommandPointer")
			Destroy (target.gameObject);
	}

    public static List<Command> MoveCommand ( Vector3 start, Vector3 end, Transform target, float speed, Character character ) {
        Debug.Log ("what");
        NavMeshPath path = new NavMeshPath ();
        character.navigationAgent.CalculatePath (end, path);
        Vector3[] points = path.corners;
        List<Command> commands = new List<Command> ();

        for (int i = 0; i < path.corners.Length - 1; i++) {

            Vector3 current = path.corners[i];
            Vector3 next = path.corners[i + 1];

            Command c = Command.CreateInstance<Command> ();
            c.name = "Move: " + next.ToString ();
            float d = Vector3.Distance (current, next);
            c.target = target;
            c.time = d / speed;
            c.position = next;
            c.type = Type.Move;
            if (character)
                character.AddCommand (c);

            commands.Add (c);
        }

        if (target) {
            GameObject t = new GameObject ("MTarget");
            t.tag = "CommandPointer";
            t.transform.position = end;
            t.transform.parent = target;
        }

        return commands;
    }

    public static List<Command> MoveCommand (Vector3 start, Vector3 end, float speed, Character character) {
		return MoveCommand (start, end, null, speed, character);
	}

	public static List<Command> KillCommand (Vector3 start, Transform target, float range, float speed, Character character, float dps, float health) {
		if (Vector3.Distance (start, target.position) > range || !character.ObjectVisibleFromHeadbone (target)) {
            List<Command> c = MoveCommand (start, target.position, speed, character);
			character.AddCommand (c);
		}
		Command k = Command.CreateInstance <Command> ();
		k.name = "Kill: " + target.name;
		k.time = dps / health;
		k.position = target.position;
		k.type = Type.Kill;
		k.target = target;
		character.AddCommand (k);

        List<Command> killCommand = new List<Command> ();
        killCommand.Add (k);
        return killCommand;
	}

}
