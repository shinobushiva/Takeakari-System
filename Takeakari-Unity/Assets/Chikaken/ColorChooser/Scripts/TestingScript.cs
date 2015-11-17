/****************************************************************************
 * ColorChooser Test function
 * Franz Reichel
 * 17.04.2014
 * 
 * Description
 * Test and Demo function for the ColorChooser
 */

using UnityEngine;
using System.Collections;

public class TestingScript : MonoBehaviour 
{
	public Transform plane;
	public Transform sphere;
	public Transform cube;
	public Transform cabsule;

	bool rayCast = true;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//---------------------------------------------------------------------------------------------------- Color change on raycast
		if (Input.GetMouseButtonDown(0))
		{
			ColorChooser c = GameObject.FindObjectOfType<ColorChooser>();

			if(rayCast && (c == null || !c.blockRayCast()))
			{  
				if (!ColorChooser.pipMode)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					if (Physics.Raycast(ray, out hit, 1000000))
					{
						ColorChooser cc_ = hit.transform.gameObject.AddComponent<ColorChooser>();
						cc_.setColor(hit.transform.GetComponent<Renderer>().material);
						cc_.setPostion(new Vector2(Input.mousePosition.x + 10, Screen.height - Input.mousePosition.y));
					}
				}
			}

			ColorChooser.pipMode = false;
		}

		//---------------------------------------------------------------------------------------------------- reset pipette mode
		if (Input.GetKey(KeyCode.Escape))
		{
			ColorChooser.pipMode = false;
		}
	}

	void OnGUI()
	{

		GUILayout.BeginArea(new Rect(Screen.width - 210 , 10 , 200, 150));
		GUILayout.BeginVertical("box");
		GUILayout.Label("The ColorChooser \n\nIn the form of the UnityEditor ColorChooser \n\nCreated by Franz Reichel \n17.04.2014");
		GUILayout.EndVertical();
		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(10 , 10 , 170, 500));

		GameObject p = GameObject.Find("TestObjects");

		if (p != null)
		{
			GUILayout.BeginVertical("box");
			GUILayout.Label("RayCast");
			rayCast = GUILayout.Toggle(rayCast, "Change Object Color \nwith clicking on it");
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			GUILayout.Label("Small ColorChooser");

			for (int i = 0 ; i < p.transform.childCount ; i++)
			{
				Transform t = p.transform.GetChild (i);

				//----------------------------------------------------------------------------------------------------
				// ColorChooser small static menu
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(t.name);
				GUILayout.FlexibleSpace();
				ColorChooser.colorMenu(t.GetComponent<Renderer>().material, 50, 20, i);
				if (Event.current.type == EventType.Repaint)
				{
					if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Input.GetMouseButton(0))
					{
						// Instantiate the ColorChooser
						ColorChooser cc = transform.gameObject.AddComponent<ColorChooser>();
						cc.setColor(t.GetComponent<Renderer>().material);
						cc.setPostion(new Vector2(Input.mousePosition.x + 20, Screen.height - Input.mousePosition.y));
					}
				}

				GUI.skin = ColorChooser.myGUISkin;
				if(GUILayout.Button(ColorChooser.texPip, GUILayout.Width(30), GUILayout.Height(30)))
				{
					ColorChooser.pipMode = true;
					ColorChooser.selectedIndex = i;
				}
				if (ColorChooser.pipMode)
				{
					StartCoroutine(ColorChooser.mousePointScreenshot(Input.mousePosition));
				}
				GUI.skin = ColorChooser.myOldGUISkin;
				
				GUILayout.EndHorizontal();
				//----------------------------------------------------------------------------------------------------

			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			GUILayout.Label("Simple ColorChooser");
			for (int i = 0 ; i < p.transform.childCount ; i++)
			{
				Transform t = p.transform.GetChild (i);
				if (GUILayout.Button(t.name))
				{
					// Instantiate the ColorChooser
					ColorChooser cc = transform.gameObject.AddComponent<ColorChooser>();
					cc.setColor(t.GetComponent<Renderer>().material);
					cc.setPostion(new Vector2(Input.mousePosition.x + 20, Screen.height - Input.mousePosition.y));
				}
			}
			GUILayout.EndVertical();
		}

		GUILayout.EndArea();
	}
}
