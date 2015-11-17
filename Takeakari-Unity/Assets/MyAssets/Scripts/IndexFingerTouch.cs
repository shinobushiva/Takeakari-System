using UnityEngine;
using System.Collections;

public class IndexFingerTouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider c)
    {

        //print("Enter:" + c.gameObject.name);
        
        if (c.gameObject.GetComponent<VideoPlayback>())
        {
            //print("Pointed");
            c.gameObject.GetComponent<VideoPlayback>().Pointed();
        }

        if (c.gameObject.GetComponent<ColorPanel>())
        {
            c.gameObject.GetComponent<ColorPanel>().Pointed( transform.position);
        }
        
    }
    
    void OnTriggerStay(Collider c)
    {
        //print("Stay:" + c.gameObject.name);
    }
    
    void OnTriggerExit(Collider c)
    {
        //print("Exit:" + c.gameObject.name);
        
        if (c.gameObject.GetComponent<VideoPlayback>())
        {
            //print("UnPointed");
            c.gameObject.GetComponent<VideoPlayback>().UnPointed();
        }
    }
}
