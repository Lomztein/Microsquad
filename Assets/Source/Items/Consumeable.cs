using UnityEngine;
using System.Collections;

public class Consumeable : MonoBehaviour {

	public virtual void Consume (Character consumingCharacter) {
        Destroy (gameObject);
    }
}
