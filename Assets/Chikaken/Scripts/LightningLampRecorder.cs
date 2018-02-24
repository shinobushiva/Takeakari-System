using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class LightningLampRecorder : MonoBehaviour {

    private CloudLamp lamp;

    public Color c;

    public float max = 10f;

	// Use this for initialization
	void Start () {
        lamp = GetComponent<CloudLamp>();
        lastMoveTime = Time.time;

        StartCoroutine(Routine());


	 
	}

    private Vector3 prevPos;

    public float lastMoveTime = 0;
    private float startTime = 0;

    private bool moving = false;

    string data = "";

    IEnumerator Routine(){

        Encoding ec = Encoding.UTF8;
        StreamWriter sw = new StreamWriter("/Users/shiva/tmp/data.csv", false, ec);

        while (true)
        {
            Vector3 pos = Input.mousePosition;
        
            float d = Vector3.Distance(pos, prevPos);
            if (d > 0)
            {
                lastMoveTime = Time.time;
                if(!moving)
                    startTime = Time.time;
                moving = true;
            }
        
            if (lastMoveTime + .5f < Time.time)
            {
                if (moving)
                {
                    moving = false;
                
                    //write out
                    print(data);
                    sw.WriteLine(data);
                    sw.Flush();
                
                    data = "";
                }
                //no move
            } else
            { 
                data += "" + (Time.time - startTime) + "," + d + ",";
            }
        
        
            Color cc = c * (d / max);
            lamp.col = cc;
            // XXX:なんだろう？
            // lamp.light.color = cc;

            prevPos = Input.mousePosition;

            yield return new WaitForSeconds(0.05f);
        }

    }
	
	// Update is called once per frame
	void Update () {


	
	}
}
