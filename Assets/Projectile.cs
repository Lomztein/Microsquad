using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float speed;
	public Vector3 velocity;
	public int damage;
	public Character.Faction faction;
	public float range;

	void FixedUpdate () {
		CastRay ();
		transform.position += velocity * Time.fixedDeltaTime;
		Destroy (gameObject, range / velocity.magnitude);
	}

	void CastRay () {
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, velocity.magnitude * Time.fixedDeltaTime)) {
			Character oc = hit.collider.GetComponent<Character>();
			if (oc) {
				if (oc.faction != faction) {
					oc.SendMessage ("OnTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				}
			}else{
				hit.collider.SendMessage ("OnTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			}
			Destroy (gameObject);
		}

	}
}
