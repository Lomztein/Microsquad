using UnityEngine;
using System.Collections;

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

	public static Command MoveCommand (Vector3 start, Vector3 end, Transform target, float speed, Character character) {
		GameObject t = new GameObject ("MTarget");
		t.tag = "CommandPointer";
		t.transform.position = end;
		t.transform.parent = target;
		Command c = Command.CreateInstance <Command> ();
		c.name = "Move: " + end.ToString ();
		float d = Vector3.Distance (start, end);
		c.target = t.transform;
		c.time = d / speed;
		c.position = end;
		c.type = Type.Move;
		if (character) character.AddCommand (c);
		return c;
	}

	public static Command MoveCommand (Vector3 start, Vector3 end, float speed, Character character) {
		return MoveCommand (start, end, null, speed, character);
	}

	public static Command KillCommand (Vector3 start, Transform target, float range, float speed, Character character, float dps, float health) {
		if (Vector3.Distance (start, target.position) > range) {
			Command c = MoveCommand (start, Utility.ClosestPointToSphere (target.position, range, start), speed, character);
			character.AddCommand (c);
		}
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
