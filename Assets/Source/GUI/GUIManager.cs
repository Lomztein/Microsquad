using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

    public GUIManager cur;

    public static Canvas mainCanvas;

	// Use this for initialization
	void Awake () {
        cur = this;
        mainCanvas = GetComponent<Canvas> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
