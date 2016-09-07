using UnityEngine;
using System.Collections;

public class SquadInventoryGUI : InventoryGUI {

    public Transform buttonParent;

	// Use this for initialization
	void Start () {
        CreateButtons (buttonParent);
	}
}
