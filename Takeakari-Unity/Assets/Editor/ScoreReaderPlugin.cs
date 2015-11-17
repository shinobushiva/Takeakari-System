using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ScoreReaderPlugin {

	[MenuItem("Akari/Score/Read")]
	public static void CreatePrefab ()
	{
		string path = EditorUtility.OpenFilePanel("Choose Score File", "", "");
		Debug.Log (path);

		string score = File.ReadAllText (path);

		int num = path.LastIndexOf ('/') + 1;
		string n = path.Substring (num, path.Length-num-1); 
		GameObject sObject = new GameObject (n);

		ScorePlayer sp = sObject.AddComponent<ScorePlayer> ();
		sp.ReadScore (score);
	
	}
}
