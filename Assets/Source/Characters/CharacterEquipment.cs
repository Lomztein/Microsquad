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
        public Inventory.Slot slot;
        public GameObject equippedItem;
        public Transform transform;

        public bool dropOnDeath;
        public bool spawnOnEquip;

        public InspectorSide side;
        public Texture defualtSlotImage;

        public Character character;

        public Slot(string _name, Type _type, bool _dropOnDeath, bool _spawnOnEquip, InspectorSide _side, Character _character) {
            name = _name; type = _type; dropOnDeath = _dropOnDeath; spawnOnEquip = _spawnOnEquip; side = _side; character = _character;
            slot = Inventory.Slot.CreateSlot ();
        }

        public virtual void Update() {
            if (equippedItem) {
                equippedItem.SendMessage ("OnUnEquip", new EquipMessage (character, "", this, null), SendMessageOptions.DontRequireReceiver);
                character.OnDeEquip ();
                Object.Destroy (equippedItem);
            }

            if (spawnOnEquip && slot && slot.item) {
                IEquipable equipable = slot.item.prefab as IEquipable;
                GameObject newTool = Object.Instantiate (equipable.GetEquipmentObject (), transform.position, transform.rotation);

                newTool.transform.position = transform.position;
                newTool.transform.rotation = transform.rotation;
                newTool.transform.parent = transform;

                equippedItem = newTool;

                newTool.SendMessage ("OnEquip", new EquipMessage (character, slot.item.metadata, this, slot.item.attributes));
                character.OnEquip (this, newTool);
            }
        }

        public IEnumerator Drop(float waitTime) {
            yield return new WaitForSeconds (waitTime);

            Rigidbody body = transform.GetComponentInParent<Rigidbody> ();
            GameObject pItem = PhysicalItem.Create (slot.item, slot.count, transform.position, transform.rotation).gameObject;
            Rigidbody drop = pItem.GetComponent<Rigidbody> ();

            drop.velocity = body.velocity;
            drop.angularVelocity = body.angularVelocity;

            slot = null;
            Update ();
        }

        public struct EquipMessage {

            public Character character;
            public string metadata;
            public Slot slot;
            public ObjectAttribute attributes;

            public EquipMessage(Character ch, string me, Slot sl, ObjectAttribute _attributes) {
                character = ch;
                metadata = me;
                slot = sl;
                attributes = _attributes;
            }

        }
    }

    [System.Serializable]
    public class Human : CharacterEquipment {

        public Slot rightHand = new Slot("Right Hand", Type.Hand, true, true, InspectorSide.Left, null);
        public Slot leftHand = new Slot ("Left Hand", Type.Hand, true, true, InspectorSide.Left, null);
        public Slot head = new Slot ("Head", Type.Head, true, true, InspectorSide.Right, null);
        public Slot chest = new Slot ("Chest", Type.Chest, true, true, InspectorSide.Right, null);
        public Slot legs = new Slot ("Legs", Type.Legs, true, true, InspectorSide.Right, null);

        public Human(Character character) {
            slots.Add (rightHand.name, rightHand);
            slots.Add (leftHand.name, leftHand);
            slots.Add (head.name, head);
            slots.Add (chest.name, chest);
            slots.Add (legs.name, legs);

            foreach (var value in slots.Values) {
                value.character = character;
                value.transform = character.transform.Find ("Rig").Find ("hips");
            }
        }
    }
}