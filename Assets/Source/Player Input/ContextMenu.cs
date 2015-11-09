using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour {

	public GameObject buttonPrefab;
	public RectTransform backgroundPanel;

	public Vector3 buttonSize;
	private static List<GameObject> curButtons = new List<GameObject>();
	public static ContextMenu menu;

	void Awake () {
		menu = this;
		gameObject.SetActive (false);
	}

	public static void Open (ContextMenuElement.Element[] elements, ContextMenuElement behaviour) {
		menu.gameObject.SetActive (true);
		menu.transform.position = Input.mousePosition + new Vector3 (menu.buttonSize.x / 2f, -menu.buttonSize.y * elements.Length / 2f);
		foreach (GameObject go in curButtons) {
			Destroy (go);
		}
		curButtons = new List<GameObject> ();

		menu.backgroundPanel.sizeDelta = new Vector2 (menu.buttonSize.x, menu.buttonSize.y * elements.Length);
		for (int i = 0; i < elements.Length; i++) { 
			GameObject newb = (GameObject)Instantiate (menu.buttonPrefab, Vector3.zero, Quaternion.identity);
			newb.transform.position = Input.mousePosition + new Vector3 (menu.buttonSize.x / 2f, -menu.buttonSize.y / 2f + menu.buttonSize.y * -i);
			curButtons.Add (newb);
			menu.AddListener (newb, i, elements[i].name, behaviour);
			newb.transform.SetParent (menu.transform, true);
		}

	}

	void AddListener (GameObject button, int index, string text, ContextMenuElement element) {
		Button b = button.GetComponent <Button> ();
		Text t = button.transform.GetChild (0).GetComponent<Text> ();
		t.text = text;
		b.onClick.AddListener(() => element.OnElementClicked (index));
	}
}
