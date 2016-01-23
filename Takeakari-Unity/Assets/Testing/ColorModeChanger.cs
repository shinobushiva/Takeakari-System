using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ColorModeChanger : MonoBehaviour
{
    public CloudLamp[] lamps;
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
    }
    public Mode mode;
    private Mode currentMode;

    public void SetNone()
    {
        mode = Mode.None;
    }

    public void SetTestSequence()
    {
        mode = Mode.TestSequence;
    }

    public void SetRandomColor()
    {
        mode = Mode.RandomColor;
    }

    public void SetRotationColor()
    {
        mode = Mode.ColorRotation;
    }

    public void SetSequentialColor()
    {
        mode = Mode.Sequence;
    }

    public void SetOneColor()
    {
        mode = Mode.OneColor;
    }

    public void SetScorePlay()
    {
        mode = Mode.ScorePlay;
    }


    public void SetRandomAuto(){
        mode = Mode.RandomAuto;
    }

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
 
    //private float rotationTime = 1f;
    private float nextRotationTime = 0f;
    private Color currentColor; 
    private Color nextColor;
    public Color oneColor = Color.blue;

    // Use this for initialization
    void Start()
    {

        lamps = FindObjectsOfType(typeof(CloudLamp)) as CloudLamp[]; 
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
                for(int i = 0; i<randomColors.Length; i++){
                    randomColors[i] = RandomColor();
                }
            }
            
            for (int i=0;i<lamps.Length;i++)
            {
                CloudLamp lamp = lamps[i];

                lamp.intaractive = false;
                lamp.col = Color.Lerp(lamp.col, randomColors[i], (Time.time + RotationTime - nextRotationTime) / RotationTime);
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
        }
        else if (mode == Mode.ScorePlay)
        {   
            foreach (CloudLamp lamp in lamps)
            {
                lamp.intaractive = false;
            }
            
            ScorePlayMaster.Instance.Play();
        }
        else if (mode == Mode.None)
        {
        }


        currentMode = mode;
    }
}
