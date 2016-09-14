using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterInspectorButton : MonoBehaviour {

	public CharacterInspectorGUI inspector;
	public Texture defaultIcon;

	public Button button;
    public RawImage image;
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
        Debug.Log ("Updating button!", this);
        text.text = "";

        if (equipment.item.item) {
            image.texture = equipment.item.item.GetIcon ();
            if (equipment.item.count > 1) {
                text.text = equipment.item.count.ToString ();
            }
        } else {
            image.texture = defaultIcon;
        }
        PlayerInput.UpdateItemInHand ();
    }
}
