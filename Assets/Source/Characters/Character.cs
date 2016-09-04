using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Unit {

	public enum State { Idle, Moving, Attacking, Fleeing, Aimless };

	[Header ("Basics")]
	public CharacterController character;
    public NavMeshAgent navigationAgent;
	public float speed;

	private Transform pointer;

	[Header ("Stats")]
	public CharacterStats stats;
	private CharacterStats multipliers;
	public float altitude;

	[Header ("Equipment")]
	public CharacterEquipment equipment;
	public Inventory inventory;
	public List<Weapon> activeWeapons;

	[Header ("Commands")]
	public List<Command> commands = new List<Command>();
	private Command currentCommand;
	public Transform target;
	public Vector3 targetPos;
	public State state;
    public NavMeshPath pathToCommand;
    public int pathIndex;

	[Header ("Animation")]
	public Animator animator;

	/*void CombineStatBonuses () {
		multipliers = new CharacterStats (1f, 1f, 1f);

		List<CharacterEquipment.Armor> armor = new List<CharacterEquipment.Armor>();
		armor.Add (equipment.headGear);
		armor.Add (equipment.chestGear);
		armor.Add (equipment.legGear);

		for (int i = 0; i < armor.Count; i++) {
			multipliers.strength *= armor[i].statBonuses.strength;
			multipliers.accuracy *= armor[i].statBonuses.accuracy;
			multipliers.speed *= armor[i].statBonuses.speed;
		}
	}*/

	public void Awake () {

		animator.StartPlayback ();
		animator.speed = 1f;

        InitializeEquipment ();

		GameObject p = new GameObject ("Pointer");
		pointer = p.transform;
		pointer.transform.position = transform.position;
		pointer.parent = transform;

        navigationAgent = GetComponent<NavMeshAgent> ();
	}

    private void InitializeEquipment () {
        foreach (CharacterEquipment.Equipment e in equipment.slots) {
            e.character = this;
            e.Update ();
        }
    }

	public float CalcDPS () {
		float val = 0f;
		for (int i = 0; i < activeWeapons.Count; i++) {
			val += activeWeapons[i].CalcDPS ();
		}
		return val;
	}

	public Vector2 CalcOptics () {
		Vector2 val = Vector2.zero;
		for (int i = 0; i < activeWeapons.Count; i++) {
			val += activeWeapons[i].GetOpticSpeed ();
		}
		return val;
	}

	public Item ChangeEquipment (CharacterEquipment.Slot slotType, CharacterEquipment.Equipment slot, Item newItem) {
		if (slotType != newItem.prefab.slotType)
			return null;

		Item curItem = slot.item;

        slot.item = newItem;
        UpdateItem (slot);

        return curItem;
	}

	float CalcWepWeight () {
		float val = 0f;
		for (int i = 0; i < activeWeapons.Count; i++) {
			val += activeWeapons[i].combinedStats.weight;
		}
		return val;
	}

	void UpdateItem (CharacterEquipment.Equipment slot) {
		slot.Update ();
	}

	void ResetAltitude () {
		Ray ray = new Ray (transform.position, Vector3.down);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, Mathf.Infinity, Game.game.terrainLayer))
		    transform.position = hit.point + Vector3.up * (altitude + 0.1f);
	}

	public virtual void FixedUpdate () {
		SetNextCommand ();
		ResetAltitude ();

		if (Vector3.Distance (transform.position, targetPos) > (speed + 0.1f) * Time.fixedDeltaTime)
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (0f, pointer.eulerAngles.y, 0f), Time.fixedDeltaTime * stats.strength / CalcWepWeight () * 360f);

        pointer.LookAt (targetPos);

		if (target) {

			if (state == State.Attacking) {
				if (ObjectVisibleFromHeadbone (target) && Vector3.Distance (target.position, transform.position) < CalcOptics ().y) {
                    targetPos = target.position;
                    pathIndex = 1;
                    FireWeapons ();
				}else{
                    targetPos = NextPathPosition ();
					MoveTowardsCommand ();
                }
			}


            if (pathToCommand == null && pathToCommand.corners.Length > 0) {

                Vector3 t = new Vector3 (targetPos.x, 0f, targetPos.z);
                Vector3 d = new Vector3 (pathToCommand.corners[pathToCommand.corners.Length - 1].x, 0f, pathToCommand.corners[pathToCommand.corners.Length - 1].z);

                if (Vector3.Distance (t, d) > 0.1f) {
                    FindPathToCommand ();
                }
            }

            if (target.tag == "DeadCharacter")
                target = null;

        } else{
			if (state == State.Attacking) {
				CompleteCommand ();
				state = State.Idle;
			}else if (state == State.Idle) {
				targetPos = transform.position;
			}

            MoveTowardsCommand ();
        }
    }

    private void MoveTowardsCommand () {
        if (pathToCommand != null) {
            if (Vector3.Distance (transform.position, NextPathPosition ()) > (speed + 0.1f) * Time.fixedDeltaTime) {
                targetPos = NextPathPosition ();
                MoveTowardsPosition (targetPos);
            } else {
                pathIndex++;
                if (pathIndex == pathToCommand.corners.Length)
                    CompleteCommand ();
            }
        }
    }

    private Vector3 NextPathPosition () {

        if (pathToCommand != null && pathToCommand.corners.Length > 0) {
            return pathToCommand.corners[pathIndex];
        }else {
            if (commands.Count == 0) {
                return targetPos;
            }else {
                return commands[0].GetPosition ();
            }
        }
    }

    private void ClearPathfindingAgent () {
        if (pathToCommand.corners.Length != 0)
            pathToCommand.ClearCorners ();
    }

	public bool ObjectVisibleFromHeadbone (Transform other) {
		Transform head = FindSlotByType (CharacterEquipment.Slot.Head).transform;
		Ray ray = new Ray (head.position, (other.position + Vector3.up * 1f) - head.position);
		RaycastHit hit;

        Debug.DrawLine (head.position, other.position);
        float distance = Vector3.Distance (head.position, other.position);

		if (Physics.SphereCast (ray, 0.15f, out hit, distance, Game.game.terrainLayer))
			return false;

		return true;
	}

    private void FindPathToCommand () {
        pathToCommand = new NavMeshPath ();
        navigationAgent.CalculatePath (commands[0].position, pathToCommand);
        pathIndex = 0;
    }

	void MoveTowardsPosition (Vector3 position) {
		Vector3 dir = (position - transform.position).normalized;
		character.Move (dir * speed * Time.deltaTime);
	}

	void FireWeapons () {
		for (int i = 0; i < activeWeapons.Count; i++)
			// Lol this actually worked.
			if (transform.rotation == Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (0f, pointer.eulerAngles.y, 0f), 5))
				activeWeapons[i].Fire (faction, target);
	}

	public void CompleteCommand () {
		commands.Remove (currentCommand);
		Destroy (currentCommand);
		currentCommand = null;
		animator.SetFloat ("Speed", 0);
        ClearPathfindingAgent ();
        state = State.Idle;
	}

    public void AddCommand ( Command command ) {
        commands.Add (command);
    }

    public void AddCommand (List<Command> command) {
        for (int i = 0; i < command.Count; i++)
		    commands.Add (command[i]);
	}

	public void ClearCommands (Command command = null) {
		StopAllCoroutines ();
		for (int i = 0; i < commands.Count; i++)
			Destroy (commands [i]);
		commands.Clear ();
		if (command != null)
			AddCommand (command);
	}

    public CharacterEquipment.Equipment FindSlotByName (string slotName) {
        foreach (CharacterEquipment.Equipment e in equipment.slots) {
            if (e.name == slotName)
                return e;
        }

        return null;
    }

    public CharacterEquipment.Equipment FindSlotByType (CharacterEquipment.Slot slotType) {
        foreach (CharacterEquipment.Equipment e in equipment.slots) {
            if (e.slot == slotType)
                return e;
        }

        return null;
    }

	void EquipWeapon () {
	}

	void SetNextCommand () {
		if (commands.Count == 0 || currentCommand != null)
			return;

		currentCommand = commands [0];
		if (currentCommand.type == Command.Type.Move) {
			DoMoveCommand ();
		}
		if (currentCommand.type == Command.Type.Kill) {
			DoKillCommand ();
		}
	}

	void DoMoveCommand () {
	    targetPos = currentCommand.position;
		target = null;
		state = State.Moving;
        FindPathToCommand ();
	}

	void DoKillCommand () {
		target = currentCommand.target;
	    state = State.Attacking;
        FindPathToCommand ();
    }

    public void OnEquip (CharacterEquipment.Equipment slot, GameObject equipment) {
		Weapon wep = equipment.GetComponentInChildren<Weapon>();
		if (wep) {
			activeWeapons.Add (wep);
			animator.SetInteger ("WeaponType", 1);
		}
	}

    void DropLooseEquipment () {
        foreach (CharacterEquipment.Equipment e in equipment.slots) {
            if (e.dropOnDeath) {
                e.Drop ();
            }
        }
    }

	private bool isDead = false;
	void OnTakeDamage (Damage d) {
		health -= d.damage;
		if (health <= 0 && !isDead) {
            Die (d);
		}
	}

    private void Die (Damage d) {
        RagdollHandler rag = GetComponent<RagdollHandler> ();
        rag.Ragdoll ();
        rag.OnTakeDamage (d);
        isDead = true;

        Component[] components = GetComponents<Component> ();
        for (int i = 0; i < components.Length; i++) {
            if ((components[i] as Transform) == null)
                Destroy (components[i]);
        }

        if (faction == Faction.Player)
            Game.AddMessage ("Oh my god, they killed " + unitName + "!");

        BroadcastMessage ("OnDeath", SendMessageOptions.DontRequireReceiver);
        DropLooseEquipment ();

        int layer = LayerMask.NameToLayer ("DeadCharacter");
        Collider[] colliders = GetComponentsInChildren<Collider> ();
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].gameObject.tag = "DeadCharacter";
            colliders[i].gameObject.layer = layer;
        }
    }

    void OnDrawGizmos () {
        if (commands.Count > 0) Gizmos.DrawLine (transform.position, commands[0].position);
        if (pathToCommand != null) {
            for (int i = 0; i < pathToCommand.corners.Length - 1; i++) {
                Gizmos.DrawLine (pathToCommand.corners[i], pathToCommand.corners[i + 1]);
            }
        }
    }
}

