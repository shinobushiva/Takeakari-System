using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour 
{	
	
	// use SetBar anywhere between 0 and 1
	public void SetBar( float v )
	{
		float offset = 0f;
		offset = Mathf.Clamp01( v/2f );
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0);
	}
}
