using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterInspectorButton : MonoBehaviour {

	public CharacterInspectorGUI inspector;
	public Texture defaultIcon;

	public Button button;
    public RawImage image;
    public Text text;

    public CharacterEquipment.Slot equipment;
    public HoverContextElement element;

	// Use this for initialization
	void Start () {
		if (inspector.character.faction != Faction.Player) {
			button.interactable = false;
		}

        UpdateButton ();
    }

    public void OnClick () {
        inspector.character.ChangeEquipment (equipment.type, equipment, PlayerInput.itemInHand);
        UpdateButton ();
	}

    void UpdateButton () {
        text.text = "";

        if (equipment.slot.item) {
            image.texture = equipment.slot.item.GetIcon ();
            if (equipment.slot.count > 1) {
                text.text = equipment.slot.count.ToString ();
            }
            element.text = equipment.name.ToString () + " - " + equipment.slot.ToString ();
        } else {
            image.texture = defaultIcon;
            element.text = equipment.name.ToString ();
        }
        PlayerInput.UpdateItemInHand ();
    }
}
