using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dungeon : MonoBehaviour {

	public GameObject startingRoom;
	public GameObject[] roomPrefabs;
	public GameObject blindWall;
	public int maxRooms;

	public Queue<Room> remainingRooms = new Queue<Room>();
	public List<Room> currentRooms = new List<Room>();
	public List<Door> allDoors = new List<Door>();

	private Bounds checkingBounds;
	private Bounds currentBounds;
	public int checksPerTick;

	void Start () {
		StartCoroutine (GenerateDungeon ());
	}

	void EnqueueRoom (Room r) {
		r.transform.parent = transform;
		for (int a = 0; a < r.doors.Length; a++) {
			remainingRooms.Enqueue (r);

			if (!allDoors.Contains (r.doors[a])) {
				allDoors.Add (r.doors[a]);
			}
		}
	}

	Bounds AbsBounds (Bounds bounds) {
		return new Bounds (bounds.center, new Vector3 (Mathf.Abs (bounds.size.x), Mathf.Abs (bounds.size.y), Mathf.Abs (bounds.size.z)));
	}

	IEnumerator GenerateDungeon () {
		GameObject firstRoom = (GameObject)Instantiate (startingRoom, Vector3.zero, Quaternion.identity);
		EnqueueRoom (firstRoom.GetComponent<Room> ());
		currentRooms.Add (firstRoom.GetComponent<Room> ());

		int numRooms = 1;
		int curChecks = 0;

        for (int i = 0; i < roomPrefabs.Length; i++) {
            NavMeshObstacle[] obstacles = roomPrefabs[i].GetComponentsInChildren<NavMeshObstacle> ();
            for (int j = 0; j < obstacles.Length; j++) {
                obstacles[j].carving = false;
            }
        }

		GameObject bp = new GameObject ("BuildPointer");
		Transform buildPointer = bp.transform;
		
		while (numRooms < maxRooms && remainingRooms.Count > 0) {
			Room curRoom = remainingRooms.Dequeue ();

			for (int j = 0; j < curRoom.doors.Length; j++) {

				if (!curRoom.doors [j].isConnected) {
					GameObject newRoomGO = roomPrefabs [Random.Range (0, roomPrefabs.Length)];
					if (curRoom.doors [j].forcedRoom) {
						newRoomGO = curRoom.doors [j].forcedRoom;
					}
					Room newRoom = newRoomGO.GetComponent<Room> ();
						
					Transform door = curRoom.doors [j].transform;
					Vector3 spawnPos = door.position + door.forward * (newRoom.Size ().z / 2f);
					spawnPos = new Vector3 (spawnPos.x, 0f, spawnPos.z);
					
					buildPointer.position = spawnPos;
					buildPointer.LookAt (door);

					buildPointer.rotation = Quaternion.Euler (0, buildPointer.eulerAngles.y, 0);

					bool allowBuild = true;

					for (int i = 0; i < currentRooms.Count; i++) {
						currentBounds = AbsBounds (currentRooms [i].GetBounds ());
						checkingBounds = AbsBounds (new Bounds (spawnPos, buildPointer.rotation * newRoom.size * 0.9f));

						if (curChecks >= checksPerTick) {
							yield return new WaitForFixedUpdate ();
							curChecks = 0;
						} else {
							curChecks++;
						}

						if (checkingBounds.Intersects (currentBounds)) {
							curRoom.doors [j].isConnected = true;
							allowBuild = false;
						}
					}

					if (allowBuild) {
						numRooms++;
						curRoom.doors [j].isConnected = true;
						GameObject spawnedRoom = (GameObject)Instantiate (newRoomGO, buildPointer.position, buildPointer.rotation);

						curRoom = null;
						Room r = spawnedRoom.GetComponent<Room> ();
						r.doors [0].isConnected = true;
						currentRooms.Add (r);
						EnqueueRoom (r);

						break;
					}
				}
			}
		}

		// Check all doors to find out if a door is nearby, and then removed one of the doors based on priority.
		Transform survivingDoor = null;
		bool noNearby = true;

		List<GameObject> toDestroy = new List<GameObject> ();
		List<Transform> toEnd = new List<Transform> ();

		for (int i = 0; i < allDoors.Count; i++) {
			survivingDoor = allDoors [i].transform;
			noNearby = true;
			Door t = allDoors [i];
			if (toDestroy.Contains(t.transform.gameObject))
				continue;

			for (int j = 0; j < allDoors.Count; j++) {
				Door d = allDoors [j];

				if (t.transform == d.transform)
					continue;

				if (Vector3.Distance (t.transform.position, d.transform.position) < 0.5f) {
					if (t.priority > d.priority) {
						toDestroy.Add (d.transform.gameObject);
						survivingDoor = t.transform;
					} else {
						toDestroy.Add (t.transform.gameObject);
						survivingDoor = d.transform;
					}
					noNearby = false;
					break;
				}
			}

			if (noNearby && survivingDoor) {
				toEnd.Add (survivingDoor);
			}
		}

		foreach (Transform door in toEnd) {
			GameObject wall = (GameObject)Instantiate (blindWall, new Vector3 (door.position.x, 0, door.position.z), door.rotation);
			wall.transform.parent = transform;
			yield return new WaitForFixedUpdate ();
		}

		foreach (GameObject door in toDestroy) {
			Destroy (door);
			yield return new WaitForFixedUpdate ();
		}

        NavMeshObstacle[] obs = GetComponentsInChildren<NavMeshObstacle> ();
        for (int j = 0; j < obs.Length; j++) {
            obs[j].carving = true;
        }
    }

	void OnDrawGizmos () {
		Gizmos.DrawCube (currentBounds.center + Vector3.up * currentBounds.size.y / 2f, currentBounds.size);
		Gizmos.DrawWireCube (checkingBounds.center + Vector3.up * checkingBounds.size.y / 2f, checkingBounds.size);
	}
}
