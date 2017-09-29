using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (Room))]
public class RoomInspector : Editor {

	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		Room room = target as Room;
		if (GUILayout.Button ("Add navmesh vertex")) {
			if (!room.roomNavMesh)
				room.roomNavMesh = new Mesh ();
			
			Vector3[] currentVerts = room.roomNavMesh.vertices.Clone() as Vector3[];
			room.roomNavMesh.vertices = new Vector3[currentVerts.Length + 1];
			for (int i = 0; i < currentVerts.Length; i++) {
				room.roomNavMesh.vertices[i] = currentVerts[i];
			}
			room.roomNavMesh.vertices[currentVerts.Length] = new Vector3 (0f, 0f, 5f);
		}
		if (room.roomNavMesh) {
			for (int i = 0; i < room.roomNavMesh.vertexCount; i++) {
				room.roomNavMesh.vertices[i] = Handles.PositionHandle (room.roomNavMesh.vertices[i], Quaternion.identity);
			}
		}
	}

	void Update () {
		Room room = target as Room;

	}

	void OnDrawGizmos () {
		Room room = target as Room;
		if (room.roomNavMesh) {
			Gizmos.DrawMesh (room.roomNavMesh);
			for (int i = 0; i < room.roomNavMesh.vertexCount; i++) {
				Gizmos.DrawSphere (room.transform.position + room.roomNavMesh.vertices[i], 0.25f);
			}
		}
	}
}
