using UnityEngine;
using System.Collections;

public class OpenableDoor : MonoBehaviour {

	private bool isOpen;
	public bool isLocked;
	public Animation animator;

	void CMOpen () {
		if (!isLocked) {
			gameObject.SetActive (false);
		}
	}
}
