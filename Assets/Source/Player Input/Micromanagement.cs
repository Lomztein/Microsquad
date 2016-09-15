using UnityEngine;
using System.Collections;

public class Micromanagement : MonoBehaviour {

	public static Vector3[] GetSpriralPositions (float unitSize, int unitAmount) {
		int placed = 0;
		Vector3[] positions = new Vector3[unitAmount];
		float radius = 0f;

		while (placed < unitAmount) {

			int toPlace = Mathf.FloorToInt (Mathf.Max (radius * 2f * Mathf.PI / unitSize, 1f));
			toPlace = Mathf.Min (toPlace, positions.Length - placed);

			for (int i = 0; i < toPlace; i++) {
				if (placed < positions.Length) {
					positions[placed] = Quaternion.Euler (0f, ((float)i / (float)toPlace) * 360f, 0f) * Vector3.forward * radius;
					placed++;
				}
			}

			radius += unitSize;
		}
		return positions;
	}

    public static void OrderInteraction (Transform interactionObject, string command) {
        if (PlayerInput.selectedUnits.Count > 0) {
            // Find available member, one without any commands.
            Squadmember member = PlayerInput.selectedUnits[0];
            for (int i = 0; i < PlayerInput.selectedUnits.Count; i++) {
                if (PlayerInput.selectedUnits[i].ai.commands.Count == 0) {
                    member = PlayerInput.selectedUnits[i];
                }
            }

            Command.InteractCommand (interactionObject, member.ai, command);
        } else {
            Game.AddMessage ("A soldier must be selected to interact with something.");
        }
    }
}
