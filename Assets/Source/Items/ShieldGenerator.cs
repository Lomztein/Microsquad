using UnityEngine;
using System.Collections;

public class ShieldGenerator : MonoBehaviour {

    public int shieldHealth;
    public float range;

    public Transform shieldTransform;

    public Faction faction;

    void Start () {
        shieldTransform.localScale = Vector3.one * range;
        shieldTransform.gameObject.layer = Game.FactionLayerIndex (faction);
    }

	public void OnTakeDamage (Damage d) {
        shieldHealth -= d.damage;
        if (shieldHealth <= 0) {
            Destroy (gameObject);
        }
    }
}
