using UnityEngine;
using System.Collections;

public class ContextMenuElement : MonoBehaviour {

	public Element[] elements;

	public void OnElementClicked (int index) {
		gameObject.SendMessage ("CM" + elements [index].method);
	}

	[System.Serializable]
	public class Element {
		public string name;
		public string method;
	}
}
