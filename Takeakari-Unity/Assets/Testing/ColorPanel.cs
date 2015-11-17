using UnityEngine;
using System.Collections;

public class ColorPanel : MonoBehaviour {

    
    public Texture2D colorTex;

    public Renderer colorIndicator;

	// Use this for initialization
	void Start () { 
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Pointed(Vector3 pos){
        print("Color:" + pos);

        float w2 = GetComponent<Collider>().bounds.extents.x - GetComponent<Collider>().bounds.center.x;
        float h2 = GetComponent<Collider>().bounds.extents.y - GetComponent<Collider>().bounds.center.y;
        
        float x = (pos.x + w2)/(GetComponent<Collider>().bounds.extents.x*2);
        float y = (pos.y + h2)/(GetComponent<Collider>().bounds.extents.y*2); 
        
        Color c = colorTex.GetPixel((int)(colorTex.width*x), (int)(colorTex.height*y));
        colorIndicator.material.color = c;
        ColorChooser.Instance.setColor(c);
    }
}
