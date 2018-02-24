using UnityEngine;
using UnityEditor;
using System.Collections;
using Shiva.Utils;

public class ModelPacement : EditorWindow {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	[MenuItem ("Custom/Placement/Model Placement")]
	static void ShowWindow () {
		EditorWindow.GetWindow<ModelReplacer>();
	}

	private Transform replaceObject;
	private bool yUp = true;
	private Vector3 rotation;

	private GameObject[] trees;

	void OnGUI () {
		
		replaceObject = EditorGUILayout.ObjectField (
			"Replace Object", replaceObject, typeof(Transform)) as Transform;
		yUp = EditorGUILayout.Toggle("Change Y-up to Z-up", yUp);
		rotation = EditorGUILayout.Vector3Field ("Rotation", rotation);

		GameObject go = Selection.activeGameObject;
		if (go == null)
			return;
		
		Transform[] ts = go.transform.GetComponentsInChildren<Transform> ();
		GUILayout.Label (""+ts.Length+" selected", EditorStyles.boldLabel);

		if (GUILayout.Button ("Place")) {

//			Undo.RecordObject (go, "Replacing Object");

			GameObject par = new GameObject(go.transform.name + "-Replaced");
			par.transform.SetParent (go.transform.parent, true);
			par.transform.localScale = Vector3.one;
			par.transform.rotation = go.transform.rotation;
			par.transform.position = go.transform.position;

			foreach(Transform t in ts){
				if (t == go.transform)
					continue;
				
				Transform tt = Instantiate<Transform> (replaceObject);
				tt.SetParent (par.transform, true);
				tt.rotation = t.rotation;
				tt.position = t.position;
				if (yUp) {
					tt.Rotate (Vector3.right * 90f, Space.Self);
				}
				Vector3 ang = tt.eulerAngles + rotation;
				tt.eulerAngles = ang;
			}
			go.SetActive (false);
		}
		
//		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
//		myBool = EditorGUILayout.Toggle ("Toggle", myBool);
//		myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
//		EditorGUILayout.EndToggleGroup ();
	}
}
