using UnityEngine;
using System.Collections;

public class PhysicalItem : MonoBehaviour {

	public Item item;

	public static PhysicalItem Create (Item item, Vector3 position, Quaternion rotation) {
		GameObject newItem = (GameObject)Instantiate (Game.game.physicalItemPrefab, position, rotation);
		newItem.GetComponent<PhysicalItem> ().item = item;
		return newItem.GetComponent<PhysicalItem>();
	}

}