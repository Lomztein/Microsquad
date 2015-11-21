using UnityEngine;
using System.Collections;

public enum Faction { Player, Corrupt, Neutral, Scavengers }; // Just some factions, subject to change.

public class Unit : MonoBehaviour {

	[Header ("Unit Basics")]
	public Faction faction;
	public string unitName;
	public int health;

	public void SetFaction (Faction f) {
		faction = f;
		gameObject.layer = Game.FactionLayerIndex (faction);
	}
}
