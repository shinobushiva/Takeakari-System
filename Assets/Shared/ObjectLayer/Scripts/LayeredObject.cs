using UnityEngine;
using System.Collections;

namespace ObjectLayer {
	public class LayeredObject : MonoBehaviour {
		
		public string layerName;

		private ObjectLayerMaster master;

		private int orgLayer;

		public bool visible = true;

		public bool collision = true;

		private int invisibleLayer;

		void Awake(){
			invisibleLayer = LayerMask.NameToLayer ("Invisible");
		
			if (!visible)
				SetVisible (false);

			master = FindObjectOfType<ObjectLayerMaster> ();
			master.AddObject (this);
		}


		public void SetVisible(bool b){
			visible = b;

			if (b) {
				gameObject.SetActive (b);
				gameObject.transform.SetLayer(orgLayer);
			} else {
				if(gameObject.layer != invisibleLayer){
					orgLayer = gameObject.layer;
					gameObject.transform.SetLayer(invisibleLayer);
				}
				gameObject.SetActive (b);
			}
		}

		void OnDestroy(){
			master.RemoveObject (this);
		}
	}
}
