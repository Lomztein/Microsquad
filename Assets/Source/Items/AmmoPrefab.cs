using UnityEngine;
using System.Collections;
using System;

public class AmmoPrefab : ItemPrefab, IAmmo {

    [Flags]
    public enum Type {
        Pistol = 1, Rifle = 2, Shotgun = 4, Sniper = 8, Rocket = 16, Energy = 32, Magic = 64
    }

    public Type type;
    public Mesh shellMesh;

    public GameObject GetProjectileObject() {
        return gameObject;
    }

    public Type GetAmmoType() {
        return type;
    }

    public Mesh GetShellMesh() {
        return shellMesh;
    }
}
