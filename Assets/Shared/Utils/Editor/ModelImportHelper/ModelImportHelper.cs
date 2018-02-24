using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ModelImportHelper : Editor {

	private static string objectNameToAlign = "Plane001";
//	private static string[] objectNamesToDelete = {"g_3dboard","Plane001"};

	[MenuItem("Custom/Align Models")]
	public static void AlignModels(){
		Debug.Log ("Align Models");

		Transform[] transformes = GameObject.FindObjectsOfType<Transform> ();
		foreach (Transform trans in transformes) {
			if(trans.name == ModelImportHelper.objectNameToAlign){
				Vector3 pos = trans.position;
				Transform root = trans.parent;
				root.Translate(-pos);
			}
//			if(objectNamesToDelete.Contains(trans.name)){
//				DestroyImmediate(trans.gameObject);
//			}
		}

	}

	[MenuItem("Custom/Navmesh/Set Not Walkable")]
	public static void SetNotWalkable(){
		GameObject selected = Selection.activeGameObject;
//		MeshFilter[] mfs = selected.GetComponentsInChildren<MeshFilter> ();
//		foreach (MeshFilter mf in mfs) {
//			if(!mf.gameObject.GetComponent<Collider>())
//				mf.gameObject.AddComponent<MeshCollider>();
//		}
	}

}
