using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TabbedGUI
{
		
	private GUIStyle style;
		
	public class Tab
	{
		public string name;
		public GUI.WindowFunction f;	
	}
		
	private List<Tab> tabs = new List<Tab> ();
	private List<string>names = new List<string> ();
		
	public void Add (Tab t)
	{
		tabs.Add (t);
			
		names.Clear ();
		foreach (Tab tt in tabs) {
			names.Add (tt.name);
		}
	}
		
	public void Add (string name, GUI.WindowFunction f)
	{
		Tab t = new Tab ();
		t.name = name;
		t.f = f;
		Add (t);
	}
		
	private int selected = 0;
		
	public void DrawGUI ()
	{
			
		if (style == null) {
			style = new GUIStyle ();
			Texture2D t2d = new Texture2D (1, 1);
			t2d.SetPixel (0, 0, Color.gray);
			t2d.Apply ();
			style.normal.background = t2d;
		}
			
			
		GUILayout.BeginVertical (style, GUILayout.MinWidth (600));
		selected = GUILayout.SelectionGrid (selected, names.ToArray (), names.Count);
		GUILayout.Space (20);
		tabs [selected].f (0);
		GUILayout.EndVertical ();
			
	}
}

