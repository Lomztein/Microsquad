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

    public bool typeOnStart = true;
    public bool skipOnSubmit;
    private bool skip;

	// Use this for initialization
	void Start () {
        if (typeOnStart) {
            startingText = text.text;
            StartCoroutine (TypeText ());
        }
	}

    void Update () {
        if (skipOnSubmit && Input.GetButtonDown ("Submit")) {
            skip = true;
        }
    }

    // If all coroutines that run these functions run on the instance, that allows the typing to be cancelled and for another one to run, in its place.
    public IEnumerator TypeText (string text) {
        startingText = text;
        yield return TypeText ();
    }

    IEnumerator TypeText () {
        skip = false;
        text.text = "";

        yield return WaitForSkippableSeconds (startWaitTime);

        for (int i = 0; i < startingText.Length; i++) {
            char cur = startingText[i];

            if (cur == '\n' && !skip) {
                yield return WaitForSkippableSeconds (lineWaitTime);
            }

            if (!skip)
                yield return WaitForSkippableSeconds (Random.Range (charWaitTime * 0.5f, charWaitTime * 1.5f));

            text.text += cur;

            if ((cur == '.' || cur == '!' || cur == '?' || cur == ',') && !skip)
                yield return WaitForSkippableSeconds (charWaitTime * 8f);
        }

        bool didSkip = skip;
        skip = false;

        if (didSkip)
            yield return WaitUntillSkip ();
        else
            yield return WaitForSkippableSeconds (afterWait);
        
        for (int i = 0; i < revealAfter.Length; i++) {
            revealAfter[i].SetActive (true);
        }

        skip = false;

    }

    IEnumerator WaitForSkippableSeconds (float time) {
        int waitTime = Mathf.RoundToInt (time / PlayerInput.defaultFixedDeltaTime);
        for (int i = 0; i < waitTime; i++) {
            if (!skip) {
                yield return new WaitForFixedUpdate ();
            }
        }
    }

    IEnumerator WaitUntillSkip () {
        while (!skip)
            yield return new WaitForFixedUpdate ();
    }
}
