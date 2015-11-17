using UnityEngine;
using System.Collections;

public class CubeRain : MonoBehaviour {

    public GameObject[] cubes;

    public Transform place;

	// Use this for initialization
	void Start () {

        StartCoroutine(Create());
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Create(){

        for (int i=0; i<50; i++)
        {
            GameObject go = (GameObject)Instantiate(cubes[Random.Range(0, cubes.Length)], place.position, Quaternion.identity);
            Vector3 s = go.transform.localScale;
            s.x = Random.Range(s.x/2f, s.x*3f);
            s.y = Random.Range(s.y/2f, s.y*3f);
            s.z = Random.Range(s.z/2f, s.z*3f);
            go.transform.localScale = s;

            yield return new WaitForFixedUpdate();
        }

        while (true)
        {
            GameObject go = (GameObject)Instantiate(cubes[Random.Range(0, cubes.Length)], place.position, Quaternion.identity);
            Vector3 s = go.transform.localScale;
            s.x = Random.Range(s.x/3f, s.x*2f);
            s.y = Random.Range(s.y/3f, s.y*2f);
            s.z = Random.Range(s.z/3f, s.z*2f);
            go.transform.localScale = s;

            yield return new WaitForSeconds(Random.Range(2f, 5f) );
        }

    }
}
