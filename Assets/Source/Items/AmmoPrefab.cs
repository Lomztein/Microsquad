using UnityEngine;
using System.Collections;

public class AmmoPrefab : ItemPrefab {

    public enum AmmoType { Pistol, Rifle, Sniper, Rocket, Energy, Magic }

    public AmmoType ammoType;

    public static explicit operator Item ( AmmoPrefab prefab ) {
        Item item = (Item)prefab;
        item.attributes.AddAttribute ("AmmoType", prefab.ammoType);
        return item;
    }

}
