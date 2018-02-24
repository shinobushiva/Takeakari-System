using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ObjectLayer{
	
	[CustomEditor(typeof(LayeredObject))]
//	[CanEditMultipleObjects]
	public class LayeredObjectEditor : Editor {

		private List<string> _choices = new List<string>();
		private int _choiceIndex = 0;

		private void CreateList(ObjectLayerMaster.LayerDefinition[] ls, int depth){
			foreach (ObjectLayerMaster.LayerDefinition l in ls) {
				_choices.Add (l.name);
				CreateList (l.child, depth+1);
			}
		}


		public override void OnInspectorGUI()
		{
			ObjectLayerMaster master = FindObjectOfType<ObjectLayerMaster> ();
			_choices.Clear ();
			CreateList (master.layerDefinitions, 0);

			LayeredObject lo = target as LayeredObject;

			_choiceIndex = EditorGUILayout.Popup("Layer Name", _choices.IndexOf(lo.layerName), _choices.ToArray());
			lo.layerName = _choices[_choiceIndex];

			lo.visible = EditorGUILayout.ToggleLeft ("Visible", lo.visible);
			lo.collision = EditorGUILayout.ToggleLeft ("Collision", lo.collision);

			EditorUtility.SetDirty(target);
		}
	}
}