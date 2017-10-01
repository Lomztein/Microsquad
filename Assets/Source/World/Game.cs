using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	[Header ("Game Information")]
	public Squad currentSquad;
    public static string commanderName = "The Commander";
    public Color[] itemRarityColors;
    public Material[] itemRarityMaterials;

	[Header ("References")]
	public static Game game;

	[Header ("Prefabs")]
	public GameObject physicalItemPrefab;

	[Header ("Layer Masks")]
	public LayerMask all;
    public LayerMask[] layers;
    public int[] layerIndexes;
	public Color[] factionColors;
    public Material[] factionColorMaterials;
    public LayerMask deadCharacter;

	public LayerMask terrainLayer;
	public LayerMask groundLayer;
    public int terrainLayerIndex;
    public int groundLayerIndex;

    [Header ("Settings")]
	public static float soundVolume = 1f;
	public static float musicVolume = 1f;

	[Header ("Messages")]
	public static List<string> messages = new List<string>();
	public static List<float> messageTimes = new List<float>();

	public static float messageTime = 5f;
	public static int maxMessages = 10;

	public static LayerMask FactionLayer (Faction faction) {
		return game.layers[(int)faction];
	}

	public static int FactionLayerIndex (Faction faction) {
		return game.layerIndexes[(int)faction];
	}

	public static Color FactionColor (Faction faction) {
		return game.factionColors[(int)faction];
	}

	public static Material FactionMaterial (Faction faction) {
		return game.factionColorMaterials[(int)faction];
	}

    public static Color RarityColor (ItemPrefab.Rarity rarity) {
        return game.itemRarityColors[(int)rarity];
    }

    public static Material RarityMaterial (ItemPrefab.Rarity rarity) {
        return game.itemRarityMaterials[(int)rarity];
    }

	public static void AddMessage (string message) {
		messages.Add (message);
		messageTimes.Add (messageTime);
		if (messages.Count > maxMessages)
			messages.RemoveAt (messages.Count - 1);

        Debug.Log ("Message: " + message);
	}

	public void SavePreferences () {
		PlayerPrefs.SetFloat ("fSoundVolume", soundVolume);
		PlayerPrefs.SetFloat ("fMusicVolume", musicVolume);
	}

	void FixedUpdate () {
		for (int i = 0; i < messages.Count; i++) {
			messageTimes[i] -= Time.deltaTime;

			if (messageTimes[i] < 0f) {
				messages.RemoveAt (i);
				messageTimes.RemoveAt (i);
			}
		}
	}

	void Awake () {
		game = this;
		SavePreferences ();
        DialogGUI.RestartCoroutine (IntroDialog ());
	}

    IEnumerator IntroDialog () {
        yield return DialogGUI.RunDialog ("MICROSQUAD PROTOTYPE", "Hello, and welcome to the test area of Project Microsquad.", "This area is used to test out features in a manner outside of the game, for easier testing.");
    }

	void OnGUI () {
		for (int i = 0; i < messages.Count; i++) {
			GUI.Label (new Rect (Screen.width - 200, 10 + 20f * i, 200, 20f), messages[i]);
		}
	}

    public void Restart () {
        RestartScene ();
    }

    public void Quit () {
        QuitToDesktop ();
    }

    public static void RestartScene () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }

    public static void QuitToDesktop () {
        Application.Quit ();
    }
}
