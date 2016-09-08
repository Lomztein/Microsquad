using UnityEngine;
using System.Collections;

public class PhysicalTool : MonoBehaviour {

	public enum Type { Weapon, Toggle };
	public Type type;

	public virtual void OnEquip () {
	}
}
