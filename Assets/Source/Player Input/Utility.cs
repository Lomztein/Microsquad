using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

	public static Vector3 ClosestPointToSphere (Vector3 center, float radius, Vector3 point) {
		if (Vector3.Distance (center, point) < radius)
			return point;

		return (point - center).normalized * radius;
	}
}
