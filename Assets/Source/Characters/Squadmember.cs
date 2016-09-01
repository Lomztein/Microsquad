using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squadmember : Character {

	[Header ("Squadmember")]
	public bool isSelected;
	public Squad squad;
	public GameObject selectedIndicator;

	public void ChangeSelection (bool select) {
		if (select) {
			if (!isSelected)
				PlayerInput.selectedUnits.Add (this);
		}else{
			if (isSelected)
				PlayerInput.selectedUnits.Remove (this);
		}

		isSelected = select;
		selectedIndicator.SetActive (select);
	}

	public override void FixedUpdate () {
		base.FixedUpdate ();
		if (Input.GetMouseButton (0)) {
			if (PlayerInput.IsInsideSelector (new Vector3 (transform.position.x, 0f ,transform.position.z)))
				ChangeSelection (true);
			else if (!Input.GetButton ("Shift"))
				ChangeSelection (false);
		}
	}

    void OnDeath () {
        ChangeSelection (false);
    }

	void OnDestroy () {
		if (isSelected)
			PlayerInput.selectedUnits.Remove (this);
	}
}