using UnityEngine;
using System.Collections;

public class GUIHidder : MonoBehaviour
{
    private Canvas canvas;

    // Use this for initialization
    void Start()
    {
        canvas = GetComponent<Canvas>();
    
    }

    public float hideTime = 2;
    private float lastMouseTime = 0;
    private Vector3 lastMousePosition; 
    
    // Update is called once per frame
    void Update()
    {

        Vector3 p = Input.mousePosition;

        float d = Vector3.Distance(lastMousePosition, p);
         
        if (d >= 0.1f)
        {
            canvas.enabled = true;
            lastMouseTime = Time.time;

        } else if((Time.time - lastMouseTime) > hideTime){
            canvas.enabled = false;
        }

        lastMousePosition = Input.mousePosition;

    }
}
