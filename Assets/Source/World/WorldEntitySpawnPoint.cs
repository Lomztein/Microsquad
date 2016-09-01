using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WorldEntitySpawnPoint : MonoBehaviour {

    const float IMPORT_SCALE = 100f;

    public int spawnChance = 100;
    public bool spawnOnCreation;
    public GameObject[] possibleEntities = new GameObject[0];
    public GameObject representativeMesh;

    void Start () {
        if (spawnOnCreation)
            Spawn ();

        Destroy (representativeMesh);
    }

	public void Spawn () {
        int ranNumber = Random.Range (0, 100);
        if (ranNumber < spawnChance) {
            Instantiate (possibleEntities[Random.Range (0, possibleEntities.Length)], transform.position, transform.rotation);
        }
    }
}
