using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class FollowingUIComponent : MonoBehaviour {

    private RectTransform rect;
    private Text text;

    public Transform target;

	// Use this for initialization
	void Start () {
        rect = gameObject.AddComponent<RectTransform>();
        text = gameObject.AddComponent<Text>();
        text.text = name;
	}
	
	// Update is called once per frame
	void LateUpdate () {

        transform.position = target.position;
		
	}
}
