using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad : MonoBehaviour {

	public Inventory sharedInventory;
	public List<Squadmember> members;
	public static Squad activeSquad;

	void OnEnable () {
		activeSquad = this;
	}

	public void AddMember (Squadmember member) {
		members.Add (member);
		member.squad = this;
	}

	public void RemoveMember (Squadmember member) {
		members.Remove (member);
		member.squad = null;

        if (members.Count == 0) {
            GUIManager.cur.ShowGameOverScreen ();
        }
	}

}
