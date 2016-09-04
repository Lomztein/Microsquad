using UnityEngine;
using System.Collections;

public class StartingEquipment : MonoBehaviour {

    public Piece[] equipmentPieces;
    public Piece[] inventoryPieces;

	// Use this for initialization
	void Start () {
        ApplyInventory ();
	}

    void ApplyInventory () {
        Character character = GetComponent<Character> ();
        foreach (Piece p in equipmentPieces) {
            character.ChangeEquipment (p.item.slotType, character.FindSlotByName (p.slotName), GetItem (p));
        }

        if (character.inventory) {
            for (int i = 0; i < inventoryPieces.Length; i++) {
                character.inventory.items[i] = GetItem (inventoryPieces[i]);
            }
        }

        Destroy (this);
    }

    private Item GetItem (Piece piece) {
        Item item = (Item)piece.item;
        if (piece.metadata.Length > 0)
            item.metadata = piece.metadata;
        return item;
    }

    [System.Serializable]
    public class Piece {
        public ItemPrefab item;
        public string metadata;
        public string slotName;
    }
}
