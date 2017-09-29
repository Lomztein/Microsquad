using UnityEngine;
using System.Collections;

public class CapturedSquadmember : Character {

    public GameObject squadmemberPrefab;
    public string[] possibleNames;

    new void Awake () {
        base.Awake ();
        unitName = possibleNames[Random.Range (0, possibleNames.Length)];
    }

    public void CMJoinSquad () {
        if (Squad.activeSquad.members.Count == 0) {
            DialogGUI.RestartCoroutine (RunDialog ());
        } else {
            Micromanagement.OrderInteraction (transform, "Recruit");
        }    
    }

    void Recruit () {
        ChangeToSquadmember ();
    }

    IEnumerator RunDialog () {
		yield return DialogGUI.RunDialog (Game.commanderName, "Wake up you twat.");
        ChangeToSquadmember ();
        Squad.activeSquad.GetComponent<StartingEquipment> ().ApplyInventory ();
        SquadInventoryGUI.cur.ForceUpdateAll ();
    }

    void ChangeToSquadmember () {
        GameObject newMember = (GameObject)Instantiate (squadmemberPrefab, transform.position, transform.rotation);
        Squadmember member = newMember.GetComponent<Squadmember> ();
        member.unitName = unitName;

        Squad.activeSquad.AddMember (member);
        Destroy (gameObject);
    }
}
