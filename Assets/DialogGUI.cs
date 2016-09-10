using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogGUI : MonoBehaviour {

    public TypingText dialogText;
    public Text speakerNameText;

    public static DialogGUI cur;
    public Coroutine routine;

    void Awake () {
        cur = this;
    }

    public static void RestartCoroutine (IEnumerator routine) {
        cur.StopAllCoroutines ();
        cur.routine = cur.StartCoroutine (routine);
    }

    public static IEnumerator RunDialog (string speakerName, params string[] messages ) {
        cur.SetAllGUIGraphics (true);
        cur.speakerNameText.text = speakerName;
        for (int i = 0; i < messages.Length; i++) {
            yield return cur.dialogText.TypeText (messages[i]);
        }
        cur.SetAllGUIGraphics (false);
    }

    void SetAllGUIGraphics (bool setting) {
        Graphic[] graphics = GetComponentsInChildren<Graphic> ();
        for (int i = 0; i < graphics.Length; i++) {
            graphics[i].enabled = setting;
        }
    }
}
