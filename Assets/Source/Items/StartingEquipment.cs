﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartingEquipment : MonoBehaviour {

    public Piece[] equipmentPieces;
    public Piece[] inventoryPieces;
    public List<Inventory.Slot> tempSlots;

    public bool applyOnStart = true;

	// Use this for initialization
	void Start () {
        if (applyOnStart)
            ApplyInventory ();
	}

    public void ApplyInventory () {
        Inventory inventory = GetComponent<Inventory> ();
        if (inventory) {
            for (int i = 0; i < inventoryPieces.Length; i++) {
                inventory.slots[i].item = GetItem (inventoryPieces[i]).item;
                inventory.slots[i].count = GetItem (inventoryPieces[i]).count;
            }
        }

        Character character = GetComponent<Character> ();
        if (character) {
            foreach (Piece p in equipmentPieces) {
                character.ChangeEquipment ((p.item as IEquipable).GetSlotType (), character.FindSlotByName (p.slotName), GetItem (p));
            }
        }

        for (int i = 0; i < tempSlots.Count; i++)
            Destroy (tempSlots[i]);
        Destroy (this);
    }

    private Inventory.Slot GetItem (Piece piece) {

        Inventory.Slot tempSlot = Inventory.Slot.CreateSlot ();
        tempSlot.item = piece.item.GetItem ();
        // Fuck I love these compact if statements.
        tempSlot.count = piece.count == -1 ? piece.item.maxStack : piece.count;
        tempSlots.Add (tempSlot);

        if (piece.metadata.Length > 0)
            tempSlot.item.metadata = piece.metadata;
        return tempSlot;
    }

    [System.Serializable]
    public class Piece {
        public ItemPrefab item;
        public int count;

        public string metadata;
        public string slotName;
    }
}
