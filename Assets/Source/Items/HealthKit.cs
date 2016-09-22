using UnityEngine;
using System.Collections;

public class HealthKit : Consumeable {

    public enum HealType { Constant, Percentage };

    public int healAmount;
    public HealType healType;

    public override void Consume (Character consumingCharacter) {
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
        base.Consume (consumingCharacter);
    }

    /*IEnumerator ConsumeHealth ( Character consumingCharacter ) {
        float ticks = healTime / PlayerInput.defaultFixedDeltaTime;
        float healPerTick = healAmount / ticks;

        for (int i = 0; i < ticks; i++) {
            consumingCharacter.OnTakeDamage (new Damage (healPerTick))
        }
    }*/
}
