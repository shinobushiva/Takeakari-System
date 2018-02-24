using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class CloudLampGUI : MonoBehaviour {
    
    public int size = 20;
    public Vector2 offset = Vector2.zero;

	// Use this for initialization
	void Start () {
//        FollowingUI fui = FindObjectOfType<FollowingUI>();
//        if (fui == null)
//        {
//            fui = new GameObject("FollowingUI").AddComponent<FollowingUI>();
//            fui.gameObject.AddComponent<Canvas>();
//        }
//
//        GameObject go = new GameObject("FollowingComponent - " + name);
//        FollowingUIComponent fuicomp = go.AddComponent<FollowingUIComponent>();
//        fui.Add(transform, fuicomp);
	
	}
	
    void OnGUI(){
        
        Vector3 v3 = Camera.main.WorldToScreenPoint(transform.position);
        v3.y = Screen.height - v3.y;

        string str = "Dodecahedron-";
        int idx = gameObject.name.IndexOf(str)+str.Length;
        str = gameObject.name.Substring(idx);

        
        GUI.Label(new Rect(v3.x-offset.x-size/2, v3.y-offset.y-size/2, size, size), str);
        
        
    }
}
