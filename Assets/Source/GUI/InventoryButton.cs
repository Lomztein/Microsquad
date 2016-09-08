﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryButton : Button {

    public Inventory.Slot slot;
    public RawImage itemImage;
    public Text countText;

    public void Init (Inventory.Slot buttonSlot) {
        itemImage = GetComponentInChildren<RawImage> ();
        countText = GetComponentInChildren<Text> ();
        slot = buttonSlot;
    }

    public void UpdateButton () {
        if (slot.item) {
            itemImage.gameObject.SetActive (true);

            if (slot.item)
                itemImage.texture = slot.item.GetIcon ();
            else
                itemImage.texture = null;

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
