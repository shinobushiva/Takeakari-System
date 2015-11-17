using UnityEngine;
using System.Collections;

public class CloudLampSensor : MonoBehaviour {

    public CloudLamp cloudLamp;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider c){

        cloudLamp.SensorTriggerEnter(c.transform);

    }

    void OnTriggerExit(Collider c){
        cloudLamp.SensorTriggerExit(c.transform);

    }
}
