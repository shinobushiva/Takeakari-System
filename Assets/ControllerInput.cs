using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour {

    public CloudLamp lamp1;
    public CloudLamp lamp2;

    public Color[] colors = new Color[]{Color.red};

    private int currentColor = 0;

	// Use this for initialization
	void Start () {
		
	}

    private Color GetColor(int nextBack){
        
        currentColor = (currentColor + nextBack +colors.Length)%colors.Length; 
        return colors[currentColor];
        
    }


	
	// Update is called once per frame
	void Update () {

        float h = Input.GetAxis ("Horizontal"); 
        float v = Input.GetAxis ("Vertical"); 

        if (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)
        {
            print("h=" + h + ",v=" + v);
        }

        if (Input.GetButton("Fire1"))
        {
            lamp1.col = GetColor((int)h);
        } else
        {
            lamp1.col = Color.black;
        }

        if (Input.GetButton("Fire2"))
        {
            lamp2.col = GetColor((int)h);
        } else
        {
            lamp2.col = Color.black;
        }
           
		
	}
}
