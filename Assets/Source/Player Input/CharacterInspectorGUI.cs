using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterInspectorGUI : MonoBehaviour {

	public Character character;
	public Transform inspectorCamera;
    public RawImage characterRender;
    public int renderSize = 200;

    private RenderTexture renderTexture;

    public InventoryGUI inventoryGUI;
    public Transform inventoryParent;
    public Transform[] equipmentSides;

    // Update is called once per frame
    void Update () {
        if (character) {
            inspectorCamera.transform.position = character.transform.position + character.transform.forward * 2.5f + Vector3.up * 1.2f;
            inspectorCamera.LookAt (character.transform.position + Vector3.up * 0.9f);

            if (Input.GetMouseButtonDown (1))
                Destroy ();
        }else {
            Destroy ();
        }
    }
    
    void Destroy () {
        Destroy (gameObject);
    }

	public static void InspectCharacter (Character c, Vector2 screenPos) {
        GameObject inspectorPrefab = Resources.Load<GameObject> ("GUI/CharacterInspector");
        GameObject newInspector = Instantiate (inspectorPrefab);

        CharacterInspectorGUI inspector = newInspector.GetComponent<CharacterInspectorGUI> ();
        inspector.transform.position = (Vector3)screenPos;

		inspector.character = c;
        inspector.Initialize ();
	}

    void Initialize () {
        transform.SetParent (GUIManager.mainCanvas.transform, true);
        Camera cam = inspectorCamera.GetComponent<Camera> ();
        renderTexture = new RenderTexture (renderSize, renderSize, 24);
        cam.gameObject.SetActive (true);
        cam.targetTexture = renderTexture;
        characterRender.texture = renderTexture;

        inventoryGUI.inventory = character.inventory;

        ShowInventory ();
        CreateEquipmentButtons ();
    }

    void ShowInventory () {
        if (inventoryGUI.inventory == null) {
            inventoryGUI.gameObject.SetActive (false);
            return;
        }

        inventoryGUI.CreateButtons (inventoryParent);
    }

    void CreateEquipmentButtons () {
        GameObject buttonPrefab = Resources.Load<GameObject> ("GUI/EquipmentButton");
        for (int i = 0; i < character.equipment.slots.Length; i++) {
            CharacterEquipment.Equipment cur = character.equipment.slots[i];
            GameObject newButton = Instantiate (buttonPrefab);

            CharacterInspectorButton butt = newButton.GetComponent<CharacterInspectorButton> ();

            butt.inspector = this;
            butt.defaultIcon = cur.defualtSlotImage;
            butt.button = newButton.GetComponent<Button> ();
            butt.image = newButton.GetComponentInChildren<RawImage> ();
            butt.text = newButton.GetComponentInChildren<Text> ();
            butt.equipment = cur;

            cur.item.inventoryButton = newButton;
               
            butt.button.onClick.AddListener (() => butt.OnClick());

            if (cur.side == CharacterEquipment.InspectorSide.Left) {
                butt.transform.SetParent (equipmentSides[0]);
            }else {
                butt.transform.SetParent (equipmentSides[1]);
            }
        }
    }

	void CloseWindow () {
		gameObject.SetActive (false);
	}
}
