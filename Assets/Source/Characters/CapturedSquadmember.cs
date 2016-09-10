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
            if (PlayerInput.selectedUnits.Count > 0) {
                // Find available member, one without any commands.
                Squadmember member = PlayerInput.selectedUnits[0];
                for (int i = 0; i < PlayerInput.selectedUnits.Count; i++) {
                    if (PlayerInput.selectedUnits[i].commands.Count == 0) {
                        member = PlayerInput.selectedUnits[i];
                    }
                }

                Command.InteractCommand (transform, member, "Recruit");
            }
        }    
    }

    void Recruit () {
        ChangeToSquadmember ();
    }

    IEnumerator RunDialog () {
        yield return DialogGUI.RunDialog (Game.commanderName, "Wake up...\nWake up...\n\nWake up goddammit you lazy piece of sausage." );
        yield return DialogGUI.RunDialog (unitName, "Wha-\nWho's that speaking to me through a conveniently unexplained mechanism?", "Also did you just call me a sausage? How dare you." );
        yield return DialogGUI.RunDialog (Game.commanderName, "Fuck if I know, this is just a really early prototype lol");
        yield return DialogGUI.RunDialog (unitName, "Oh okay cool, lets go shoot some guys! :D");
        yield return DialogGUI.RunDialog (Game.commanderName, "Sweet, I'll give you this assault rifle.\nWith this, you can murder your enemies in cold blood and take their weapons.");
        yield return DialogGUI.RunDialog (unitName, "An assault rifle? That's a bit OP for this early, isn't it? Oh well, ain't gonna complain about the good things.");
        ChangeToSquadmember ();
        Squad.activeSquad.GetComponent<StartingEquipment> ().ApplyInventory ();
        SquadInventoryGUI.cur.ForceUpdateAll ();
    }

    void ChangeToSquadmember () {
        GameObject newMember = (GameObject)Instantiate (squadmemberPrefab, transform.position, transform.rotation);
        Squadmember member = newMember.GetComponent<Squadmember> ();
        Squad.activeSquad.AddMember (member);
        Destroy (gameObject);
    }
}
