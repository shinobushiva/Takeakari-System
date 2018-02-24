using UnityEngine;
using System.Collections;

public class MousePointer : MonoBehaviour
{
    GameObject currentHit;

    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void Update()
    {
    
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (currentHit != hit.transform.gameObject)
            {

                if (currentHit != null)
                {
                    currentHit.SendMessage("UnPointed", SendMessageOptions.DontRequireReceiver);
                }
                
                currentHit = hit.transform.gameObject;
                currentHit.SendMessage("Pointed", SendMessageOptions.DontRequireReceiver);


            }


            Debug.DrawLine(ray.origin, hit.point);
        } else {
            if (currentHit != null)
            {
                currentHit.SendMessage("UnPointed", SendMessageOptions.DontRequireReceiver);
                currentHit = null;
            }

        }
    
    }
}
