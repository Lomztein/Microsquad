using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterInspectorButton : MonoBehaviour {

	public CharacterInspectorGUI inspector;
	public Sprite defaultIcon;

	public Button button;
    public Image image;
    public Text text;

    public CharacterEquipment.Equipment equipment;

	// Use this for initialization
	void Start () {
		if (inspector.character.faction != Faction.Player) {
			button.interactable = false;
		}

        UpdateButton ();
    }

    public void OnClick () {
        inspector.character.ChangeEquipment (equipment.slot, equipment, PlayerInput.itemInHand);
        UpdateButton ();
	}

    void UpdateButton () {
        text.text = "";

        if (equipment.item.item) {
            image.sprite = equipment.item.item.prefab.icon;
            if (equipment.item.count > 1) {
                text.text = equipment.item.count.ToString ();
            }
        } else {
            image.sprite = defaultIcon;
        }
        PlayerInput.UpdateItemInHand ();
    }
}
