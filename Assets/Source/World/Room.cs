using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour {

	public Door[] doors;
	public Vector3 size;
	private Bounds bounds;

	void OnDrawGizmos () {
		Gizmos.DrawWireCube (transform.position + new Vector3 (0f, Size ().y / 2f, 0f), Size ());
	}

	public Vector3 Size () {
		return transform.rotation * size;
	}

	public Vector3 Size (Quaternion rotation) {
		return rotation * size;
	}

	public Bounds GetBounds () {
		return new Bounds (transform.position, transform.rotation * size);
	}

	public Bounds GetBounds (Quaternion rotation) {
		return new Bounds (transform.position, rotation * size * 0.9f);
	}
}

[System.Serializable]
public class Door {

	public Transform transform;
	public GameObject forcedRoom;
	public bool isConnected;
	public int priority;

}