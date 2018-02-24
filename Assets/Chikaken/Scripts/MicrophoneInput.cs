using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
	
	public ModuleArray moduleArray;
	public float[] colorTimes;
	//

	public float sensitivity = 100.0f;
	public float loudness = 0.0f;
	public float frequency = 0.0f;
	public string note = "";
	public int samplerate = 44100;
	public int rate = 8192;
	public int scale = 1;
	//
	float[] data ;
	bool[] flags = new bool[12 * 8];
	GUIText[] noteTextes = new GUIText[12 * 8];
	public float loudnessTh = 0.1f;

	//
	public bool rotateColor = false;
	private Color[] colors = new Color[]{
		new Color (0xff, 0x00, 0x00) / 0xff,
		new Color (0xff, 0x80, 0x00) / 0xff,
		new Color (0xff, 0xff, 0x00) / 0xff,
		new Color (0x80, 0xff, 0x00) / 0xff,
		new Color (0x00, 0xff, 0x00) / 0xff,
		new Color (0x00, 0xff, 0x80) / 0xff,
		new Color (0x00, 0xff, 0xff) / 0xff,
		new Color (0x00, 0x80, 0xff) / 0xff,
		new Color (0x00, 0x00, 0xff) / 0xff,
		new Color (0x80, 0x00, 0xff) / 0xff,
		new Color (0xff, 0x00, 0xff) / 0xff,
		new Color (0xff, 0x00, 0x8f) / 0xff
	};
	private int colorIdx = 0;
	public float timeToChange = 1;
	public float lastTimeChanged = 0;

	private int numNotes = 88;

	//
	public string[] noteNames = new string[] {
		"c",
		"c+",
		"d",
		"d+",
		"e",
		"f",
		"f+",
		"g",
		"g+",
		"a",
		"a+",
		"b"
	};
	public float middleA = 440;
	public float[] noteFrequences;
	public bool showDebugTexts = false;

	//
	public bool isRunning = false;

	public void End ()
	{
		if (!isRunning)
			return;

		StopAllCoroutines ();
		GetComponent<AudioSource>().Stop ();
		Microphone.End (null);
		isRunning = false;
	}

	public void Begin ()
	{
		
		if (isRunning)
			return;

		StartCoroutine (_Begin ());
	}


	private  IEnumerator _Begin ()
	{

		bool fail = false;

		GetComponent<AudioSource>().clip = Microphone.Start (null, true, 10, samplerate);
		GetComponent<AudioSource>().loop = true; // Set the AudioClip to loop
		GetComponent<AudioSource>().mute = true; // Mute the sound, we don't want the player to hear it
		float tt = Time.time + 10;
		while (!(Microphone.GetPosition(Microphone.devices [0]) > 0)) {
			if (tt < Time.time) {
				print ("Failed to Start Microphon...");
				Microphone.End (null);
				fail = true;
				break;
			}
			yield return new WaitForEndOfFrame ();
		} // Wait until the recording has started

		if (!fail) {
			GetComponent<AudioSource>().Play (); // Play the audio source!

			//initialize
			data = new float[rate];

			//Create frequency map
			float a0 = Mathf.RoundToInt (middleA * 100 / 16) / 100f;
			float a1 = a0 * 2;

			noteFrequences = new float[numNotes];
			for (int i=0; i<88; i++) {
				float pitch = (a1 - a0) / 12f;
				float f = a0 + pitch * (i % 12);
				noteFrequences [i] = f;
				if ((i + 1) % 12 == 0) {
					a0 = a1;
					a1 = a1 * 2;
				}
			}

			for (int i=0; i<numNotes; i++) {
				GameObject go = new GameObject ();
				GUIText t = go.AddComponent<GUIText> ();
				t.text = "" + noteNames [(i + 9) % 12] + ":" + ((i + 9) / 12);
				t.transform.position = new Vector3 (0.1f + 0.8f / 12f * ((i + 9) % 12), 0.9f - 0.1f * ((i + 9) / 12), 0);
				noteTextes [i] = t;
			}

			colorTimes = new float[moduleArray.modules.Length];
			StartCoroutine (Loop (0));

			isRunning = true;
		}
	}

	IEnumerator Loop2 (float waitTime)
	{
		
		int colorTimeMax = 5;
		
		while (true) {
			GetComponent<AudioSource>().GetSpectrumData (data, 0, FFTWindow.BlackmanHarris);
		
			for (int i=0; i<numNotes; i++) {
				if (showDebugTexts) {
					noteTextes [i].enabled = true;
					noteTextes [i].color = Color.gray;
				} else {
					noteTextes [i].enabled = false;
				}
			}
		
			GetComponent<AudioSource>().GetSpectrumData (data, 0, FFTWindow.BlackmanHarris);
			//frequency = GetFundamentalFrequency (data);
			//note = FrequencyToNote (frequency);
			loudness = GetAveragedVolume () * sensitivity;

			bool on = loudness / 1000f > loudnessTh;

			for (int i=0; i<colorTimes.Length; i++) {
				if (on) {
					colorTimes [i] += Time.deltaTime * Random.Range(loudness/100f, loudness);
				} else {
					colorTimes [i] = Mathf.Clamp (colorTimes [i] - Time.deltaTime*100, 0, colorTimeMax);
				}

			}


			Color c;
			if (rotateColor) {
					
				if (lastTimeChanged + timeToChange < Time.time) {
					lastTimeChanged = Time.time + (timeToChange * (Random.Range (0f, 1f) - 0.5f));
					//colorIdx = (colorIdx + 1) % colors.Length;
					colorIdx = Random.Range (0, colors.Length);
				}
					
				Color c1 = colors [colorIdx];
				c = c1;
					
			} else {
                c = FindObjectOfType<ColorChooser>().GetColor();
			}

				
			Color bc = Color.black;
			int mapped;
			for (int i=0; i<colorTimes.Length; i++) {
					
				mapped = i;
					
				if (colorTimes [i] > 0) {
					float f = colorTimes [i] / colorTimeMax;
					moduleArray.modules [mapped].LightColor = c * f;
				} else {
					float f = colorTimes [i] / colorTimeMax;
					moduleArray.modules [mapped].LightColor = bc * f;
				}
			}

			yield return new WaitForSeconds (waitTime);
		}
	}
	
	IEnumerator Loop (float waitTime)
	{

		while (true) {

			for (int i=0; i<numNotes; i++) {
				if (showDebugTexts) {
					noteTextes [i].enabled = true;
					noteTextes [i].color = Color.gray;
				} else {
					noteTextes [i].enabled = false;
				}
			}

			GetComponent<AudioSource>().GetSpectrumData (data, 0, FFTWindow.BlackmanHarris);
			frequency = GetFundamentalFrequency (data);
			//note = FrequencyToNote (frequency);
			loudness = GetAveragedVolume () * sensitivity;

			scale = Mathf.Clamp (scale, 1, 10000);
			for (int f = 1; f < rate; f++) {

				float s = data [f];
				float freq = f * samplerate / rate / scale;
				
				if (s > loudnessTh) {
					for (int i=1; i<numNotes; i++) {
						int mNum = Mathf.RoundToInt (moduleArray.modules.Length * i / ((float)numNotes));
						//int mNum = Random.Range(0, moduleArray.modules.Length);

						moduleArray.modules [mNum].light.color = Color.black;
						if (noteFrequences [i - 1] <= freq && freq <= noteFrequences [i]) {
							if ((freq - noteFrequences [i - 1]) > (noteFrequences [i] - freq)) {
								if (showDebugTexts)
									noteTextes [i].color = Color.green;
								
								colorTimes [mNum] += Time.deltaTime * s * 1000;
								colorTimes [(mNum + moduleArray.modules.Length / 2) % moduleArray.modules.Length] += Time.deltaTime * s * 1000;
							} else {
								if (showDebugTexts)
									noteTextes [i - 1].color = Color.green;

								colorTimes [mNum] += Time.deltaTime * s * 1000;
								colorTimes [(mNum + moduleArray.modules.Length / 2) % moduleArray.modules.Length] += Time.deltaTime * s * 1000;
							}
							break;
						}
					}
				}
			}

			if (showDebugTexts) {
				for (int i=1; i<numNotes; i++) {
					int mNum = Mathf.RoundToInt (moduleArray.modules.Length * i / ((float)numNotes));
					moduleArray.modules [mNum].light.color = Color.black;
					if (noteFrequences [i - 1] <= frequency && frequency <= noteFrequences [i]) {
						if ((frequency - noteFrequences [i - 1]) > (noteFrequences [i] - frequency)) {
							noteTextes [i].color = Color.red;

						} else {
							noteTextes [i - 1].color = Color.red;
						
						}
						break;
					} 
				}
			}


			Color c;
			if (rotateColor) {

				if (lastTimeChanged + timeToChange < Time.time) {
					lastTimeChanged = Time.time + (timeToChange * (Random.Range (0f, 1f) - 0.5f));
					//colorIdx = (colorIdx + 1) % colors.Length;
					colorIdx = Random.Range (0, colors.Length);
				}

				Color c1 = colors [colorIdx];
				c = c1;

			} else {
                c = FindObjectOfType<ColorChooser>().GetColor();
			}

			int colorTimeMax = 5;

			Color bc = Color.black;
			int mapped;
			for (int i=0; i<colorTimes.Length; i++) {

				mapped = (i + (colorTimes.Length - 12)) % colorTimes.Length;

				if (colorTimes [i] > 0) {
					float f = colorTimes [i] / colorTimeMax;
					c.a = f;
					//moduleArray.modules [mapped].light.color = c; 
					//moduleArray.modules [mapped].light.intensity = f;
					moduleArray.modules [mapped].LightColor = c;
				} else {

					float f = colorTimes [i] / colorTimeMax;
					bc.a = f;
					//moduleArray.modules [mapped].light.color = bc;
					//moduleArray.modules [mapped].light.intensity = f;
					moduleArray.modules [mapped].LightColor = bc;
				}
				colorTimes [i] = Mathf.Clamp (colorTimes [i] - Time.deltaTime * 100, 0, colorTimeMax);

			}


			yield return new WaitForSeconds (waitTime);
		}
	}
	
	void Update ()
	{
				
	}
	
	float GetAveragedVolume ()
	{
		float[] data = new float[256];
		float a = 0;
		GetComponent<AudioSource>().GetOutputData (data, 0);
		foreach (float s in data) {
			a += Mathf.Abs (s);
		}
		return a / 256;
	}
	
	void MapFrequencyToNote (float[] data, bool[] notes, float th)
	{
		for (int i = 1; i < rate; i++) {
			float s = data [i];
			if (s > th) {



			}
		}
			
			
	}
	
	string FrequencyToNote (float freq)
	{
		float a0 = Mathf.RoundToInt (middleA / 8);
		float a1 = Mathf.RoundToInt (middleA / 4);

		while (true) {

			float pitch = (a1 - a0) / 12f;
			for (int i=0; i<12; i++) {
				float f = a0 + pitch * i;

				if (f >= freq)
					return noteNames [(i + 9) % 12];
			}

			a0 = a1;
			a1 = Mathf.RoundToInt (a1 * 2);

		}

	}
	
	float GetFundamentalFrequency (float[] data)
	{
		float fundamentalFrequency = 0.0f;
				
		float s = 0.0f;
		int i = 0;
		for (int j = 1; j < rate; j++) {
			if (s < data [j]) {
				s = data [j];
				i = j;
			}
		}
		fundamentalFrequency = i * samplerate / rate;
		return fundamentalFrequency;
	}

}