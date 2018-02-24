using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class FollowingUI : MonoBehaviour {

    public Dictionary<Transform, FollowingUIComponent> map = new Dictionary<Transform, FollowingUIComponent>();

	// Use this for initialization
	void Start () {
		
	}
	

    public void Add(Transform target, FollowingUIComponent comp){
        map.Add(target, comp);
        comp.transform.SetParent(transform);
        comp.target = target;
    }
}
