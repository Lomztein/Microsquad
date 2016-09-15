using UnityEngine;
using System.Collections.Generic;

public class Command : ScriptableObject {

	public enum Type { Move, Kill, Defend, Interact, Execute }
	public static Color[] colors = new Color[] { Color.green, Color.red, Color.blue };

	public Vector3 position;
	public Transform target;
	public float time;
	public Type type;
    public string metadata;
    public ObjectAttribute attributes = new ObjectAttribute();

	void OnDestroy () {
		if (target && target.tag == "CommandPointer")
			Destroy (target.gameObject);
	}

    public Vector3 GetPosition () {
        if (target)
            return target.position;
        return position;
    }

    // Todo, create a "Basic command", which just fills out the command informtion with basic data.

    public static Command MoveCommand ( Vector3 start, Vector3 end, CharacterAI character ) {

        Command c = Command.CreateInstance<Command> ();
        c.name = "Move: " + end.ToString ();
        float d = Vector3.Distance (start, end);
        c.time = d / character.character.speed;
        c.position = end;
        c.type = Type.Move;
        character.AddCommand (c);

        return c;
    }

	public static Command KillCommand (Transform target, CharacterAI character, float health) {

        Command k = Command.CreateInstance <Command> ();
        k.name = "Kill: " + target.name;
		k.time = character.character.CalcDPS () / health;
		k.position = target.position;
		k.type = Type.Kill;
		k.target = target;
		character.AddCommand (k);
        return k;
    }

    public static Command ExecuteCommand ( Transform target, CharacterAI character, float health ) {

        Command k = Command.CreateInstance<Command> ();
        k.name = "Execute: " + target.name;
        k.time = character.character.CalcDPS () * 5f / health;
        k.position = target.position;
        k.type = Type.Execute;
        k.target = target;
        character.AddCommand (k);
        return k;
    }

    public static Command InteractCommand (Transform target, CharacterAI character, string interactCommand, float interactRange = 5f) {

        Command i = Command.CreateInstance<Command> ();
        i.metadata = interactCommand;

        i.name = "Interact: " + target.name + " with " + interactCommand;
        i.time = Vector3.Distance (character.transform.position, target.position) / character.character.speed;
        i.position = target.position;
        i.type = Type.Interact;
        i.target = target;
        i.attributes.AddAttribute ("InteractRange", interactRange);

        character.AddCommand (i);
        return i;

    }
}