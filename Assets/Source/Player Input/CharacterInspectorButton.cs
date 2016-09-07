using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterInspectorButton : MonoBehaviour {

	public CharacterInspector inspector;
	public Sprite defaultIcon;
	public Button button;

	public CharacterEquipment.Slot slot;
	public ItemPrefab.Type type;

	// Use this for initialization
	void Awake () {
		if (inspector.character.faction != Faction.Player) {
			button.interactable = false;
		}
	}

	public void OnClick () {
		Item i = Game.itemInHand.singleSlot.item;
		if (slot == i.prefab.slotType) {
            if (Game.itemInHand.singleSlot.count != 1)
                Debug.LogWarning ("Tried to place a multicount item in a character equipment slot.");
		}
	}
}
