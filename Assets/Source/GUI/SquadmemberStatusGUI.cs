﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SquadmemberStatusGUI : MonoBehaviour {

    public Squadmember squadmember;

    public Text squadmemberName;
    public Slider squadmemberHealth;
    public Text squadmemberArmor;
    public RawImage squadmemberWeapon;
    public Text ammoText;

    public void UpdateAll () {
        UpdateStatus ();
        UpdateWeapon ();
        UpdateAmmo ();
    }

    public void Initialize () {
        UpdateAll ();

        Button button = GetComponentInChildren<Button> ();
        button.onClick.AddListener (() => OnButtonClicked ());
    }

    public void OnButtonClicked () {
        if (PlayerInput.itemInHand.item) {
            Item item = PlayerInput.itemInHand.item;
            IConsumeable consumeable = item.prefab as IConsumeable;
            if (consumeable != null) {
                consumeable.Consume (squadmember);
                PlayerInput.itemInHand.ChangeCount (-1);
            }
        }else {
            squadmember.ChangeSelection (true);
        }
    }

	public void UpdateStatus () {
        squadmemberName.text = squadmember.unitName;
        squadmemberHealth.value = (float)squadmember.health / squadmember.maxHealth;
        squadmemberArmor.text = "Armor:\n" + squadmember.CalcArmor ().ToString ();
    }

    public void UpdateWeapon () {
        CharacterEquipment.Slot weaponSlot = squadmember.FindSlotByType (CharacterEquipment.Type.Hand);
        if (weaponSlot.slot.item) {
            squadmemberWeapon.gameObject.SetActive (true);
            squadmemberWeapon.texture = weaponSlot.slot.item.GetIcon ();
        }else {
            squadmemberWeapon.gameObject.SetActive (false);
        }
    }

    public void UpdateAmmo () {
        if (squadmember.activeWeapon) {
            int count = squadmember.activeWeapon.Slot.count;

            ammoText.text = count + "\n/" + squadmember.activeWeapon.body.magazine.maxAmmo;
        } else {
            ammoText.text = "";
        }
    }
}
