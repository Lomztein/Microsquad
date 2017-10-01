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
    public CharacterEquipment.Slot toolSlot;
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

    public void Awake() {
        if (animator) {
            animator.StartPlayback ();
            animator.speed = 1f;
        }

        InitializeEquipment ();
    }

    private void InitializeEquipment() {
        armorPieces = new List<Armor> ();
        foreach (CharacterEquipment.Slot e in equipment) {
            e.item = Inventory.Slot.CreateSlot ();
            e.character = this;
            e.Update ();
        }

        toolSlot = FindSlotByType (CharacterEquipment.Type.Hand);
    }

    public float CalcDPS() {
        if (activeWeapon)
            return activeWeapon.CalcDPS ();
        return 0;
    }

    public int CalcArmor() {
        int value = 0;
        for (int i = 0; i < armorPieces.Count; i++) {
            value += armorPieces [ i ].armorRating;
        }
        return value;
    }

    public Vector2 CalcOptics() {
        if (activeWeapon)
            return activeWeapon.GetOpticSpeed ();
        return Vector2.zero;
    }

    public void ChangeEquipment(CharacterEquipment.Slot slotType, CharacterEquipment.Slot slot, Inventory.Slot fromSlot) {
        if (fromSlot.item) {
            IEquipable equipable = fromSlot.item.prefab as IEquipable;
            if (equipable != null && slotType != equipable.GetSlotType ())
                return;
        }

        // Handle removing incompatable ammo if new weapon type is placed.
        if (fromSlot.item && slotType == CharacterEquipment.Slot.Hand) {
            CharacterEquipment.Slot ammoSlot = FindSlotByType (CharacterEquipment.Slot.Ammo);

            if (ammoSlot.item.item) {
                IAmmo ammo = ammoSlot.item.item.prefab as IAmmo;
                AmmoPrefab.Type ammoType = ammo.GetAmmoType ();
                SavedWeapon saved = SavedWeapon.LoadFromString (fromSlot.item.metadata);
                AmmoPrefab.Type weaponType = WeaponGenerator.cur.weaponClasses [ saved.classID ].bodies [ saved.bodyID ].GetComponent<WeaponBody> ().ammoType;

                if (weaponType != ammoType) {
                    inventory.PlaceItems (ammoSlot.item);
                }

                // Just to avoid memory being used up or something. Feels right to do this.
                Destroy (saved);
            }
        }

        // Handle specifics when swapping ammunition.
        if (fromSlot.item && slotType == CharacterEquipment.Slot.RAmmo || slotType == CharacterEquipment.Slot.LAmmo) {
            IAmmo ammo = fromSlot.item.prefab as IAmmo;
            AmmoPrefab.Type ammoType = ammo.GetAmmoType ();

            if (activeWeapon) {
                if ((activeWeapon.body.ammoType & ammoType) != 0) {
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

    public float CalcWepWeight() {
        if (activeWeapon)
            return activeWeapon.combinedStats.weight;
        return 0;
    }

    public void WeaponRecoil(Transform weapon, float recoil) {
        recoil /= stats.strength;
        weapon.position -= weapon.forward * recoil;
        weapon.eulerAngles += new Vector3 (recoil, 0.25f * Random.Range (-recoil * 0.25f, recoil * 0.25f), 0f);
    }

    void UpdateItem(CharacterEquipment.Slot slot) {
        slot.Update ();
    }

    void ResetAltitude() {
        Ray ray = new Ray (transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, Mathf.Infinity, Game.game.terrainLayer))
            transform.position = hit.point + Vector3.up * (altitude + 0.1f);
    }

    public virtual void FixedUpdate() {
        ResetAltitude ();
    }

    public bool ObjectVisibleFromHeadbone(Transform other) {
        Transform head = FindSlotByType (CharacterEquipment.Slot.Head).transform;
        Ray ray = new Ray (head.position, (other.position + Vector3.up * 1f) - head.position);
        RaycastHit hit;

        Debug.DrawLine (head.position, other.position);
        float distance = Vector3.Distance (head.position, other.position);

        if (Physics.SphereCast (ray, 0.15f, out hit, distance, Game.game.terrainLayer))
            return false;

        return true;
    }



    public void CMInspect() {
        CharacterInspectorGUI.InspectCharacter (this, new Vector2 (Screen.width / 2f, Screen.height / 2f + 100));
    }

    public CharacterEquipment.Slot FindSlotByName(string slotName) {
        foreach (CharacterEquipment.Slot e in equipment.slots) {
            if (e.name == slotName)
                return e;
        }

        return null;
    }

    public CharacterEquipment.Slot FindSlotByType(CharacterEquipment.Slot slotType) {
        foreach (CharacterEquipment.Slot e in equipment.slots) {
            if (e.slot == slotType)
                return e;
        }

        return null;
    }

    void EquipWeapon() {
    }

    public void OnEquip(CharacterEquipment.Slot slot, GameObject equipment) {
        UpdatePose ();
    }

    public void OnDeEquip() {
        UpdatePose ();
    }

    void UpdatePose() {
        if (isDead)
            return;

        GameObject tool = FindSlotByType (CharacterEquipment.Slot.RHand).equippedItem;
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
            } else
                animator.SetInteger ("ToolType", 0);
        }
    }

    void DropLooseEquipment() {
        foreach (CharacterEquipment.Slot e in equipment.slots) {
            if (e.item.item && e.dropOnDeath) {
                Game.game.StartCoroutine (e.Drop (0.1f));
            }
        }
    }

    private bool isDead = false;
    public void OnTakeDamage(Damage d) {
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
        } else {
            health -= d.damage;
        }
    }

    public void UpdateAmmunition() {
        if (activeWeapon)
            activeWeapon.UpdateAmmunition ();
    }

    private void Die(Damage d) {
        RagdollHandler rag = GetComponent<RagdollHandler> ();
        rag.Ragdoll ();
        rag.OnTakeDamage (d);
        isDead = true;

        Component [ ] components = GetComponents<Component> ();
        for (int i = 0; i < components.Length; i++) {
            if ((components [ i ] as Transform) == null)
                Destroy (components [ i ]);
        }

        if (faction == Faction.Player)
            Game.AddMessage ("Oh my god, they killed " + unitName + "!");

        BroadcastMessage ("OnDeath", SendMessageOptions.DontRequireReceiver);
        DropLooseEquipment ();

        int layer = LayerMask.NameToLayer ("Ragdoll");
        Collider [ ] colliders = GetComponentsInChildren<Collider> ();
        for (int i = 0; i < colliders.Length; i++) {
            colliders [ i ].gameObject.tag = "DeadCharacter";
            colliders [ i ].gameObject.layer = layer;
        }
    }

    public Inventory.Slot FindAmmoByType(AmmoPrefab.Type type) {
        foreach (Inventory.Slot slot in inventory.slots) {
            if (slot.item) {
                IAmmo ammo = slot.item.prefab as IAmmo;
                if (ammo != null) {

                    if ((ammo.GetAmmoType () & type) != 0)
                        return slot;
                }
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

        public CharacterStats(float _strength = 1f, float _accuracy = 1f, float _speed = 1f, float _recoilRecovery = 15f) {
            strength = _strength;
            accuracy = _accuracy;
            speed = _speed;
            recoilRecovery = _recoilRecovery;
        }

    }

