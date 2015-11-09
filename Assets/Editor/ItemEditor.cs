using UnityEngine;
using System.Collections;
using UnityEditor;

public class ItemEditor : EditorWindow {

	public Item currentItem;

	[MenuItem ("Microsquad/Item Editor")]
	public static void ShowWindow () {
		EditorWindow.GetWindow (typeof(ItemEditor));
	}

	void OnGUI () {
		currentItem = (Item)EditorGUILayout.ObjectField (currentItem, typeof (Item), true);
		if (currentItem) {
			currentItem.name = EditorGUILayout.TextField ("Item name", currentItem.name);
			currentItem.gameObject = (GameObject)EditorGUILayout.ObjectField ("Item object", currentItem.gameObject, typeof (GameObject), false);
			currentItem.rarity = (Item.Rarity)EditorGUILayout.EnumPopup ("Item rarity", currentItem.rarity);
			currentItem.model = (Mesh)EditorGUILayout.ObjectField ("Item model", currentItem.model, typeof (Mesh), false);
			currentItem.value = EditorGUILayout.IntField ("Item value", currentItem.value);
			if (GUILayout.Button ("Save item to assets")) {
				ScriptableObjectUtility.CreateAsset<Item> (currentItem);
			}
		}else{
			if (GUILayout.Button ("Create new item")) {
				currentItem = Item.CreateInstance<Item>();
			}
		}
	}
}
