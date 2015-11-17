using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CloudLampGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI(){
        
        Vector3 v3 = Camera.main.WorldToScreenPoint(transform.position);
        v3.y = Screen.height - v3.y;

        string str = "Dodecahedron-";
        int idx = gameObject.name.IndexOf(str)+str.Length;
        
        GUI.Label(new Rect(v3.x-20, v3.y-20, 40, 40), gameObject.name.Substring(idx));
        
        
    }
}
