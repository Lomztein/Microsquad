using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Settings : MonoBehaviour {

    public static Settings cur;

    public bool update;

    [Header ("References")]
    public Canvas[] canvasses;

    [Header ("Settings")]
    public Color guiImageColor;
    public Color guiTextColor;
    public bool debugMode;

    // Dev settings aren't supposed to be changed from within the game,
    // and exists more of a utility tool, so that one doesn't have to change
    // all thing affected manually.
    [Header ("Dev Settings")]
    public Font standardFont;

    void Awake () {
        cur = this;
    }

    void Update () {
        if (update) {
            UpdateGUIColor ();
        }
    }

    void UpdateGUIColor () {
        foreach (Canvas c in canvasses) {
            Graphic[] graphics = c.GetComponentsInChildren<Graphic> ();
            foreach (Graphic g in graphics) {
                if (g.gameObject.tag == "ColoredGUIElement") {
                    g.color = guiImageColor;
                    Text t = g.GetComponent<Text> ();
                    if (t)
                        t.color = guiTextColor;
                }
            }
        }
    }



    void OnGUI () {
        if (debugMode) {
            debugIndex = -1;
            if (PlayerInput.itemInHand)
                GUI.Label (GetDebugPosition (), "Item in hand: " + PlayerInput.itemInHand.ToString ());
        }
    }

    int debugIndex = 0;
    Rect GetDebugPosition () {
        debugIndex++;
        return new Rect (10, 10 + 30 * debugIndex, Screen.width, 20);
    }
}
