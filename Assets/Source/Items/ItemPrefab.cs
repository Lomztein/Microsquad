using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "New Item Prefab",menuName = "Microsquad/Create Item Prefab", order = 0)]
public class ItemPrefab : ScriptableObject {

	public enum Rarity { Common, Uncommnon, Special, Rare, VeryRare, Legendary };
	public enum Type { Tool, HeadArmor, ChestArmor, LegArmor, Consumeable, Ammunition };
	
	public GameObject gameObject;
	public Rarity rarity;
	public Type type;
	public CharacterEquipment.Slot slotType;
	public Sprite icon;
	public Mesh model;
	public string data;

    public static explicit operator Item (ItemPrefab prefab) {
        Item item = CreateInstance<Item> ();
        item.name = prefab.name;
        item.prefab = prefab;
        item.metadata = prefab.data;
        return item;
    }

}