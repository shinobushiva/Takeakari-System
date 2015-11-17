using UnityEngine;
using System.Collections;

public class PinchHandInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(NoInputDetector.Instance != null)
            NoInputDetector.Instance.Detect();
	
	}
}
