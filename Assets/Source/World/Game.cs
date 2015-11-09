using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	[Header ("Game Information")]
	public Squad currentSquad;
	public static PhysicalItem itemInHand;

	[Header ("References")]
	public static Game game;

	[Header ("Prefabs")]
	public GameObject physicalItemPrefab;

	[Header ("Layer Masks")]
	public LayerMask all;
	public LayerMask playerLayer;
	public LayerMask curroptLayer;
	public LayerMask neutralLayer;
	public LayerMask scavangerLayer;

	void Awake () {
		game = this;
	}

}
