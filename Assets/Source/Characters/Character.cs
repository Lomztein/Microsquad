using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Unit {

    // TODO: Move all commmand related stuff into a seperate "BasicCharacter" class.
	public enum State { Idle, Moving, Attacking, Fleeing, Aimless, Interacting };

	[Header ("Basics")]
	public CharacterController character;
    public NavMeshAgent navigationAgent;
	public float speed;

	private Transform pointer;

	[Header ("Stats")]
	public CharacterStats stats;
	private CharacterStats multipliers;
	public float altitude;

    public bool alert;

	[Header ("Equipment")]
	public CharacterEquipment equipment;
	public Inventory inventory;
	public Weapon activeWeapon;
    public CharacterEquipment.Equipment toolSlot;

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
            e.item = Inventory.Slot.CreateSlot ();
            e.character = this;
            e.Update ();
        }

        toolSlot = FindSlotByType (CharacterEquipment.Slot.Hand);
    }

	public float CalcDPS () {
        if (activeWeapon)
            return activeWeapon.CalcDPS ();
        return 0;
	}

	public Vector2 CalcOptics () {
        if (activeWeapon)
            return activeWeapon.GetOpticSpeed ();
        return Vector2.zero;
    }

    public void ChangeEquipment (CharacterEquipment.Slot slotType, CharacterEquipment.Equipment slot, Inventory.Slot newSlot) {
		if (newSlot.item && slotType != newSlot.item.prefab.slotType)
			return;

        // Handle specifics when swapping ammunition.
        if (slotType == CharacterEquipment.Slot.Ammo) {
            AmmoPrefab.AmmoType ammoType = newSlot.item.attributes.GetAttribute<AmmoPrefab.AmmoType> ("AmmoType");
            Debug.Log (ammoType);

            if (activeWeapon) {
                if (activeWeapon.body.ammoType == ammoType) {
                    newSlot.MoveItem (slot.item, activeWeapon.body.magazine.maxAmmo, true);
                }
                UpdateAmmunition ();
            }

            UpdateItem (slot);
            return;
        }

        newSlot.MoveItem (slot.item);
        UpdateItem (slot);
	}

	float CalcWepWeight () {
        if (activeWeapon)
            return activeWeapon.combinedStats.weight;
        return 0;
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

        // TODO: Change activeWeapons to a single weapon or tool reference.
        if (activeWeapon) {
            Transform tran = activeWeapon.transform;
            tran.position = Vector3.Lerp (tran.position, toolSlot.transform.position, stats.recoilRecovery * Time.fixedDeltaTime);
            tran.rotation = Quaternion.Slerp (tran.rotation, toolSlot.transform.rotation, stats.recoilRecovery / 5f * Time.fixedDeltaTime);
        }

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


            if (pathToCommand != null && pathToCommand.corners.Length > 0) {

                Vector3 t = new Vector3 (target.position.x, 0f, target.position.z);
                Vector3 d = new Vector3 (pathToCommand.corners[pathToCommand.corners.Length - 1].x, 0f, pathToCommand.corners[pathToCommand.corners.Length - 1].z);

                if (Vector3.Distance (t, d) > 0.1f) {
                    commands[0].position = target.position;
                    FindPathToCommand ();
                }
            }

            if (state == State.Interacting) {
                MoveTowardsCommand ();
                if (Vector3.Distance (target.position, transform.position) < 3f) {
                    target.SendMessage (currentCommand.metadata);
                    CompleteCommand ();
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
        pathIndex = 1;
    }

	void MoveTowardsPosition (Vector3 position) {
		Vector3 dir = (position - transform.position).normalized;
		character.Move (dir * speed * Time.deltaTime);
	}

	void FireWeapons () {
		// Lol this actually worked.
        if (activeWeapon)
		    if (transform.rotation == Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (0f, pointer.eulerAngles.y, 0f), 5))
                activeWeapon.Fire (faction, target);
	}

    public void WeaponRecoil (Transform weapon, float recoil) {
        recoil /= stats.strength;
        weapon.position -= weapon.forward * recoil;
        weapon.eulerAngles += new Vector3 (recoil, 0.25f * Random.Range (-recoil * 0.25f, recoil * 0.25f), 0f);
    }

	public void CompleteCommand () {
		commands.Remove (currentCommand);
		Destroy (currentCommand);
		currentCommand = null;
		animator.SetFloat ("Speed", 0);
        ClearPathfindingAgent ();
        state = State.Idle;
	}

    public void CMInspect () {
        CharacterInspectorGUI.InspectCharacter (this, new Vector2 (Screen.width / 2f, Screen.height / 2f + 100));
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
            Game.AddMessage (unitName + " moves to " + currentCommand.position.ToString ());
		}
		if (currentCommand.type == Command.Type.Kill) {
			DoKillCommand ();
            Game.AddMessage (unitName + " goes for the kill.");
        }
        if (currentCommand.type == Command.Type.Interact) {
            DoInteractCommand ();
            Game.AddMessage (unitName + " goes to interact.");
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
        targetPos = target.position;
	    state = State.Attacking;
        FindPathToCommand ();
    }

    void DoInteractCommand () {
        // Because DoKillCommand is so similar, we just call that and change state to "Interacting".
        DoKillCommand ();
        state = State.Interacting;
    }

    public void OnEquip (CharacterEquipment.Equipment slot, GameObject equipment) {
        UpdatePose ();
        Weapon wep = equipment.GetComponentInChildren<Weapon> ();
        if (wep)
            activeWeapon = wep;
    }

    public void OnDeEquip () {
        UpdatePose ();
    }

    void UpdatePose () {
        if (isDead)
            return;

        GameObject tool = FindSlotByType (CharacterEquipment.Slot.Hand).equippedItem;
        Weapon wep = tool.GetComponentInChildren<Weapon> ();
        if (wep) {
            int type = 0;
            switch (wep.body.animationType) {
                case WeaponBody.AnimType.Pistol:
                    type = 2;
                    break;

                case WeaponBody.AnimType.Rifle:
                    type = 3;
                    break;

                default:
                    type = 0;
                    break;
            }

            animator.SetInteger ("ToolType", type);
        } else {
            EquippedItem i = tool.GetComponent<EquippedItem> ();
            if (i) {
                if (i.type == EquippedItem.Type.Tool) {
                    animator.SetInteger ("ToolType", 1);
                }
            }else
                animator.SetInteger ("ToolType", 0);
        }
    }

    void DropLooseEquipment () {
        foreach (CharacterEquipment.Equipment e in equipment.slots) {
            if (e.item.item && e.dropOnDeath) {
                Game.game.StartCoroutine (e.Drop (0.1f));
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

    public void UpdateAmmunition () {
        if (activeWeapon)
            activeWeapon.UpdateAmmunition ();
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

        int layer = LayerMask.NameToLayer ("Ragdoll");
        Collider[] colliders = GetComponentsInChildren<Collider> ();
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].gameObject.tag = "DeadCharacter";
            colliders[i].gameObject.layer = layer;
        }
    }

    public Inventory.Slot FindAmmoByType (AmmoPrefab.AmmoType type) {
        return inventory.FindItemByType (ItemPrefab.Type.Ammunition);
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
    public float recoilRecovery;

    public CharacterStats (float _strength = 1f, float _accuracy = 1f, float _speed = 1f, float _recoilRecovery = 15f) {
		strength = _strength;
		accuracy = _accuracy;
		speed = _speed;
        recoilRecovery = _recoilRecovery;
	}
	
}

[System.Serializable]
public class CharacterEquipment {

	public enum Slot { Hand, Head, Chest, Legs, Ammo, None };
    public enum InspectorSide { Left, Right }
    public Equipment[] slots;

	[System.Serializable]
	public class Equipment {

        public string name;
		public Slot slot;
		public Inventory.Slot item;
		public GameObject equippedItem;
		public Transform transform;

        public bool dropOnDeath;
        public bool spawnOnEquip;

        public InspectorSide side;
        public Texture defualtSlotImage;

		public Character character;

        public virtual void Update () {
            if (equippedItem) {
                equippedItem.SendMessage ("OnUnEquip", new EquipMessage (character, "", this), SendMessageOptions.DontRequireReceiver);
                character.OnDeEquip ();
                Object.Destroy (equippedItem);
            }

            if (spawnOnEquip && item && item.item) {
                GameObject newTool = (GameObject)Object.Instantiate (item.item.prefab.gameObject, transform.position, transform.rotation);

                newTool.transform.position = transform.position;
                newTool.transform.rotation = transform.rotation;
                newTool.transform.parent = transform;

                equippedItem = newTool;
                    
                newTool.SendMessage ("OnEquip", new EquipMessage (character, item.item.metadata, this));
                character.OnEquip (this, newTool);
            }
        }

        public IEnumerator Drop (float waitTime) {
            yield return new WaitForSeconds (waitTime);

            Rigidbody body = transform.GetComponentInParent<Rigidbody> ();
            GameObject pItem = PhysicalItem.Create (item.item, 1, transform.position, transform.rotation).gameObject;
            Rigidbody drop = pItem.GetComponent<Rigidbody> ();

            drop.velocity = body.velocity;
            drop.angularVelocity = body.angularVelocity;

            item = null;
            Update ();
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