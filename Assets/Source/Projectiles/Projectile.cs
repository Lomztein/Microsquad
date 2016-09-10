using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float speed;
	public Vector3 velocity;
	public int damage;
	public Faction faction;
	public float range;
	public LayerMask layer;
    public Character character;

	void FixedUpdate () {
		CastRay ();
		transform.position += velocity * Time.fixedDeltaTime;
		Destroy (gameObject, range / velocity.magnitude);
		layer = Game.game.all - Game.game.layers[(int)faction];
	}

	void CastRay () {
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, velocity.magnitude * Time.fixedDeltaTime, layer)) {
			Vector3 force = velocity * damage / 10f;
			Character oc = hit.collider.GetComponent<Character>();
			if (oc) {
				if (oc.faction != Faction.Scavengers) {
					oc.SendMessage ("OnTakeDamage", new Damage (damage, force, character, hit.point), SendMessageOptions.DontRequireReceiver);
					Destroy (gameObject);
				}
			}else{
				hit.collider.SendMessage ("OnTakeDamage", new Damage (damage, force, character, hit.point), SendMessageOptions.DontRequireReceiver);
				Destroy (gameObject);
			}
		}

	}
}

public struct Damage {

	public int damage;
	public Vector3 force;
	public Character character;
	public Vector3 point;

	public Damage (int _d, Vector3 _force, Character _char, Vector3 _point) {
		damage = _d;
		force = _force;
		character = _char;
		point = _point;
	}
}
