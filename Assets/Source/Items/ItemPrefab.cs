using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "New Item Prefab",menuName = "Microsquad/Create Item Prefab", order = 0)]
public class ItemPrefab : ScriptableObject {

	public enum Rarity { Common, Uncommnon, Special, Rare, VeryRare, Legendary };
    public static Color [ ] rarityColors = new Color [ ] {
        Color.white, Color.blue, Color.yellow, Color.red, Color.magenta, Color.green
    };

    public string itemName;
    public string itemDescription;

	public GameObject gameObject;
    // The model variable should only be used if the gameObject cannot represent the item properly in an icon. In that case, the model will take its place.
    public GameObject model;
    public Sprite overridingIcon;
	public Rarity rarity;
    public string data;
    public int maxStack = 999;

    public virtual Item GetItem () {
        Item item = CreateInstance<Item> ();
        item.name = name;
        item.prefab = this;
        item.metadata = data;
        return item;
    }
}