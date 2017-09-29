using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Unit {

    // TODO: Move all commmand related stuff into a seperate "BasicCharacter" class.
	[Header ("Basics")]
	public CharacterController character;
	public float speed;
    public CharacterAI ai;

	[Header ("Stats")]
	public CharacterStats stats;
	private CharacterStats multipliers;
	public float altitude;
    public float damageBlocking;

    public bool alert;

	[Header ("Equipment")]
	public CharacterEquipment equipment;
	public Inventory inventory;
	public Weapon activeWeapon;
    public CharacterEquipment.Equipment toolSlot;
    public List<Armor> armorPieces;

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
	}

    private void InitializeEquipment () {
        armorPieces = new List<Armor> ();
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

    public int CalcArmor () {
        int value = 0;
        for (int i = 0; i < armorPieces.Count; i++) {
            value += armorPieces[i].armorRating;
        }
        return value;
    }

	public Vector2 CalcOptics () {
        if (activeWeapon)
            return activeWeapon.GetOpticSpeed ();
        return Vector2.zero;
    }

    public void ChangeEquipment (CharacterEquipment.Slot slotType, CharacterEquipment.Equipment slot, Inventory.Slot fromSlot) {
		if (fromSlot.item && slotType != fromSlot.item.prefab.slotType)
			return;

        // Handle removing incompatable ammo if new weapon type is placed.
        if (fromSlot.item && slotType == CharacterEquipment.Slot.Hand) {
            CharacterEquipment.Equipment ammoSlot = FindSlotByType (CharacterEquipment.Slot.Ammo);

            if (ammoSlot.item.item) {
                AmmoPrefab.AmmoType ammoType = ammoSlot.item.item.attributes.GetAttribute<AmmoPrefab.AmmoType> ("AmmoType");
                SavedWeapon saved = SavedWeapon.LoadFromString (fromSlot.item.metadata);
                AmmoPrefab.AmmoType weaponType = WeaponGenerator.cur.weaponClasses[saved.classID].bodies[saved.bodyID].GetComponent<WeaponBody> ().ammoType;

                if (weaponType != ammoType) {
                    inventory.PlaceItems (ammoSlot.item);
                }

                // Just to avoid memory being used up or something. Feels right to do this.
                Destroy (saved);
            }
        }

        // Handle specifics when swapping ammunition.
        if (fromSlot.item && slotType == CharacterEquipment.Slot.Ammo) {
            AmmoPrefab.AmmoType ammoType = fromSlot.item.attributes.GetAttribute<AmmoPrefab.AmmoType> ("AmmoType");

            if (activeWeapon) {
                if (activeWeapon.body.ammoType == ammoType) {
                    int space = Mathf.Min (activeWeapon.body.magazine.maxAmmo - slot.item.count, fromSlot.count);

                    if (space > 0) {
                        fromSlot.MoveItem (slot.item, space, false);
                    }
                }
                UpdateAmmunition ();
            }

            slot.item.ForceButtonUpdate ();
            UpdateItem (slot);

            SendMessage ("OnEquipmentChanged", SendMessageOptions.DontRequireReceiver);
            return;
        }

        slot.item.ForceButtonUpdate ();
        fromSlot.MoveItem (slot.item);
        UpdateItem (slot);

        SendMessage ("OnEquipmentChanged", SendMessageOptions.DontRequireReceiver);
    }

    public float CalcWepWeight () {
        if (activeWeapon)
            return activeWeapon.combinedStats.weight;
        return 0;
    }

    public void WeaponRecoil ( Transform weapon, float recoil ) {
        recoil /= stats.strength;
        weapon.position -= weapon.forward * recoil;
        weapon.eulerAngles += new Vector3 (recoil, 0.25f * Random.Range (-recoil * 0.25f, recoil * 0.25f), 0f);
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
		ResetAltitude ();
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

   

    public void CMInspect () {
        CharacterInspectorGUI.InspectCharacter (this, new Vector2 (Screen.width / 2f, Screen.height / 2f + 100));
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

    public void OnEquip (CharacterEquipment.Equipment slot, GameObject equipment) {
        UpdatePose ();
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
	public void OnTakeDamage (Damage d) {
        if (d.damage > 0) {
            // Calculate damage based on armor.
            int damage = d.damage;
            foreach (Armor armor in armorPieces) {
                damage -= armor.armorRating;
            }

		    health -= damage;
		    if (health <= 0 && !isDead) {
                Die (d);
		    }
        }else {
            health -= d.damage;
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
        foreach (Inventory.Slot slot in inventory.slots) {
            if (slot.item && slot.item.prefab.type == ItemPrefab.Type.Ammunition) {

                AmmoPrefab.AmmoType t = slot.item.attributes.GetAttribute<AmmoPrefab.AmmoType> ("AmmoType");

                if (t == type)
                    return slot;
            }
        }

        return null;
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
            GameObject pItem = PhysicalItem.Create (item.item, item.count, transform.position, transform.rotation).gameObject;
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