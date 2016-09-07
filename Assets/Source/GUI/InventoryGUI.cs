using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryGUI : MonoBehaviour {

    public static GameObject buttonPrefab;

    public Inventory inventory;
    public InventoryButton[] buttons;

    void Awake () {
        buttonPrefab = Resources.Load<GameObject> ("GUI/InventoryButton");
    }

	public virtual void OnButtonClicked (int buttonIndex) {
        inventory.slots[buttonIndex].MoveItem (PlayerInput.itemInHand);
        buttons[buttonIndex].UpdateButton ();
        PlayerInput.UpdateItemInHand ();
    }

    public virtual void CreateButtons (Transform buttonParent) {
        buttons = new InventoryButton[inventory.size];

        for (int i = 0; i < inventory.size; i++) {
            GameObject newButton = Instantiate (buttonPrefab);
            newButton.transform.SetParent (buttonParent);

            InventoryButton button = newButton.GetComponent<InventoryButton> ();
            button.Init (inventory.slots[i]);
            button.UpdateButton ();
            buttons[i] = button;
            AddButtonListener (button, i);
        }
    }

    void AddButtonListener (Button button, int index) {
        button.onClick.AddListener (() => OnButtonClicked (index));
    }
}
