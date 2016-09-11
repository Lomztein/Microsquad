﻿using UnityEngine;
using System.Collections;

public class ItemDropper : MonoBehaviour {

    public int dropAmount;
    public float dropRate;
    private int dropped = 0;

    public ItemPrefab[] dropItems;
    public Transform dropPosition;

	// Use this for initialization
	void Start () {
        Invoke ("Drop", dropRate);
	}
	
	void Drop () {
        ItemPrefab prefab = dropItems[Random.Range (0, dropItems.Length)];
        GameObject newItem = PhysicalItem.Create ((Item)prefab, 1, dropPosition.position, Quaternion.Euler (0f, Random.Range (0f, 360f), 0f)).gameObject;
        dropped++;
        if (dropped < dropAmount)
            Invoke ("Drop", dropRate);
    }
}