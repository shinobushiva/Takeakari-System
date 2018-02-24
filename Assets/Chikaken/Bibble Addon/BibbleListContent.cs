using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BibbleListContent : MonoBehaviour {

    public Text uuid;
    public Text distance;
    public Text button;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Set(string uuid, float distance, int button){
        this.uuid.text = uuid;
        if (distance > 0)
        {
            this.distance.text = ""+distance;
        }
        if (button > 0)
        {
            this.button.text = " " + button +"@"+System.DateTime.Now;
        }
    }
}
