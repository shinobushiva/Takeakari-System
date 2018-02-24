using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Shiva.CameraSwitch;

public class CameraPlugin : Editor {

	[MenuItem("Shiva's/Camera/SyncSceneViewWithCurrentActiveCamera")]
	public static void SyncCamera(){
		Camera c = GameObject.FindObjectOfType<CameraSwitcher> ().CurrentActive.c;
		Debug.Log (c);

		SceneView.lastActiveSceneView.AlignViewToObject(c.transform);
	}
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
