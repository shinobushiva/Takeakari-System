using UnityEngine;
using System.Collections;
using System.Linq;

public class ModuleDistanceColor : MonoBehaviour
{

    public  Transform target;
    public float maxDistance = 10f;
    private Module module;

    // Use this for initialization
    void Start()
    {
        module = GetComponent<Module>();
    }
    
    // Update is called once per frame
    void Update()
    {

        IndexFingerTouch[] pp = FindObjectsOfType<IndexFingerTouch>();
        if (pp.Length <= 0)
        {
            module.LightColor = module.light.color/2;
            return;
        }

        IndexFingerTouch p = pp.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToArray()[0];

        Vector3 p1 = transform.position;
        Vector3 p2 = p.transform.position;

        float dist = Vector3.Distance(p1, p2);

        //print(dist);
        if (dist > maxDistance)
        {
            module.LightColor = Color.black;
        } else
        {
            Color c = FindObjectOfType<ColorChooser>().GetColor();
            module.LightColor = Color.Lerp(Color.black, c, (1 - dist / maxDistance)*(1 - dist / maxDistance));
        }


    
    }
}
