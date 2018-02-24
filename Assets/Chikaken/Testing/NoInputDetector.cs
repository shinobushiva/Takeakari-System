using UnityEngine;
using System.Collections;

public class NoInputDetector : SingletonMonoBehaviour<NoInputDetector> {

    float lastTime;

    public float timeToWait = 300;

    public string backTo;



	// Use this for initialization
	void Start () {
        lastTime = Time.time;

	
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > lastTime + timeToWait)
        { 
            Application.LoadLevel(backTo);
        }
	
	}

    public void Detect(){
        lastTime = Time.time;
    }
}