[System.Serializable]
public struct CharacterStats {
	
	public float strength;
	public float accuracy;
	public float speed;

	public CharacterStats (float _strength = 1f, float _accuracy = 1f, float _speed = 1f) {
		strength = _strength;
		accuracy = _accuracy;
		speed = _speed;
	}
	
}

[System.Serializable]
public class CharacterEquipment {

	public enum Slot { Hand, Head, Chest, Legs, None };
    public Equipment[] slots;

	[System.Serializable]
	public class Equipment {

        public string name;
		public Slot slot;
		public Item item;
		public GameObject physicalItem;
		public Transform transform;
        public bool dropOnDeath;

		public Character character;

        public virtual void Update () {
            if (physicalItem)
                Object.Destroy (physicalItem);

            if (item) {
                GameObject newTool = (GameObject)Object.Instantiate (item.prefab.gameObject, transform.position, transform.rotation);

                newTool.transform.position = transform.position;
                newTool.transform.rotation = transform.rotation;
                newTool.transform.parent = transform;
                    
                newTool.SendMessage ("OnEquip", new EquipMessage (character, item.metadata, this));
                character.OnEquip (this, newTool);
            }
        }

        public PhysicalItem Drop () {
            //PhysicalItem pItem = PhysicalItem.Create (item, transform.position, transform.rotation);

            item = null;
            Update ();

            //return pItem;
            return null;
        }

        public struct EquipMessage {

            public Character character;
            public string metadata;
            public Equipment slot;

            public EquipMessage (Character ch, string me, Equipment sl) {
                character = ch;
                metadata = me;
                slot = sl;
            }

        }
    }
}