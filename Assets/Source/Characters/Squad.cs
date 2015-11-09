﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad : MonoBehaviour {

	public Inventory sharedInventory;
	public List<Squadmember> members;

	public void AddMember (Squadmember member) {
		members.Add (member);
		member.squad = this;
	}

	public void RemoveMember (Squadmember member) {
		members.Remove (member);
		member.squad = null;
	}

}
