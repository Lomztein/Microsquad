using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

	public static Vector3 ClosestPointToSphere (Vector3 center, float radius, Vector3 point) {
		if (Vector3.Distance (center, point) < radius)
			return point;

		return (point - center).normalized * radius;
	}

    public static bool LineOfSight (Vector3 start, Vector3 end) {
        Ray ray = new Ray (start, end - start);
        RaycastHit hit;

        Debug.DrawLine (start, end);
        float distance = Vector3.Distance (start, end);

        if (Physics.SphereCast (ray, 0.15f, out hit, distance, Game.game.terrainLayer))
            return false;

        return true;
    }
}
