using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RagdollHandler : MonoBehaviour {

	public RagdollPart[] majorParts;
	public float dismembermentForce;
	public static Queue<RagdollHandler> ragdolls = new Queue<RagdollHandler>();
	public static int maxRagdollsInScene = 16;

	[System.Serializable]
	public class RagdollPart {

		public Rigidbody part;
		public bool canDestroy;

	}

	IEnumerator Fade () {
		Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody b in bodies) {
			b.isKinematic = true;
		}

		float time = 2f;
		while (time > 0f) {
			transform.position -= Vector3.up * Time.fixedDeltaTime;
			time -= Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate ();
		}
		Destroy (gameObject);
	}

	public void OnTakeDamage (Damage d) {
		Vector3 point = d.point;

		float dist = float.MaxValue;
		int index = 0;

		for (int i = 0; i < majorParts.Length; i++) {
			float ld = Vector3.Distance (point, majorParts[i].part.transform.position);

			if (ld > dist) {
				ld = dist;
				index = i;
			}
		}

		majorParts[index].part.AddForceAtPosition (d.force, point);
		if (majorParts[index].canDestroy && d.force.magnitude > dismembermentForce) {
			majorParts[index].part.transform.localScale = Vector3.zero;
		}

		ragdolls.Enqueue (this);
		if (ragdolls.Count > maxRagdollsInScene) {
			RagdollHandler h = ragdolls.Dequeue ();
			h.StartCoroutine (h.Fade ());
		}
	}
}
