using UnityEngine;
using System.Collections;

public class RotateRandom : MonoBehaviour {

		public Vector3 axis = Vector3.up;

	// Use this for initialization
	void Start () {
				transform.Rotate (axis * Random.Range (0, 360));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
