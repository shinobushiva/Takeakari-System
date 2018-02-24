using UnityEngine;
using System.Collections;

public class PointerPlace : MonoBehaviour {

    public Transform target;

    public Texture2D colorTex;

    public Camera tc;

	// Use this for initialization
	void Start () {
	if (tc == null)
        {
            tc = Camera.main;
        }
	}
	
	// Update is called once per frame
	void Update () {

        Ray ray = tc.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.cyan);

            if(hit.transform.gameObject.tag =="PointerWall"){
                transform.position = hit.point;
            }

            if(hit.transform.gameObject.tag == "ColorChooser"){
                transform.position = hit.point;
                 

                float w2 = hit.transform.GetComponent<Collider>().bounds.extents.x - hit.transform.GetComponent<Collider>().bounds.center.x;
                float h2 = hit.transform.GetComponent<Collider>().bounds.extents.y - hit.transform.GetComponent<Collider>().bounds.center.y;

                float x = (hit.point.x + w2)/(w2*2);
                float y = (hit.point.y + h2)/(hit.transform.GetComponent<Collider>().bounds.extents.y*2); 

                Color c = colorTex.GetPixel((int)(colorTex.width*x), (int)(colorTex.height*y));
                GetComponent<Renderer>().material.color = c;
                FindObjectOfType<ColorChooser>().SetColor(c);

                //print(new Vector2(x, y));

                 
            }

        }




	
	}
}
