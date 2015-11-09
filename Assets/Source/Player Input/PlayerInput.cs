using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	private Camera camera;
	public float sensitivity = 5f;
	public static Vector3 worldMousePos;
	public static Vector3 screenMousePos;

	// Use this for initialization
	void Start () {
		camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {

		screenMousePos = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit, Mathf.Infinity))
			worldMousePos = hit.point;

		Vector3 movement = Vector3.zero;
		if (screenMousePos.x < 5f) {
			movement += Vector3.left;
		} else if (screenMousePos.x > Screen.width - 5f) {
			movement += Vector3.right;
		}

		if (screenMousePos.y < 5f) {
			movement += Vector3.back;
		} else if (screenMousePos.y > Screen.height - 5f) {
			movement += Vector3.forward;
		}

		movement.y += Input.GetAxis ("Mouse ScrollWheel");
		transform.position += movement * sensitivity * Time.deltaTime;

	}
}
