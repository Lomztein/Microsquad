using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryButton : Button {

    public Inventory.Slot slot;
    public Image itemImage;
    public Text countText;

    public void Init (Inventory.Slot buttonSlot) {
        itemImage = GetComponentsInChildren<Image> ()[1];
        countText = GetComponentInChildren<Text> ();
        slot = buttonSlot;
    }

    public void UpdateButton () {
        if (slot.item) {
            itemImage.gameObject.SetActive (true);

            if (slot.item.prefab.icon)
                itemImage.sprite = slot.item.prefab.icon;
            else
                itemImage.sprite = Resources.Load<Sprite> ("GUI/PlaceholderImage");

            if (slot.count > 1)
                countText.text = slot.count.ToString ();
            else
                countText.text = "";
        }else {
            itemImage.gameObject.SetActive (false);
            countText.text = "";
        }
    }
}
