using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BibbleLightControl : MonoBehaviour {

    private ColorModeChanger cmc;

    private int buttonOneMode = 3;

    private float distance;

	// Use this for initialization
	void Start () {
        cmc = FindObjectOfType<ColorModeChanger>();
	}

    private IEnumerator DistanceSequence(ColorModeChanger cmc) {
        while (buttonOneMode == 2)
        {
            foreach (CloudLamp lamp in cmc.lamps)
            {
                Color col = Color.white;
                col.r = col.r / Mathf.Max(1, distance);
                col.g = col.g / Mathf.Max(1, distance);
                col.b = col.b / Mathf.Max(1, distance);

                lamp.col = col;
            }
//            yield return new WaitForSeconds(cmc.RotationTime * 0.1f);
            yield return new WaitForEndOfFrame();
        }
    }

	public void OnBibbleServer(bool running) {
		if (!running) {
			buttonOneMode = 3;
		}
	}

    public void DataReceived(BibbleServer.ReceivedData rd){

        if (cmc)
        {
            if (rd.distance > 0)
            {
                this.distance = (this.distance + rd.distance) / 2;
            }
            
            if (rd.button == 2)
            {
                cmc.ColorMode = ColorModeChanger.Mode.RandomAuto;
            }

            if (rd.button == 1)
            {
                buttonOneMode = (buttonOneMode + 1) % 4;
                switch (buttonOneMode)
                {
                    case 0:
                        cmc.ColorMode = ColorModeChanger.Mode.TestSequence;
                        break;
                    case 1:
                        cmc.ColorMode = ColorModeChanger.Mode.Sequence;
                        break;
                    case 2:
                        cmc.customMode = ((cmc) =>  {
                            foreach (CloudLamp lamp in cmc.lamps)
                            {
                                lamp.intaractive = false;
                            }
                            StartCoroutine(DistanceSequence(cmc));
                        });
                        cmc.ColorMode = ColorModeChanger.Mode.Custom;
                        break;
                    case 3:
                        cmc.ColorMode = ColorModeChanger.Mode.None;
                        break;
                }
            }
                
        }
        
    }
}
