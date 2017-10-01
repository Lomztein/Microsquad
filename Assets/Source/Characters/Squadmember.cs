using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squadmember : Character {

	[Header ("Squadmember")]
	public bool isSelected;
	public Squad squad;
	public GameObject selectedIndicator;
    public SquadmemberStatusGUI statusGUI;

    new void Awake () {
        base.Awake ();
    }

    void Start () {
        if (!squad) {
            Squad spawnSquad = GetComponentInParent<Squad> ();
            if (spawnSquad)
                spawnSquad.AddMember (this);
        } else {
            squad.AddMember (this);
        }
        statusGUI.UpdateAll ();
    }

    public void ChangeSelection (bool select) {
        if (!squad)
            return;
        
		if (select) {
            if (!isSelected)
                PlayerInput.selectedUnits.Add (this);
        } else {
			if (isSelected) {
                if (ContextMenu.activeMenus.Count != 0)
                    return;
				PlayerInput.selectedUnits.Remove (this);
            }
        }

		isSelected = select;
		selectedIndicator.SetActive (select);
	}

	public override void FixedUpdate () {
		base.FixedUpdate ();
		if (Input.GetMouseButton (0)) {
			if (PlayerInput.IsInsideSelector (new Vector3 (transform.position.x, 0f ,transform.position.z)))
				ChangeSelection (true);
			else if (!Input.GetButton ("Shift") && !HoverContextElement.activeElement)
				ChangeSelection (false);
		}
	}

    void OnFireWeapons () {
        statusGUI.UpdateAmmo ();
    }

    void OnEquipmentChanged () {
        statusGUI.UpdateAll ();
    }

    new void OnTakeDamage (Damage d) {
        base.OnTakeDamage (d);
        statusGUI.UpdateStatus ();
    }

    void OnDeath () {
        ChangeSelection (false);
        squad.RemoveMember (this);
    }

	void OnDestroy () {
		if (isSelected)
			PlayerInput.selectedUnits.Remove (this);
	}
}