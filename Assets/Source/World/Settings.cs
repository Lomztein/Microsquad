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
    public Color guiContrastColor;

    public Color guiTextColor;

    public bool debugMode;

    // Dev settings aren't supposed to be changed from within the game,
    // and exists more of a utility tool, so that one doesn't have to change
    // all thing affected manually.
    [Header ("Dev Settings")]
    public Font standardFont;
    public ColorBlock buttonColor;

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
                }

                if (g.gameObject.tag == "ContrastGUIElement") {
                    g.color = guiContrastColor;
                }

                if (g.gameObject.tag == "ContrastGUIElement" || g.gameObject.tag == "ColoredGUIElement") { }
                    Text t = g.GetComponent<Text> ();
                    if (t) {
                    t.color = guiTextColor;
                    t.font = standardFont;

                    Button b = g.GetComponent<Button> ();
                    if (b)
                        b.colors = buttonColor;
                }
            }
        }
    }



    void OnGUI () {
        if (debugMode) {
            debugIndex = -1;
            if (PlayerInput.itemInHand)
                GUI.Label (GetDebugPosition (), "Item in hand: " + PlayerInput.itemInHand.ToString ());

            GUI.Label (GetDebugPosition (), "Active context menus: " + ContextMenu.activeMenus.Count);

            if (PlayerInput.cur)
                GUI.Label (GetDebugPosition (), "Current advanced command: " + PlayerInput.cur.currentAdvCommand.ToString ());

            if (HoverContextElement.activeElement)
                GUI.Label (GetDebugPosition (), "Current active tooltip: " + HoverContextElement.activeElement);
        }
    }

    int debugIndex = 0;
    Rect GetDebugPosition () {
        debugIndex++;
        return new Rect (10, 10 + 20 * debugIndex, Screen.width, 20);
    }
}
