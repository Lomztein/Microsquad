using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SquadStatusGUI : MonoBehaviour {

    public GameObject squadmemberStatusPrefab;
    public List<SquadmemberStatusGUI> currentMemberGUIs = new List<SquadmemberStatusGUI>();
    public Text isEmtpyText;

    public static SquadStatusGUI cur;

    void Awake () {
        cur = this;
    }

	public void UpdateSquad () {
        foreach (SquadmemberStatusGUI gui in currentMemberGUIs) {
            Destroy (gui.gameObject);
        }

        currentMemberGUIs = new List<SquadmemberStatusGUI> ();

        Squad squad = Squad.activeSquad;
        for (int i = 0; i < squad.members.Count; i++) {
            Squadmember m = squad.members[i];

            GameObject newGUI = Instantiate (squadmemberStatusPrefab);
            newGUI.transform.SetParent (transform);

            SquadmemberStatusGUI gui = newGUI.GetComponent<SquadmemberStatusGUI> ();
            gui.squadmember = m;
            m.statusGUI = gui;

            gui.Initialize ();

            currentMemberGUIs.Add (gui);
        }

        isEmtpyText.text = squad.members.Count == 0 ? "Your squad is empty" : "Squadmembers";
    }
}
