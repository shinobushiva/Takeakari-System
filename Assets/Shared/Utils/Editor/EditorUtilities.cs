using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EditorUtilities : Editor {

	[MenuItem("Custom/Utilities/Add Mesh Collider")]
	public static void AddMeshCollider(){
		GameObject selected = Selection.activeGameObject;
		MeshFilter[] mfs = selected.GetComponentsInChildren<MeshFilter> ();
		foreach (MeshFilter mf in mfs) {
			if(!mf.gameObject.GetComponent<Collider>())
				mf.gameObject.AddComponent<MeshCollider>();
		}
	}

	[MenuItem("Custom/Utilities/Create Bounding Cube")]
	public static void CreateBoundingCube(){
		GameObject selected = Selection.activeGameObject;
		Bounds b1 = Helper.GetBoundingBox (selected);
		Quaternion rot = selected.transform.rotation;
		selected.transform.rotation = Quaternion.identity;
		Bounds b = Helper.GetBoundingBox (selected);
		selected.transform.rotation = rot;

		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		cube.transform.localScale = b.extents*2;
		cube.transform.position = b1.center;
		cube.transform.rotation = rot;
		cube.name = selected.name+"_BBox";

		Material m = null;

		string[] ass = AssetDatabase.FindAssets ("Wireframe");
		foreach (string a in ass) {
			Debug.Log(a);
			string path = AssetDatabase.GUIDToAssetPath(a);
			Debug.Log (path);
			if(path.EndsWith(".mat")){
				m = AssetDatabase.LoadAssetAtPath<Material>(path);
				break;
			}
		}
		cube.GetComponent<Renderer> ().sharedMaterial = m;
		if (selected.transform.parent != null) {
			cube.transform.SetParent(selected.transform.parent, true);
		}


	}

	[MenuItem("Custom/Utilities/Remove Collider")]
	public static void RemoteCollider(){
		GameObject selected = Selection.activeGameObject;
		Collider[] mfs = selected.GetComponentsInChildren<Collider> ();
		foreach (Collider mf in mfs) {
			DestroyImmediate(mf);
		}
	}

	[MenuItem("Custom/Utilities/Fix Combined Mesh")]
	public static void FixCombinedMesh(){
		int count = 0;
		GameObject selected = Selection.activeGameObject;

		MeshFilter[] mfs = selected.GetComponentsInChildren<MeshFilter> ();
		foreach (MeshFilter mf in mfs) {
			if (mf.sharedMesh == null) {
				Debug.Log ("No mesh set for " + mf.gameObject);
				DestroyImmediate (mf.gameObject);
				count++;
			} else if(mf.sharedMesh.name.StartsWith("Combined Mesh")){
				MeshCollider mc = mf.gameObject.GetComponent<MeshCollider> ();
				if (mc) {
					Debug.Log ("Fix Mesh for " + mf.gameObject.name);
					mf.sharedMesh = mc.sharedMesh;
				}
			}
		}

		Debug.Log ("" + count + " objects were removed");
	}
}
