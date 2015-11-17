using UnityEngine;
using System.Collections;

public class Module : MonoBehaviour
{
	
	public string moduleName;
	private int pIdx = 0;

    public int moduleId = -1;
	
	[System.Serializable]
	public class Partial
	{
		
		public int start;
		public int end; 
		public Color sColor;
		public Color eColor;
	}
	
	public Partial[] partials;
	public Light light;
	//
	private bool isStarted;
	private float startTime;
	public bool isFinished;
	private ControlPanel cp;
	public float lightMultiplier = 1;

	public Color LightColor {
		set {
			light.color = value * cp.lightMultiplier * lightMultiplier;
			//light.intensity = value.a * cp.lightMultiplier * lightMultiplier;
		}
        get{
            return light.color;
        }
	}

	void Start ()
	{		
		cp = ControlPanel.Instance;
		LightColor = Color.black;
	}

	public void StartModule (float startTime)
	{
		this.startTime = startTime;
		isStarted = true;
		isFinished = false;
		pIdx = 0;
		timeElappsed = 0;

	}

	public void StopModule ()
	{
		isStarted = false;
		isFinished = true;
	}

	private float speed = 0;
	private float timeElappsed = 0;

	void Update ()
	{
		if (!isStarted || isFinished)
			return;

		if (partials == null) {
			//light.color = Color.black;
			LightColor = Color.black;
			return;
		}

		if (speed != cp.speed) {
			pIdx = 0;
		}
		speed = cp.speed;

		timeElappsed = (Time.time - startTime) * 1000 * speed; 
		//float t = (Time.time - startTime) * 1000 * speed;
		float t = timeElappsed;

		if (partials [partials.Length - 1].end < t) {
			Partial last = partials [partials.Length - 1];
			LightColor = last.eColor;
			isFinished = true;
			return;
		}


		for (int i=pIdx; i<partials.Length; i++) {
			Partial p = partials [i];
			if (t > p.start && t < p.end) {
				//light.color = Color.Lerp (p.sColor, p.eColor, (t - p.start) / (float)(p.end - p.start)) * cp.lightMultiplier * lightMultiplier;
				//light.intensity = Mathf.Lerp (p.sColor.a, p.eColor.a, (t - p.start) / (float)(p.end - p.start)) * cp.lightMultiplier * lightMultiplier;
				LightColor = Color.Lerp (p.sColor, p.eColor, (t - p.start) / (float)(p.end - p.start));
				pIdx = i;
				break;
			}
		}
	}
}
