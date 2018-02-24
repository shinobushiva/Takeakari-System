using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChooser : MonoBehaviour {

    private Color color;
	
    // Use this for initialization
	void Start () {
		
	}
	

    public void SetColor(Color c){
        this.color = c;
    }

    public Color GetColor(){
        return this.color;
    }
}
