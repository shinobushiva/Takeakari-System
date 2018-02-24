using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ObjectLayer{
	public class ObjectLayerEntry : MonoBehaviour {

		public Text layerText;
		public Toggle toggle;
		
		private string layerName;
		private ObjectLayerMaster master;

		public RectTransform childrenPanel;

 
		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void Dispose(){

		}

		public void Load(){

		}

		public void AddChild(ObjectLayerEntry e){
			e.gameObject.transform.SetParent (childrenPanel.transform);
		}

		public void Set(string layerName, ObjectLayerMaster master){
			layerText.text = layerName;
			this.layerName = layerName;
			this.master = master;
		}

		public void SetVisible(Toggle t){
			master.SetLayerVisible (t.isOn, layerName);
		}
	}
}