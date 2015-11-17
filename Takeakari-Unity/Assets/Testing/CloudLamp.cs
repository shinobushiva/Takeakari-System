using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudLamp : MonoBehaviour {

    public Renderer lamp; 
    public Light light;

    public Color col;

    public bool intaractive = false;

    private List<Transform> sensors = new List<Transform>();

    public float fadeTime = 5f;
     
	// Use this for initialization
	void Start () {

        lamp.material.color = Color.black;
	
        Vector3 v = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        v = v.normalized;
        col = new Color(v.x, v.y, v.z);
	}

    private Color Clamp(Color c, Color col){
        c.r = Mathf.Min(c.r, col.r);
        c.g = Mathf.Min(c.g, col.g);
        c.b = Mathf.Min(c.b, col.b);
        c.a = Mathf.Min(c.a, col.a);

        return c;

    }
	
	// Update is called once per frame
	void Update () {
        
        Color c = lamp.material.color;

        if (intaractive)
        {

            float d = float.MaxValue;

            foreach (Transform t in sensors)
            {
                if (t != null)
                    d = Mathf.Min(Vector3.Distance(t.position, transform.position), d);
            }


            if (d < float.MaxValue)
            {
                //lamp.material.color = col;

                //d = d; 

                c = lamp.material.color + (col * Time.deltaTime) * fadeTime * d;
                c = Clamp(c, col);

            }
            {
                c = c - (col * Time.deltaTime * fadeTime); 
                c.r = Mathf.Clamp(c.r, 0f, 1f);
                c.g = Mathf.Clamp(c.g, 0f, 1f);
                c.b = Mathf.Clamp(c.b, 0f, 1f);
                c.a = 1;
                /*
                if(c.r < 0.05f && c.b < 0.05f && c.g < 0.05f){
                    c = Color.black;  
                }
                */
            }

          
        } else
        {
            c = col;
        }

        lamp.material.color = c ;
        light.transform.parent.GetComponent<Module>().LightColor = c;
        light.color = c;
	
	}

    public void SensorTriggerEnter(Transform t){

        if(!sensors.Contains(t))
            sensors.Add(t);

    }

    public void SensorTriggerExit(Transform t){

        sensors.Remove(t);
    }




}
