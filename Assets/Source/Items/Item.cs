using UnityEngine;
using System.Collections;

public class Item : ScriptableObject {

	public enum Rarity { Common, Uncommnon, Special, Rare, VeryRare, Legendary };
	public enum Type { OneHandTool, TwoHandTool, HeadArmor, ChestArmor, LegArmor, Consumeable, Ammunition };
	
	public GameObject gameObject;
	public Rarity rarity;
	public Type type;
	public CharacterEquipment.Slot slotType;
	public Sprite icon;
	public Mesh model;
	public int value;
	public string data;

	public static Item CreateItem (GameObject _gameObject, Rarity _rarity, Type _type, CharacterEquipment.Slot _slotType, Sprite _icon, Mesh _model, int _value, string _data) {
		Item item = Item.CreateInstance<Item> ();
		item.gameObject = _gameObject;
		item.rarity = _rarity;
		item.type = _type;
		item.slotType = _slotType;
		item.icon = _icon;
		item.model = _model;
		item.value = _value;
		item.data = _data;

		return item;
	}

	public Item CloneItem () {
		return CreateItem (gameObject, rarity, type, slotType, icon, model, value, data);
	}

	public void Destroy () {
		Destroy (this);
	}
}