using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmo {

    GameObject GetProjectileObject();

    AmmoPrefab.Type GetAmmoType();

    Mesh GetShellMesh();

}
