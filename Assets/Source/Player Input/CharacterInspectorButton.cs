using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterInspectorButton : MonoBehaviour {

	public CharacterInspector inspector;
	public Sprite defaultIcon;
	public Button button;

	public CharacterEquipment.Slot slot;
	public Item.Type type;

	// Use this for initialization
	void Awake () {
		if (inspector.character.faction != Character.Faction.Player) {
			button.interactable = false;
		}
	}

	public void OnClick () {
		Item i = Game.itemInHand.item;
		if (slot == i.slotType) {
		}
	}
}
