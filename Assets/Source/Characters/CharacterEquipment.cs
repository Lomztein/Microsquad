using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterEquipment {

    public enum Type { Hand, Head, Chest, Legs, None };
    public enum InspectorSide {
        Left, Right, Top, Button
    }

    public Dictionary<string, Slot> slots = new Dictionary<string, Slot> ();

    [System.Serializable]
    public class Slot {

        public string name;
        public Type type;
        public Inventory.Slot item;
        public GameObject equippedItem;
        public Transform transform;

        public bool dropOnDeath;
        public bool spawnOnEquip;

        public InspectorSide side;
        public Texture defualtSlotImage;

        public Character character;

        public Slot(string _name, Type _type, bool _dropOnDeath, bool _spawnOnEquip, InspectorSide _side, Character _character) {
            name = _name; type = _type; dropOnDeath = _dropOnDeath; spawnOnEquip = _spawnOnEquip; side = _side; character = _character;
        }

        public virtual void Update() {
            if (equippedItem) {
                equippedItem.SendMessage ("OnUnEquip", new EquipMessage (character, "", this), SendMessageOptions.DontRequireReceiver);
                character.OnDeEquip ();
                Object.Destroy (equippedItem);
            }

            if (spawnOnEquip && item && item.item) {
                GameObject newTool = (GameObject)Object.Instantiate (item.item.prefab.gameObject, transform.position, transform.rotation);

                newTool.transform.position = transform.position;
                newTool.transform.rotation = transform.rotation;
                newTool.transform.parent = transform;

                equippedItem = newTool;

                newTool.SendMessage ("OnEquip", new EquipMessage (character, item.item.metadata, this));
                character.OnEquip (this, newTool);
            }
        }

        public IEnumerator Drop(float waitTime) {
            yield return new WaitForSeconds (waitTime);

            Rigidbody body = transform.GetComponentInParent<Rigidbody> ();
            GameObject pItem = PhysicalItem.Create (item.item, item.count, transform.position, transform.rotation).gameObject;
            Rigidbody drop = pItem.GetComponent<Rigidbody> ();

            drop.velocity = body.velocity;
            drop.angularVelocity = body.angularVelocity;

            item = null;
            Update ();
        }

        public struct EquipMessage {

            public Character character;
            public string metadata;
            public Slot slot;

            public EquipMessage(Character ch, string me, Slot sl) {
                character = ch;
                metadata = me;
                slot = sl;
            }

        }
    }

    public class Human : CharacterEquipment {

        public Human(Character character) {
            slots.Add ("Right Hand", Type.Head, true, true, InspectorSide.Left, character);
            Slot chest = new Slot ("Right Hand", Type.Chest, true, true, InspectorSide.Right, character);


            Slot rightHand = new Slot ("Right Hand", Type.Hand, true, true, InspectorSide.Right, character);
            Slot leftHand = new Slot ("Left Hand", Type.Hand, true, true, InspectorSide.Left, character);

        }


    }
}