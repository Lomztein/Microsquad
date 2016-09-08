using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TypingText : MonoBehaviour {

    private string startingText;

    public float charWaitTime = 0.05f;
    public float startWaitTime = 1f;
    public float lineWaitTime = 0.5f;
    public float afterWait;

    public GameObject[] revealAfter;

    public Text text;

	// Use this for initialization
	void Start () {
        if (Application.isPlaying) {
            startingText = text.text;
            StartCoroutine (TypeText ());
        }
	}

    IEnumerator TypeText () {
        text.text = "";
        yield return new WaitForSeconds (startWaitTime);

        for (int i = 0; i < startingText.Length; i++) {
            char cur = startingText[i];

            if (cur == '\n') {
                yield return new WaitForSeconds (lineWaitTime);
            }

            yield return new WaitForSeconds (Random.Range (charWaitTime * 0.5f, charWaitTime * 1.5f));
            text.text += cur;
        }

        yield return new WaitForSeconds (afterWait);
        for (int i = 0; i < revealAfter.Length; i++) {
            revealAfter[i].SetActive (true);
        }
    }
}
