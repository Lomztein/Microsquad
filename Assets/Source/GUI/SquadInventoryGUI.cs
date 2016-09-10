using UnityEngine;
using System.Collections;

public class SquadInventoryGUI : InventoryGUI {

    public Transform buttonParent;
    public static SquadInventoryGUI cur;

	// Use this for initialization
	void Start () {
        CreateButtons (buttonParent);
        cur = this;
	}
}
