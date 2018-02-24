using UnityEngine;
using System.Collections;

public class VideoPlayback : MonoBehaviour
{

    public MovieTexture movTexture;
    public Vector3 orgScale;
    public Quaternion orgRotation;
    private bool isPointed;
    private float pointedTime;
    public HealthBar bar;
    public float waitTime = 3f;
    public string nextScene;
    public float rotationSpeed = 30f;

    void Start()
    {

        orgRotation = transform.rotation;
        orgScale = transform.localScale;

        GetComponent<Renderer>().material.mainTexture = movTexture;
        movTexture.loop = true;
        movTexture.Play();

        UnPointed();
    }

    public void Pointed()
    {
        transform.localScale = orgScale * 1.5f;
        if (!isPointed)
        {
            isPointed = true;
            pointedTime = Time.time;
        }
    }
    
    public void UnPointed()
    {
        transform.localScale = orgScale;
        isPointed = false;

        bar.SetBar(1);
        bar.GetComponent<Renderer>().enabled = false;
    }

    void FixedUpdate(){
        if (!isPointed)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        } else
        {
            transform.rotation = orgRotation;
        }
    }

    void Update()
    {
       

        if (isPointed)
        {
            float f = (Time.time - pointedTime) / waitTime;
            bar.GetComponent<Renderer>().enabled = true;
            bar.SetBar(1- f);
            if (Time.time - pointedTime > waitTime)
            {
                Application.LoadLevel(nextScene);
            }
        }



    }

    void OnTriggerEnter(Collider c)
    {
        //print("Enter:" + c.gameObject.name);

        if (c.gameObject.GetComponent<IndexFingerTouch>())
        {
            print("Pointed");
            Pointed();
        }

    }

    void OnTriggerStay(Collider c)
    {
        //print("Stay:" + c.gameObject.name);
    }

    void OnTriggerExit(Collider c)
    {
        //print("Exit:" + c.gameObject.name);

        if (c.gameObject.GetComponent<IndexFingerTouch>())
        {
            print("UnPointed");
            UnPointed();
        }
    }
}
