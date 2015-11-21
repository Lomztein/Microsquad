﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {

	public static Vector3 worldMousePos;
	public static Vector3 screenMousePos;
	public static PlayerInput cur;

	[Header ("Camera Controls")]
	private Camera camera;
	public float sensitivity = 5f;
	public float zoom;
	public Vector2 cameraHeight;
	public Vector2 cameraAngle;

	[Header ("Unit Control")]
	public static List<Squadmember> selectedUnits = new List<Squadmember>();

	private float timeSinceLastClick;
	public float doubleClickTime;

	private Vector3 mouseDragStart;
	public Bounds selector;

	[Header ("Tactical Pause")]
	public static bool isTacticallyPaused;
	public float slowdownTime;
	public float defaultFixedDeltaTime = 0.02f;

	// Use this for initialization
	void Start () {
		camera = Camera.main;
		cur = this;
	}

	void DeselectAllUnits () {
		for (int i = 0; i < selectedUnits.Count; i++) {
			selectedUnits[i].ChangeSelection (false);
		}
	}

	IEnumerator ToggleTacticalPause (bool enable) {
		if (enable) {
			while (Time.timeScale > 0.1f) {
				Time.timeScale -= 1f/slowdownTime * Time.unscaledDeltaTime;
				Time.timeScale = Mathf.Clamp01 (Time.timeScale);
				Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
				yield return new WaitForEndOfFrame ();
			}
			Time.timeScale = 0f;
		}else{
			while (Time.timeScale < 0.1f) {
				Time.timeScale += 1f/slowdownTime * Time.unscaledDeltaTime;
				Time.timeScale = Mathf.Clamp01 (Time.timeScale);
				Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
				yield return new WaitForEndOfFrame ();
			}
			Time.timeScale = 1f;
		}
		isTacticallyPaused = enable;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("Jump"))
			StartCoroutine (ToggleTacticalPause (!isTacticallyPaused));

		screenMousePos = Input.mousePosition;
		timeSinceLastClick += Time.unscaledDeltaTime;

		zoom += Input.GetAxis ("Mouse ScrollWheel");
		zoom = Mathf.Clamp01 (zoom);

		camera.transform.position = 
			Vector3.Lerp (camera.transform.position, 
			              Vector3.Lerp (new Vector3 (camera.transform.position.x, cameraHeight.y, camera.transform.position.z),
			              				new Vector3 (camera.transform.position.x, cameraHeight.x, camera.transform.position.z), zoom), 
			              20f * Time.unscaledDeltaTime);

		camera.transform.eulerAngles = 
			Vector3.Slerp (camera.transform.eulerAngles, 
			               Vector3.Slerp (new Vector3 (cameraAngle.y, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z),
			               				  new Vector3 (cameraAngle.x, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z), zoom), 
			               20f * Time.unscaledDeltaTime);

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, Game.game.terrainLayer + Game.game.all)) {
			worldMousePos = hit.point;
			WorldCursor.cur.transform.position = worldMousePos;

			if (Input.GetMouseButtonDown (0)) {
				if (!Input.GetButton ("Shift"))
					DeselectAllUnits ();

				mouseDragStart = worldMousePos;
			}

			if (Input.GetMouseButton (0)) {

				Vector3 middle = mouseDragStart + (worldMousePos - mouseDragStart) / 2f;
				selector.center = new Vector3 (middle.x, 0f, middle.z);
				selector.size = mouseDragStart - worldMousePos;
				selector.size = new Vector3 (selector.size.x, 2f, selector.size.z);
			}
			
			Unit unit = hit.collider.GetComponent<Unit>();
			if (unit) {
				WorldCursor.cur.transform.position = hit.collider.transform.position;
				if (unit.faction == Faction.Player) {
					WorldCursor.SetCursor (WorldCursor.CursorType.Select);
				}else if (unit.faction != Faction.Player) {
					WorldCursor.SetCursor (WorldCursor.CursorType.Attack);
					WorldCursor.ForceMaterial (1, Game.FactionMaterial (unit.faction));
				}
				
				if (Input.GetMouseButtonUp (0)) {

					unit.SendMessage ("ChangeSelection", true, SendMessageOptions.DontRequireReceiver);
					if (timeSinceLastClick < doubleClickTime) {
						for (int i = 0; i < Squad.activeSquad.members.Count; i++) {
							Squad.activeSquad.members[i].ChangeSelection (true);
						}
					}

					timeSinceLastClick = 0f;
				}
			}else{
				WorldCursor.SetCursor (WorldCursor.CursorType.Move);
				WorldCursor.cur.transform.position = hit.point;
			}
			
			if (Input.GetMouseButtonDown (1)) {
				OrderUnits (hit, unit);
			}
		}

		Vector3 movement = Vector3.zero;
		if (screenMousePos.x < 5f) {
			movement += Vector3.left;
		} else if (screenMousePos.x > Screen.width - 5f) {
			movement += Vector3.right;
		}

		if (screenMousePos.y < 5f) {
			movement += Vector3.back;
		} else if (screenMousePos.y > Screen.height - 5f) {
			movement += Vector3.forward;
		}

		transform.position += Quaternion.Euler (0f, camera.transform.eulerAngles.y, 0f) * movement * sensitivity * Time.unscaledDeltaTime;
	}

	public static bool IsInsideSelector (Vector3 position) {
		return PlayerInput.cur.selector.Contains (position);
	}

	void OrderUnits (RaycastHit hit, Unit unit) {
		// Figure out whatever was clicked.
		if (hit.collider.gameObject.layer == Game.game.terrainLayerIndex) {
			Vector3[] positions = Micromanagement.GetSpriralPositions (1.5f, selectedUnits.Count);
			for (int i = 0; i < selectedUnits.Count; i++) {

				Squadmember member = selectedUnits[i];
				Vector3 startPos = member.transform.position;
				bool shiftPressed = Input.GetButton ("Shift");

				if (member.commands.Count > 0 && shiftPressed)
					startPos = member.commands[member.commands.Count - 1].position + Vector3.up;
				
				Vector3 pos = hit.point + positions[i] + Vector3.up;
				Debug.DrawLine (startPos, pos);
				if (!Physics.Linecast (startPos, pos, Game.game.terrainLayer)) {
					if (!shiftPressed)
						member.ClearCommands ();
						
					Command.MoveCommand (startPos, pos, member.speed, member);
				}
			}
		}

		if (unit) {
			for (int i = 0; i < selectedUnits.Count; i++) {
				Squadmember member = selectedUnits[i];

				if (unit.faction != Faction.Player) {
					if (!Input.GetButton ("Shift"))
						member.ClearCommands ();

					Command.MoveCommand (member.transform.position, unit.transform.position, unit.transform, member.speed, member);
				}
			}
		}
	}

	void OnDrawGizmos () {
		Gizmos.DrawWireCube (selector.center, selector.size);
	}
}
