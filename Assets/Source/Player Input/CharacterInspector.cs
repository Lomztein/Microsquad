using UnityEngine;
using System.Collections;

public class CharacterInspector : MonoBehaviour {

	public Character character;
	public Transform inspectorCamera;

	// Update is called once per frame
	void Update () {
		if (character) {
			inspectorCamera.transform.position = character.transform.position + character.transform.forward * 5f + Vector3.up * 0.9f;
			inspectorCamera.LookAt (character.transform.position + Vector3.up * 0.9f);
		}
	}

	void OpenCharacter (Character c) {
		character = c;
		if (!gameObject.activeInHierarchy)
			gameObject.SetActive (true);
	}

	void CloseWindow () {
		gameObject.SetActive (false);
	}
}
