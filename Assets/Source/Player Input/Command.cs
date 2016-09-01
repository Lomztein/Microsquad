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

    public Vector3 GetPosition () {
        if (target)
            return target.position;
        return position;
    }

    public static Command MoveCommand ( Vector3 start, Vector3 end, float speed, Character character ) {

        Command c = Command.CreateInstance<Command> ();
        c.name = "Move: " + end.ToString ();
        float d = Vector3.Distance (start, end);
        c.time = d / speed;
        c.position = end;
        c.type = Type.Move;
        character.AddCommand (c);

        return c;
    }

	public static Command KillCommand (Vector3 start, Transform target, float range, float speed, Character character, float dps, float health) {
		/*if (Vector3.Distance (start, target.position) > range || !Utility.LineOfSight (start + Vector3.up, target.position + Vector3.up)) {
            MoveCommand (start, target.position, speed, character);
		}

        if (character.commands.Count > 0) {
            Command lastCommand = character.commands[character.commands.Count - 1];
            if (Vector3.Distance (lastCommand.position, target.position) < 0.5f)
                character.commands.RemoveAt (character.commands.Count - 1);
        }*/

        Command k = Command.CreateInstance <Command> ();
        k.name = "Kill: " + target.name;
		k.time = dps / health;
		k.position = target.position;
		k.type = Type.Kill;
		k.target = target;
		character.AddCommand (k);
        return k;
    }

}
