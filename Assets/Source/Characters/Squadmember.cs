using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squadmember : Character {

	public bool isSelected;
	public Squad squad;

	void OnMouseDown () {
		isSelected = true;
	}

	void Update () {
		if (Input.GetMouseButtonDown (1) && isSelected) {
			Vector3 startPos = transform.position;
			bool shiftPressed = Input.GetButton ("Shift");
			if (commands.Count > 0 && shiftPressed)
				startPos = commands[commands.Count - 1].position;

			Vector3 pos = PlayerInput.worldMousePos + Vector3.up;
			Debug.DrawLine (startPos, pos);
			if (!Physics.Linecast (startPos, pos))
				if (shiftPressed) {
					Command.MoveCommand (startPos, pos, speed, this);
			}else{
				Debug.Log ("wat");
				ClearCommands ();
				Command.MoveCommand (startPos, pos, speed, this);
			}
		}
	}
}