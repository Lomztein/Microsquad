using UnityEngine;
using System.Collections;

public class HoverContextElement : MonoBehaviour {

    public static HoverContextElement activeElement;
	public string text;
	private bool prevHit;
    public bool isWorldElement;

	void OnMouseEnterElement () {
		HoverContext.ChangeText (text);
	}

	void OnMouseExitElement () {
		HoverContext.ChangeText ("");
    }

    public void ForceUpdate () {
        activeElement = null;
    }

    void Start () {
        Invoke ("SetSize", 0.1f);
    }

    void SetSize () {
        RectTransform rTransform = GetComponent<RectTransform> ();
        if (rTransform) {
            BoxCollider col = GetComponent<BoxCollider> ();
            col.size = new Vector3 (rTransform.sizeDelta.x, rTransform.sizeDelta.y, 1f);
        }
    }
}
