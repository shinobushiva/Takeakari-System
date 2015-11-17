using UnityEngine;
using System.Collections;

public class RandomMove : MonoBehaviour {

    public Vector3 range;
    Vector3 org;

    void Reset(){
        org = transform.position;
    }

	// Use this for initialization
	void Start () {
        org = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = org + new Vector3(Random.Range(0, range.x)-range.x/2, 0, Random.Range(0, range.z)-range.z/2);
	}

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(org, range);
    }
}
