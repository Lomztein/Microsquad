using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of interface which is able to contain another item.
/// </summary>
public interface IContainsItem {

	Inventory.Slot Slot {
        get;
    }
}
