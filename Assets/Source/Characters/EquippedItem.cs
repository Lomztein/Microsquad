using UnityEngine;
using System.Collections;

public class EquippedItem : MonoBehaviour {

	public enum Type { Tool, Armor, Accessory };
	public Type type;

	public virtual void OnEquip () {
	}
}
