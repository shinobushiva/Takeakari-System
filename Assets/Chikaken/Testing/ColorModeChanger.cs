using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ColorModeChanger : MonoBehaviour
{
    public CloudLamp[] lamps;
    public Text modeText;
    //    public ModuleArray moduleArray;
    
    [System.Serializable]
    public enum Mode
    {
        RandomColor,
        ColorRotation,
        Sequence,
        OneColor,
        TestSequence,
        None,
        RandomAuto,
        ScorePlay,
        Custom
    }

    private Mode mode;
    public Mode ColorMode
    {
        get
        {
            return currentMode;
        }
        set
        {
            this.mode = value;
            modeText.text = this.mode.ToString();
        }
    }
    private Mode currentMode;

    //private float rotationTime = 1f;
    private float nextRotationTime = 0f;
    private Color currentColor; 
    private Color nextColor;
    public Color oneColor = Color.blue;

    public delegate void CustomModeDelegate(ColorModeChanger cmc);
    public CustomModeDelegate customMode;

    public float RotationTime
    {
        get
        {
            return ControlPanel.Instance.Speed;
        }
        set
        {
            ControlPanel.Instance.Speed = value;
        }
    } 


    public void SetNone()
    {
        ColorMode = Mode.None;
    }

    public void SetTestSequence()
    {
        ColorMode = Mode.TestSequence;
    }

    public void SetRandomColor()
    {
        ColorMode = Mode.RandomColor;
    }

    public void SetRotationColor()
    {
        ColorMode = Mode.ColorRotation;
    }

    public void SetSequentialColor()
    {
        ColorMode = Mode.Sequence;
    }

    public void SetOneColor()
    {
        ColorMode = Mode.OneColor;
    }

    public void SetScorePlay()
    {
        ColorMode = Mode.ScorePlay;
    }

    public void SetRandomAuto(){
        ColorMode = Mode.RandomAuto;
    }

   

    // Use this for initialization
    void Start()
    {
        modeText = GetComponentInChildren<Text>();

        if (lamps.Length == 0)
        {
            lamps = FindObjectsOfType(typeof(CloudLamp)) as CloudLamp[]; 
        }
        mode = Mode.OneColor;

        
    }
    
    public Color RandomColor()
    {
        Vector3 v = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        v = v.normalized;
        return new Color(v.x, v.y, v.z);
    }

    bool testSequence = false;
    IEnumerator TestSequence(){

        Color c = RandomColor();

        foreach (CloudLamp lamp in lamps)
        {
            lamp.intaractive = true;
            lamp.col = Color.black;
            
            if(!testSequence)
                break;
        }

        foreach (CloudLamp lamp in lamps)
        {
            lamp.intaractive = false;
            lamp.col = c;
            yield return new WaitForSeconds(.1f * RotationTime);
            lamp.col = Color.black;
            
            if(!testSequence)
                break;
        }

        testSequence = false;
    }

    Color[] randomColors;
    
    // Update is called once per frame
    void Update()
    {

//        if (mode != Mode.ScorePlay)
//        {
//            ScorePlayMaster.Instance.Stop();
//        }

        if (mode != Mode.TestSequence)
        {
            testSequence = false;
        }
        
        if (mode == Mode.RandomColor)
        {
            if (currentMode != mode)
            {
                nextRotationTime = 0;
            }

            if (nextRotationTime < Time.time)
            {
                nextRotationTime = Time.time + RotationTime;
                foreach (CloudLamp lamp in lamps)
                {
                    lamp.intaractive = true;
                    lamp.col = RandomColor();
                }
            }
        } else if (mode == Mode.RandomAuto)
        {
            if (currentMode != mode)
            {
                nextRotationTime = 0;
                randomColors = new Color[lamps.Length];
            }
            
            if (nextRotationTime < Time.time)
            {
                nextRotationTime = Time.time + RotationTime;
                for (int i = 0; i < randomColors.Length; i++)
                {
                    randomColors [i] = RandomColor();
                }
            }
            
            for (int i = 0; i < lamps.Length; i++)
            {
                CloudLamp lamp = lamps [i];

                lamp.intaractive = false;
                lamp.col = Color.Lerp(lamp.col, randomColors [i], (Time.time + RotationTime - nextRotationTime) / RotationTime);
            }
            
        } else if (mode == Mode.ColorRotation)
        {
            if (currentMode != mode)
            {
                nextRotationTime = 0;
                nextColor = RandomColor();
            }

            if (nextRotationTime < Time.time)
            {
                nextRotationTime = Time.time + RotationTime;
                currentColor = nextColor;
                nextColor = RandomColor();
            }

            foreach (CloudLamp lamp in lamps)
            {
                lamp.intaractive = true;
                lamp.col = Color.Lerp(currentColor, nextColor, (Time.time + RotationTime - nextRotationTime) / RotationTime);
            }

        } else if (mode == Mode.OneColor)
        {
            foreach (CloudLamp lamp in lamps)
            {
                lamp.intaractive = false;
                lamp.col = oneColor;
            }
        } else if (mode == Mode.TestSequence)
        {
            if (!testSequence)
            {
                testSequence = true;
                StartCoroutine(TestSequence());
            }
        } else if (mode == Mode.Sequence)
        {
            if (nextRotationTime < Time.time)
            {
                nextRotationTime = Time.time + RotationTime; 
                currentColor = nextColor;
                nextColor = RandomColor();
            }

            foreach (CloudLamp lamp in lamps)
            {
                float val = (Mathf.Sin(Time.time / RotationTime) + 1) / 2f;

                lamp.intaractive = false;
                lamp.col = Color.Lerp(currentColor, nextColor, 
                    (Time.time + RotationTime - nextRotationTime) / RotationTime
                );
            }
        } else if (mode == Mode.ScorePlay)
        {   
            foreach (CloudLamp lamp in lamps)
            {
                lamp.intaractive = false;
            }
            
            ScorePlayMaster.Instance.Play();
        } else if (mode == Mode.None)
		{
			foreach (CloudLamp lamp in lamps) {
				lamp.col = Color.black;
				lamp.intaractive = false;
			}
        } else if (mode == Mode.Custom)
        {
            if (customMode != null)
            {
                customMode.Invoke(this);
            }
        }


        currentMode = mode;
    }
}
