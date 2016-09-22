using UnityEngine;
using System.Collections;

public class WorldCursor : MonoBehaviour {

	public enum CursorType { Disable, Select, Move, Attack, AttackMove, Defend };

	public Material[] materials;
	public GameObject[] cursors;

	public static Renderer[] cursorRenderer;
	public static WorldCursor cur;

	// Use this for initialization
	void Awake () {
		cur = this;
		cursorRenderer = new Renderer[cursors.Length];
		for (int i = 0; i < cursors.Length; i++) {
			cursorRenderer[i] = cursors[i].GetComponent<Renderer>();
		}
	}

	/// <summary>
	/// Forces the material of the cursor at index. Use after SetCursor function.
	/// </summary>
	/// <param name="cursor">Cursor.</param>
	/// <param name="material">Material.</param>
	public static void ForceMaterial (int cursor, Material material) {
		cursorRenderer[cursor].sharedMaterial = material;
	}

	public static void SetCursor (CursorType type) {
		if (type == CursorType.Disable) {
			cur.cursors[1].SetActive (true);
			cur.cursors[1].SetActive (false);
		}
		if (type == CursorType.Select) {
			cur.cursors[0].SetActive (true);
			cur.cursors[1].SetActive (false);

			cursorRenderer[0].sharedMaterial = cur.materials[0];
		}
		if (type == CursorType.Move) {
			cur.cursors[1].SetActive (true);
			cur.cursors[0].SetActive (false);
			
			cursorRenderer[1].sharedMaterial = cur.materials[0];
		}
		if (type == CursorType.Attack) {
			cur.cursors[1].SetActive (true);
			cur.cursors[0].SetActive (false);
			
			cursorRenderer[1].sharedMaterial = cur.materials[1];
		}
		if (type == CursorType.AttackMove) {
			cur.cursors[1].SetActive (true);
			cur.cursors[0].SetActive (true);
			
			cursorRenderer[0].sharedMaterial = cur.materials[1];
			cursorRenderer[1].sharedMaterial = cur.materials[0];
		}

        if (type == CursorType.Defend) {
            cur.cursors[0].SetActive (true);
            cur.cursors[1].SetActive (false);
            cursorRenderer[0].sharedMaterial = cur.materials[2];
        }
    }
}
