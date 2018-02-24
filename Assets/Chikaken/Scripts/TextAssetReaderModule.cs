using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class TextAssetReaderModule : ReaderModule
{

	private string csv;
	public TextAsset textAsset;
	private List<Module.Partial[]> list;
	private List<string> moduleNames;
	
	public override void Initialize ()
	{
		list = new List<Module.Partial[]> ();
		moduleNames = new List<string> ();
		ReadOnly ();
		dataName = gameObject.name;

	}

	private void ReadOnly ()
	{
		list.Clear ();
		moduleNames.Clear ();

		string csv = textAsset.text;	
		StringReader sr = new StringReader (csv);
		
		int idx = 0;
		string line = null;
		while ((line = sr.ReadLine()) != null) {
			int c = 0;
			if (line.Length <= 0)
				continue;

			string[] ss = line.Split (',');

			int num = (ss.Length - 1) / 4;
			Module.Partial[] partials = new Module.Partial[num];
			list.Add (partials);
			moduleNames.Add (ss [c++]);

			try {
				for (int i=0; i<num; i++) {
					Module.Partial p = new Module.Partial ();
					p.start = int.Parse (ss [c++]);
				
					p.sColor = ToColor (int.Parse (ss [c++]));
					p.end = int.Parse (ss [c++]);
					p.eColor = ToColor (int.Parse (ss [c++]));
					partials [i] = p;
				}
			} catch (System.Exception e) { 
				print (e);
				print (ss [c - 1]);
				print (ss [c]);
			}
			
			idx++;
		}
	}

	public override void Read (Module[] modules)
	{

		for (int i=0; i<modules.Length; i++) {
			if(modules [i] == null)
				continue;
			if (i >= list.Count) {
				modules [i].partials = null;
				modules [i].moduleName = "";
			} else {
				modules [i].partials = list [i];
				modules [i].moduleName = moduleNames [i];
			}
		}

	}


}
