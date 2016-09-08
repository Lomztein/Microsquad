using UnityEngine;
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

	[Header ("Tactical Slowdown")]
	public static bool isTacticallySlowed;
	public float slowdownTime;
	public static float defaultFixedDeltaTime = 0.02f;
	public float slowdownFactor = 0.1f;

    [Header ("Items")]
    public static Inventory.Slot itemInHand;
    public GameObject representativeObject;
    public LayerMask physicalItemMask;

    // Use this for initialization
    void Awake () {
		camera = Camera.main;
		cur = this;
		defaultFixedDeltaTime = Time.fixedDeltaTime;
        itemInHand = Inventory.Slot.CreateSlot ();
	}

	void DeselectAllUnits () {
		for (int i = 0; i < selectedUnits.Count; i++) {
			selectedUnits[i].ChangeSelection (false);
		}
	}

	IEnumerator ToggleTacticalPause (bool enable) {
        float modifier = 1f / slowdownTime * Time.unscaledDeltaTime;

        if (enable) {
			while (Time.timeScale - modifier > slowdownFactor) {

                Time.timeScale = Mathf.Max (slowdownFactor, Time.timeScale - modifier);
				Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

				yield return new WaitForFixedUpdate ();
			}
			Time.timeScale = slowdownFactor;
		}else{
			while (Time.timeScale + modifier < 1 - slowdownFactor) {

                Time.timeScale = Mathf.Min (1, Time.timeScale + modifier);
				Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
				yield return new WaitForFixedUpdate ();
			}
			Time.timeScale = 1f;
		}
		Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
		isTacticallySlowed = enable;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("Jump")) {
			StartCoroutine (ToggleTacticalPause (!isTacticallySlowed));
		}

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
		
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, Game.game.groundLayer + Game.game.all)) {
			worldMousePos = hit.point;
			WorldCursor.cur.transform.position = worldMousePos;
            representativeObject.transform.position = worldMousePos + Vector3.up + Vector3.up * GetInHandHeight ();
            representativeObject.transform.rotation = Quaternion.Euler (new Vector3 (0f, representativeObject.transform.eulerAngles.y + 60f * Time.deltaTime, 0f));

			if (Input.GetMouseButtonDown (0)) {
				if (!Input.GetButton ("Shift"))
					DeselectAllUnits ();

				mouseDragStart = worldMousePos;
                PickUpItem (worldMousePos);
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

                ContextMenuElement element = hit.collider.GetComponentInParent<ContextMenuElement> ();
                if (element) {
                    ContextMenu.Open (element.elements, element);
                }
            }
		}

		Vector3 movement = Vector3.zero;
		if (screenMousePos.x < 5f) {
			movement += Vector3.left;
		} else if (screenMousePos.x > Screen.width - 15f) {
			movement += Vector3.right;
		}

		if (screenMousePos.y < 5f) {
			movement += Vector3.back;
		} else if (screenMousePos.y > Screen.height - 15f) {
			movement += Vector3.forward;
		}

        float hor = Input.GetAxis ("Horizontal");
        float ver = Input.GetAxis ("Vertical");
        movement += new Vector3 (hor, 0, ver);

        transform.position += Quaternion.Euler (0f, camera.transform.eulerAngles.y, 0f) * movement * sensitivity * Time.unscaledDeltaTime;
	}

    private float GetInHandHeight () {
        return Mathf.Sin (Time.time / 0.5f) * 0.2f;
    }

	public static bool IsInsideSelector (Vector3 position) {
		return PlayerInput.cur.selector.Contains (position);
	}

    private Vector3 GetCommandStartPos (Squadmember member) {
        Vector3 startPos = member.transform.position;
        if (member.commands.Count > 0 && Input.GetButton ("Shift"))
            startPos = member.commands[member.commands.Count - 1].position + Vector3.up;

        return startPos;
    }

	void OrderUnits (RaycastHit hit, Unit unit) {
        // Figure out whatever was clicked.

        if (hit.collider.gameObject.layer == Game.game.groundLayerIndex) {
			Vector3[] positions = Micromanagement.GetSpriralPositions (1.5f, selectedUnits.Count);
			for (int i = 0; i < selectedUnits.Count; i++) {

				Squadmember member = selectedUnits[i];
				if (member) {
                    Vector3 startPos = GetCommandStartPos (member);
					
					Vector3 pos = hit.point + positions[i] + Vector3.up;
					Debug.DrawLine (startPos, pos);
					if (!Input.GetButton ("Shift"))
					    member.ClearCommands ();
							
					Command.MoveCommand (startPos, pos, member.speed, member);
				}
			}
		}

		if (unit) {
			for (int i = 0; i < selectedUnits.Count; i++) {
				Squadmember member = selectedUnits[i];
                Vector3 startPos = GetCommandStartPos (member);

                if (unit.faction != Faction.Player) {
					if (!Input.GetButton ("Shift"))
						member.ClearCommands ();

					Command.KillCommand (startPos, unit.transform, member.CalcOptics ().y, member.speed, member, member.CalcDPS (), unit.health);
				}
			}
		}
	}

    public void PickUpItem (Vector3 position) {
        Collider[] col = Physics.OverlapSphere (position, 0.25f);

        float dist = float.MaxValue;
        GameObject closest = null;

        for (int i = 0; i < col.Length; i++) {
            float  locDist = Vector3.Distance (col[i].transform.position, position);
            if (locDist < dist) {
                locDist = dist;
                closest = col[i].gameObject;
            }
        }

        if (itemInHand.item) {
            PhysicalItem.Create (itemInHand.item, itemInHand.count, position + Vector3.up * (1 + GetInHandHeight ()), representativeObject.transform.rotation);
            itemInHand.item = null;
            itemInHand.count = 0;
        }

        PhysicalItem pItem = closest.GetComponent<PhysicalItem> ();

        if (pItem) {
            pItem.singleSlot.MoveItem (itemInHand);
            Destroy (closest);
        }

        UpdateItemInHand ();
    }

    public static void UpdateItemInHand () {
        MeshFilter filter = cur.representativeObject.GetComponent<MeshFilter> ();
        if (itemInHand.item) {
            cur.representativeObject.SetActive (true);
            filter.mesh = itemInHand.item.GetMesh ();
        } else {
            cur.representativeObject.SetActive (false);
        }
    }

    void OnDrawGizmos () {
		Gizmos.DrawWireCube (selector.center, selector.size);
	}
}
