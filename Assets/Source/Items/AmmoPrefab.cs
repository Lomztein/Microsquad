using UnityEngine;
using System.Collections;

public class AmmoPrefab : ItemPrefab {

    public enum AmmoType { Pistol, Rifle, Shotgun, Sniper, Rocket, Energy, Magic }

    public AmmoType ammoType;

    public override Item GetItem () {
        Item item = base.GetItem ();
        item.attributes.AddAttribute ("AmmoType", ammoType);
        return item;
    }
}
