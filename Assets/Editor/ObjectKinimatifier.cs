using UnityEngine;
using System.Collections;
using UnityEditor;

public class ObjectKinimatifier : EditorWindow {

	public GameObject currentObject;
    public bool setting;

	[MenuItem ("Microsquad/Object Kinimatifer")]
	public static void ShowWindow () {
		EditorWindow.GetWindow (typeof(ObjectKinimatifier));
	}

	void OnGUI () {
        currentObject = (GameObject)EditorGUILayout.ObjectField (currentObject, typeof (GameObject), false);
        setting = EditorGUILayout.Toggle ("Setting: ", setting);
        if (currentObject && GUILayout.Button ("Kinimatifize")) {
            Rigidbody[] bodies = currentObject.GetComponentsInChildren<Rigidbody> ();
            for (int i = 0; i < bodies.Length; i++) {
                bodies[i].isKinematic = setting;
                bodies[i].GetComponent<Collider> ().enabled = !setting;
            }
        }
	}
}
