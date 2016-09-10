using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour {

	public RectTransform backgroundPanel;
	public Vector3 buttonSize;
    public ContextMenuElement element;
    public int elementCount;

    public static List<ContextMenu> activeMenus = new List<ContextMenu>();

	public static void Open (ContextMenuElement.Element[] elements, ContextMenuElement behaviour) {
        GameObject panelPrefab = Resources.Load<GameObject> ("GUI/ContextMenu");

        GameObject newMenu = Instantiate (panelPrefab);
        ContextMenu menu = newMenu.GetComponent<ContextMenu> ();
        menu.Initialize (elements, behaviour);
	}

    void Update () {
        if (Input.GetMouseButtonDown (1))
            Destroy ();

        Vector3 pos = Camera.main.WorldToScreenPoint (element.transform.position);
        transform.position = new Vector3 (pos.x, pos.y, 0f) + new Vector3 (buttonSize.x / 2f, -buttonSize.y * elementCount / 2f);
    }

    void Initialize (ContextMenuElement.Element[] elements, ContextMenuElement behaviour) {
        activeMenus.Add (this);
        GameObject buttonPrefab = Resources.Load<GameObject> ("GUI/ContextButton");

        element = behaviour;
        elementCount = elements.Length;
        transform.SetParent (GUIManager.mainCanvas.transform, true);

        for (int i = 0; i < elements.Length; i++) {
            GameObject newb = (GameObject)Instantiate (buttonPrefab, Vector3.zero, Quaternion.identity);

            AddListener (newb, i, elements[i].name, behaviour);
            newb.transform.SetParent (transform, true);
        }
    }

    void Destroy () {
        activeMenus.Remove (this);
        Destroy (gameObject);
    }

	void AddListener (GameObject button, int index, string text, ContextMenuElement element) {
		Button b = button.GetComponent <Button> ();
		Text t = button.transform.GetChild (0).GetComponent<Text> ();
		t.text = text;
        b.onClick.AddListener (() => OnClick (index));
	}

    void OnClick (int index) {
        element.OnElementClicked (index);
        Destroy ();
    }
}
