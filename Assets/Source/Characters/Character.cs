using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Unit {

	public enum State { Idle, Searching, Attacking, Fleeing, Aimless };

	[Header ("Basics")]
	public CharacterController character;
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

	[Header ("Animation")]
	public Animator animator;
	public GameObject ragdoll;

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

	public void Start () {

		animator.StartPlayback ();
		animator.speed = 1f;

		equipment.chestGear.character = this;
		equipment.leftHand.character = this;
		equipment.rightHand.character = this;
		equipment.headGear.character = this;
		equipment.legGear.character = this;

		UpdateItem (equipment.rightHand);
		UpdateItem (equipment.leftHand);
		UpdateItem (equipment.headGear);
		UpdateItem (equipment.chestGear);
		UpdateItem (equipment.legGear);

		GameObject p = new GameObject ("Pointer");
		pointer = p.transform;
		pointer.transform.position = transform.position;
		pointer.parent = transform;
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

	public void ChangeEquipment (CharacterEquipment.Slot slotType, CharacterEquipment.Equipment slot, Item newItem) {
		if (slotType != newItem.slotType)
			return;

		Item curItem = slot.item;
		if ((slot == equipment.rightHand || slot == equipment.leftHand) &&
		    (equipment.rightHand.item == equipment.leftHand.item)) {
			
			equipment.rightHand.item = null;
			equipment.leftHand.item = null;
		}

		if (newItem.type == Item.Type.TwoHandTool) {
			CharacterEquipment.Equipment otherSlot = equipment.rightHand;
			if (slot == equipment.rightHand)
				otherSlot = equipment.leftHand;

			if (otherSlot.item != null && slot.item != null)
				return;

			slot.item = newItem;
			otherSlot.item = newItem;
		} else {
			slot.item = newItem;
			if (curItem)
				PhysicalItem.Create (curItem, transform.position, Quaternion.identity);
		}

		UpdateItem (slot);
	}

	float CalcWepWeight () {
		float val = 0f;
		for (int i = 0; i < activeWeapons.Count; i++) {
			val += activeWeapons[i].combinedStats.weight;
		}
		return val;
	}

	void UpdateItem (CharacterEquipment.Equipment slot) {
		if (slot.item == null)
			return;

		if ((slot == equipment.rightHand || slot == equipment.leftHand) &&
			(equipment.rightHand.item == equipment.leftHand.item) &&
		    slot.item.type == Item.Type.TwoHandTool) {

			if (equipment.rightHand.physicalItem) {
				GameObject.Destroy (equipment.rightHand.physicalItem);
			}

			GameObject newTool = (GameObject)Instantiate (equipment.rightHand.item.gameObject, equipment.rightHand.transform.position, equipment.rightHand.transform.rotation);
			PhysicalTool tool = newTool.GetComponent<PhysicalTool> ();
			equipment.rightHand.physicalItem = tool.gameObject;
			equipment.leftHand.physicalItem = tool.gameObject;
			equipment.rightHand.tool = tool;
			equipment.leftHand.tool = tool;
			tool.transform.transform.parent = transform;
		}else{
			slot.Update ();
		}
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

		if (Vector3.Distance (transform.position, targetPos) > 0.5f)
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (0f, pointer.eulerAngles.y, 0f), CalcOptics ().x * Time.fixedDeltaTime * stats.strength / CalcWepWeight ());
		pointer.LookAt (targetPos);

		if (target) {
			//if (ObjectVisibleFromHeadbone (target))
			    targetPos = target.position;

			if (state == State.Attacking || state == State.Searching) {
				if (Vector3.Distance (target.position, transform.position) < CalcOptics ().y) {
					FireWeapons ();
				}else{
					MoveTowardsPosition (targetPos);
				}
			}
		}else{
			if (state == State.Attacking) {
				CompleteCommand ();
				state = State.Idle;
			}else if (state == State.Idle) {
				targetPos = transform.position;
			}

			if (Vector3.Distance (transform.position, targetPos) > 0.5f) {
				MoveTowardsPosition (targetPos);
			}else{
				CompleteCommand ();
			}
		}
	}

	public bool ObjectVisibleFromHeadbone (Transform other) {
		Transform head = equipment.headGear.transform;
		Ray ray = new Ray (head.position, other.position - head.position);
		RaycastHit hit;

		Physics.Raycast (ray, out hit, ray.direction.magnitude);
		if (hit.collider.transform == other)
			return true;

		return false;
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
	}

	public void AddCommand (Command command) {
		commands.Add (command);
	}

	public void ClearCommands (Command command = null) {
		StopAllCoroutines ();
		for (int i = 0; i < commands.Count; i++)
			Destroy (commands [i]);
		commands.Clear ();
		if (command != null)
			AddCommand (command);
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
		if (currentCommand.target) {
			target = currentCommand.target;
			state = State.Attacking;
		}else{
			targetPos = currentCommand.position;
			target = null;
			state = State.Searching;
		}
	}

	void DoKillCommand () {
		target = currentCommand.target;
	}

	public void OnEquip (CharacterEquipment.Equipment slot, GameObject equipment) {
		Weapon wep = equipment.GetComponentInChildren<Weapon>();
		if (wep) {
			activeWeapons.Add (wep);
			animator.SetInteger ("WeaponType", 1);
		}
	}

	private bool isDead = false;
	void OnTakeDamage (Damage d) {
		health -= d.damage;
		if (health <= 0 && !isDead) {
			Destroy (gameObject);
			GameObject r = (GameObject)Instantiate (ragdoll, transform.position, transform.rotation);
			RagdollHandler rag = r.GetComponent<RagdollHandler>();
			rag.OnTakeDamage (d);
			isDead = true;

			if (faction == Faction.Player)
				Game.AddMessage ("Oh my god, they killed " + unitName + "!");
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
	public CharacterEquipment.Tool rightHand;
	public CharacterEquipment.Tool leftHand;
	public CharacterEquipment.Armor headGear;
	public CharacterEquipment.Armor chestGear;
	public CharacterEquipment.Armor legGear;

	[System.Serializable]
	public class Equipment {

		public Slot slot;
		public Item item;
		public GameObject physicalItem;
		public Transform transform;

		public Character character;

		public virtual void Update () {
		}

	}

	[System.Serializable]
	public class Tool : Equipment {

		public PhysicalTool tool;

		public override void Update () {
			if (physicalItem) {
				GameObject.Destroy (physicalItem);
			}

			GameObject newTool = (GameObject)GameObject.Instantiate (item.gameObject, transform.position, transform.rotation);
			PhysicalTool t = newTool.GetComponent<PhysicalTool> ();
			physicalItem = t.gameObject;
			physicalItem.transform.parent = transform;
			tool = t;

			newTool.SendMessage ("OnEquip", WeaponGenerator.cur.GenerateRandomWeaponData ());
			character.OnEquip (this, newTool);
		}

	}

	[System.Serializable]
	public class Armor : Equipment {

		public enum Position { Head, Chest, Legs };

	}
}