using UnityEngine;
using System.Collections;

public class OrbitingCamera : MonoBehaviour {

    public float orbitSpeed;
    public Vector3 focusingPoint;

    public float distance;
    public float height;

	void FixedUpdate () {
        transform.LookAt (focusingPoint);
        transform.position -= transform.forward * (distance - Vector3.Distance (transform.position, focusingPoint));
        transform.position += transform.up * (height - transform.position.y);
        transform.position += transform.right * orbitSpeed * Time.fixedDeltaTime;
    }
}
