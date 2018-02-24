using UnityEngine;
using System.Collections;

public class OnModuleTouch : MonoBehaviour {

	private ModuleSelection ms;

	// Use this for initialization
	void Start () {
		ms = GetComponentInParent<ModuleSelection>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseDown(){
		ms.isChecked = !ms.isChecked;
	}
}
