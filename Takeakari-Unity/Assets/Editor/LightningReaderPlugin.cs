using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class LightningReaderPlugin {

	[MenuItem("Akari/Lightning/Read")]
	public static void CreatePrefab ()
	{
		string path = EditorUtility.OpenFilePanel("Choose Lightning File", "", "");
		Debug.Log (path);

		string score = File.ReadAllText (path);

		int num = path.LastIndexOf ('/') + 1;
		string n = path.Substring (num, path.Length-num); 
		GameObject sObject = new GameObject (n);

		LightningPlayer sp = sObject.AddComponent<LightningPlayer> ();
		sp.ReadScore (score);
	
	}
}
