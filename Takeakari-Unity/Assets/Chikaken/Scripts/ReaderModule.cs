using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class ReaderModule: MonoBehaviour
{

	public string dataName = "";

	public abstract void Initialize ();

	public abstract void Read (Module[] modules);

	public Color ToColor (int argb)
	{
		Color c = new Color ();
		c.a = ((argb & 0xff000000) >> 24) / 255f;
		c.r = ((argb & 0x00ff0000) >> 16) / 255f;
		c.g = ((argb & 0x0000ff00) >> 8) / 255f;
		c.b = (argb & 0x000000ff) / 255f;
		return c;
	}

}
