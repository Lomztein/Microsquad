using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

    public static GUIManager cur;

    [Header ("GUI Objects")]
    public GameObject gameOverScreen;

    public static Canvas mainCanvas;

	// Use this for initialization
	void Awake () {
        cur = this;
        mainCanvas = GetComponent<Canvas> ();
	}
	
	public void ShowGameOverScreen () {
        gameOverScreen.SetActive (true);
    }
}
