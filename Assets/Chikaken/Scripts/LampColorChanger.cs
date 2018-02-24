 using UnityEngine;
using System.Collections;

public class LampColorChanger : MonoBehaviour {


    private float nextRotationTime = 0f;
    private Color currentColor; 
    private Color nextColor;
    public Color oneColor = Color.blue;

    public float minRadius;
    public float maxRadius;

    public ModuleArray ma;

    public float randomFactor;
    public float offsetSinDegree = 30f;

    public float speed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Module[] ms = ma.modules;
        foreach (Module m in ms){
            if( Vector3.Distance(m.transform.position, transform.position) < GetComponent<Collider>().bounds.extents.magnitude){
                CloudLamp cl = m.GetComponent<CloudLamp>();
                if(cl)
                    cl.col = Color.Lerp(cl.col, oneColor, Time.deltaTime); 
            }
            else{
                CloudLamp cl = m.GetComponent<CloudLamp>();
                if(cl)
                    cl.col =  Color.Lerp(cl.col, cl.col*.1f, Time.deltaTime/3);
            }
        }

        randomFactor = Random.value;
        transform.localScale = Vector3.one * Mathf.Lerp(minRadius, maxRadius, Mathf.Sin(Time.time * ControlPanel.Instance.Speed * speed + offsetSinDegree));
	}

}
