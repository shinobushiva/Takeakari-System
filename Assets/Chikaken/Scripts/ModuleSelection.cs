using UnityEngine;
using System.Collections;

public class ModuleSelection : MonoBehaviour {

	private Module module;
	private ColorChooser c;

	// Use this for initialization
	void Start () {
		module = GetComponent<Module>();
		module.name = module.name.Remove(0, 7);
		c = GameObject.FindObjectOfType<ColorChooser>();
	}
	
	// Update is called once per frame
	void Update () {
		if (isChecked && ControlPanel.Instance.applyImmidiate) {
			Color cc = c.GetColor();
			module.LightColor = cc;
			//module.light.color = cc;
			//module.light.intensity = cc.a;
		}
	}

	public bool isChecked;

	void OnGUI(){
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

		float s = Mathf.Max (Screen.width*0.1f, Screen.height*0.1f);

		Rect r = new Rect(pos.x-20, Screen.height-pos.y, 20, 20);


		GUIStyle style = new GUIStyle();
		style.clipping = TextClipping.Overflow;
		style.normal.textColor = Color.white;

		bool b = GUI.Toggle(r, isChecked, "");
		if(b != isChecked){
			isChecked = b;
		}
		r.x -=20;
		r.y +=20;
		GUI.Label(r, module.gameObject.name, style);

	}
}
