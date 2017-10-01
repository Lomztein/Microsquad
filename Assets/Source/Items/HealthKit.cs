using UnityEngine;
using System.Collections;

public class HealthKit : ItemPrefab, IConsumeable {

    public enum HealType { Constant, Percentage };

    public int healAmount;
    public HealType healType;

    public void Consume (Character consumingCharacter) {
        int heal = 0;
        switch (healType) {
            case HealType.Constant:
                heal = healAmount;
                break;

            case HealType.Percentage:
                heal = healAmount * consumingCharacter.maxHealth;
                break;
        }
        consumingCharacter.SendMessage ("OnTakeDamage", (new Damage (-heal, Vector3.zero, null, Vector3.zero, 0.0f)));
    }
}
