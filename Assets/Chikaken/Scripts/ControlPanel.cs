using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class  ControlPanel:  SingletonMonoBehaviour<ControlPanel>
{

//	private Module[] modules;
	private bool isLoad = false;
	public string logText = "";

	//
	private ColorChooser c;
	public bool applyImmidiate = false;

	//
	public bool loop = true;
	private bool updating = false;
	//
	private TabbedGUI tabbedGUI = new TabbedGUI ();

	//
	public bool useCompress = false;

    public bool drawGUI = false;

    
    public float lightMultiplier = 1;
    public float speed = 1;

    public float LightMultiplier {
        get{
            return lightMultiplier;
        }
        set{
            lightMultiplier = value;// Mathf.Lerp(lightMultiplier, value, Time.deltaTime);
        }
    }
     
    public float Speed{
        get{
            return speed;
        }
        set{
            //speed = Mathf.Lerp(speed, value, Time.deltaTime);
            speed = value;
        }
    }

//    ModuleColorUpdator mcu;


	// Use this for initialization
	void Start ()
	{
//        mcu = GetComponent<ModuleColorUpdator>();

//		modules = moduleArray.modules;

		c = GameObject.FindObjectOfType<ColorChooser> ();
        if(c)
		    c.SetColor (Color.black);

		//tabbedGUI.Add ("Connect", DrawConnect);
		//tabbedGUI.Add ("Table", DrawTable);
		//tabbedGUI.Add (groupedLightDimmer);

		LoadSettings ();

		
        // ConnectToServer();
	}

	private void SaveSettings ()
	{

		PlayerPrefs.SetInt ("loop", loop ? 1 : 0);
//		PlayerPrefs.SetString ("server", mcu.server);

		PlayerPrefs.SetFloat ("lightMultiplier", lightMultiplier);
		PlayerPrefs.SetFloat ("speed", speed);

        /*
		GroupedLightDimmer.ModuleGroup[] mgs = groupedLightDimmer.moduleGroups;
		foreach (GroupedLightDimmer.ModuleGroup mg in mgs) {
			PlayerPrefs.SetFloat (mg.groupName + "_" + "mul", mg.value);
		}
  */      
	}

	private void LoadSettings ()
	{
		loop = (PlayerPrefs.GetInt ("loop", 1) == 1);
		//mcu.server = PlayerPrefs.GetString ("server", "localhost:33334");

	

		lightMultiplier = PlayerPrefs.GetFloat ("lightMultiplier", 1);
		speed = PlayerPrefs.GetFloat ("speed", 1);

        /*
		GroupedLightDimmer.ModuleGroup[] mgs = groupedLightDimmer.moduleGroups;
		foreach (GroupedLightDimmer.ModuleGroup mg in mgs) {
			mg.value = PlayerPrefs.GetFloat (mg.groupName + "_" + "mul", 1);
		}
  */      
	}

	
//	void OnDestroy ()
//	{
//		//SaveSettings ();
//		mcu.DisconnectFromServer ();
//	}

	Color ToColor (int argb)
	{
		Color c = new Color ();
		c.a = ((argb & 0xff000000) >> 24) / 255f;
		c.r = ((argb & 0x00ff0000) >> 16) / 255f;
		c.g = ((argb & 0x0000ff00) >> 8) / 255f;
		c.b = (argb & 0x000000ff) / 255f;
		return c;
	}


    /*
	public IEnumerator UpdateModuleColors ()
	{
		WaitForEndOfFrame wfeof = new WaitForEndOfFrame ();
		while (true) {
			if (applyImmidiate) {
                
                //print("applyImmidiate");
		
				if (!updating) {
					updating = true;
					_UpdateModuleColors2 ();
				}
			}
			yield return wfeof;
		}
	}

	private void _UpdateModuleColors2 ()    
	{
        
        //print("_UpdateModuleColors2:"+isConnectedToServer);

		if (!isConnectedToServer)
        {
            updating = false; 
            return;
        }

        //print("_UpdateModuleColors2");

		byte[] buf = this.buf;
		int idx = 0;

		int counter = 0;
		foreach (Module m in modules) {

            counter++;
            if (counter % 8 == 0) {
                counter++;
            }
			
			if (useCompress && colorMap [m] == m.light.color)
				continue;

            if(m == null)
                continue;

			Color col = m.light.color;
			colorMap [m] = col;
			
			buf [idx++] = 0x3F;
			
			buf [idx++] = (byte)(((byte)(col.r * 127)) | 0x80);
			buf [idx++] = (byte)(((byte)(col.g * 127)) | 0x80);
			buf [idx++] = (byte)(((byte)(col.b * 127)) | 0x80);


            //if(m.moduleId > 0){
            //    buf [idx++] = ((byte)(((byte)m.moduleId) | 0x80));
            //}else{
       
            //print(""+counter);
            buf [idx++] = ((byte)(((byte)counter) | 0x80)); 

            //}

		} 

		if (idx > 0) {
			ns.Write (buf, 0, idx);
			ns.Flush ();
			ns.ReadByte ();
		}

		updating = false;
	}
*/

	
	// Update is called once per frame
	void Update ()
	{

	}

    /*
	float DrawSlider (string n, float val, float init, float min, float max)
	{
		GUILayout.BeginHorizontal ();
		{
			GUILayout.Label (n + " : " + ((int)(val * 1000)) / 1000f, GUILayout.Width (120), GUILayout.MaxWidth (120));
			if (GUILayout.Button ("Reset", GUILayout.MaxWidth(80))) {
				val = init;
			}
			val = GUILayout.HorizontalSlider (val, min, max, GUILayout.MinWidth (200), GUILayout.ExpandWidth(true));
		}
		GUILayout.EndHorizontal ();
		
		return val;
	}

	float DrawSlider (string n, float val, float init)
	{
		return DrawSlider (n, val, init, 0, 1);
	}

	bool isDataLoad = false;

	void DrawTable (int windowID)
	{

		GUILayout.BeginHorizontal ();
		{
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("SelectAll")) { 
				foreach (Module m in modules) {
					m.GetComponent<ModuleSelection> ().isChecked = true;
				}
			}
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("UnSelectAll")) {
				foreach (Module m in modules) {
					m.GetComponent<ModuleSelection> ().isChecked = false;
				}
			}
			GUILayout.FlexibleSpace ();
		}
		GUILayout.EndHorizontal ();

		GUILayout.Space (20);
		
		GUILayout.BeginHorizontal ();
		{
			if (applyImmidiate) {
				GUI.enabled = false;
			}
			if (GUILayout.Button ("Apply")) {
				Color cc = c.getColor ();
				foreach (Module m in modules) {
					m.LightColor = cc;
				}
				_UpdateModuleColors2 ();
			}
			if (applyImmidiate) {
				GUI.enabled = true;
			}
			applyImmidiate = GUILayout.Toggle (applyImmidiate, "Apply Immidiate");
			
		}
		GUILayout.EndHorizontal ();

	}

	Vector2 scrollPos;

	void DrawDataLoad (int windowID)
	{
		GUILayout.BeginVertical ();
		
		
		if (!Reader.Instance.isLoad) {
			if (GUILayout.Button ("Load")) {
				Reader.Instance.LoadData ();
			}
		} else {

			scrollPos = GUILayout.BeginScrollView (scrollPos);
			GUILayout.BeginHorizontal ();
			int idx = 0;
			foreach (ReaderModule rm in Reader.Instance.readers) {
				if (!rm.gameObject.activeSelf) 
					continue;
				
				if (GUILayout.Button (rm.dataName, GUILayout.Width (100), GUILayout.Height (100))) {
					Reader.Instance.Play (idx);
				}
				idx++;
			}
			if (GUILayout.Button ("**Stop**", GUILayout.Width (100), GUILayout.Height (100))) {
				Reader.Instance.Stop ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndScrollView ();

			GUILayout.BeginHorizontal ();
			{
				loop = GUILayout.Toggle (loop, "Loop : ");
			}
			GUILayout.EndHorizontal ();
		}
		GUILayout.EndVertical ();
	}


	void DrawConnect (int windowID)
	{
		GUILayout.BeginVertical ();

		if (!isConnectedToServer) {
			server = GUILayout.TextField (server);

			if (GUILayout.Button ("Connect to Server")) {
				ConnectToServer ();
			}
		} else {
			if (GUILayout.Button ("Disconnect From Server")) {
				DisconnectFromServer ();
			}
		}
		GUILayout.EndVertical ();
	}
 */   

	//public GroupedLightDimmer groupedLightDimmer = new GroupedLightDimmer ();

    /*
	[System.Serializable]
	public class GroupedLightDimmer : TabbedGUI.Tab
	{
		public float lightMultiplier = 1;
		public float speed = 1;

		[System.Serializable]
		public class ModuleGroup
		{
			public string groupName;
			public Module[] modules;
			public float value = 1;
		}

		public ModuleGroup[] moduleGroups;

		public GroupedLightDimmer ()
		{
			f = Draw;
		}

		void Draw (int windowID)
		{
			GUILayout.BeginVertical ();

			lightMultiplier = ControlPanel.Instance.DrawSlider("Light : ", lightMultiplier, 1, 0, 1);
			speed = ControlPanel.Instance.DrawSlider("Speed : ", speed, 1, 0.1f, 4f);

			GUILayout.Label ("Group Light Control:");

			foreach (ModuleGroup mg in moduleGroups) {
				mg.value = ControlPanel.Instance.DrawSlider ("  " + mg.groupName, mg.value, 1);
				foreach (Module m in mg.modules) {
					m.lightMultiplier = mg.value;
				}
			}
				
			GUILayout.EndVertical ();
		}
	}
 */   

    /*
	void OnGUI ()
	{

        
        GUI.Label (new Rect (Screen.width * 0.1f, Screen.height * 0.9f, Screen.width * 0.8f, Screen.height * 0.1f), logText);

        if (!drawGUI)
            return;

		if (tabbedGUI != null)
			tabbedGUI.DrawGUI ();

		Rect wr = new Rect ();
		wr.x = Screen.width - 300;
		wr.y = 0;
		wr.width = 290;
		wr.height = 200;
		GUILayout.Window (1, wr, DrawInfoGUI, "");

	}

	private GUIStyle style;

	public void DrawInfoGUI (int windowId)
	{
		GUILayout.BeginVertical ();

		if (style == null) {
			style = new GUIStyle ();
			Texture2D t2d = new Texture2D (1, 1);
			t2d.SetPixel (0, 0, Color.gray);
			t2d.Apply ();
			style.normal.background = t2d;
		}

		GUILayout.Label ("Apply Immidiate : " + applyImmidiate);
		GUILayout.Label ("Sequence Speed : " + groupedLightDimmer.speed);
		GUILayout.Label ("Sequence Loop : " + loop);
		GUILayout.Label ("Global Light : " + groupedLightDimmer.lightMultiplier);
		


		GUILayout.EndVertical ();
		
	}
 */   
}
