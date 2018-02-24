using UnityEngine;
using System.Collections;
using System.IO;

public class Reader : SingletonMonoBehaviour<Reader>
{

	public ModuleArray moduleArray;
	private Module[] modules;
	public bool isLoad = false;
	private string logText = "";
	public ReaderModule[] readers;
	//

	public bool isPlaying = false;
	public float startTime = 0;

	// Use this for initialization
	void Start ()
	{
		modules = moduleArray.modules;
	}

	// Update is called once per frame
	void Update ()
	{
		
		bool b = true;
		foreach (Module m in modules) {

			b = (m.partials == null) || m.isFinished;
			if (!b) {
				break;
			}
		}
		if (isPlaying && b && ControlPanel.Instance.loop) {
			StartPlaying ();
		}
	}

	void StartPlaying ()
	{
		startTime = Time.time;
		//print (modules.Length);
		foreach (Module m in modules) {
			if(m == null)
				continue;

			m.StopModule ();
			m.StartModule (startTime);
		}
	}

	public void LoadData(){
		foreach (ReaderModule rm in readers) {
			if (!rm.gameObject.activeSelf)
				continue;
			
			rm.Initialize ();
		}
		isLoad = true;
	}

	public void Play(int i){
		isPlaying = true;
		ReaderModule rm = readers[i];
		rm.Read (modules);
		StartPlaying ();
	}

	public void Stop(){
		isPlaying = false;
		foreach (Module m in modules) {
			m.StopModule ();
		}
	}

	void OnGUI ()
	{

		GUI.Label (new Rect (Screen.width * 0.1f, Screen.height * 0.9f, Screen.width * 0.8f, Screen.height * 0.1f), logText);
	}
}
