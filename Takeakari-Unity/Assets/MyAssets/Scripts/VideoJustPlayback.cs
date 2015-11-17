using UnityEngine;
using System.Collections;

public class VideoJustPlayback : MonoBehaviour
{

    public MovieTexture movTexture;

    public Vector2 size;

    void Start()
    {
        GetComponent<GUITexture>().texture = movTexture;
        //renderer.material.mainTexture = movTexture;
        movTexture.Stop();
        movTexture.Play();
    }

    void Update(){

        //print(size.x / Screen.width);
        //transform.localScale = new Vector3(Screen.width/size.x, 1, 1);

        float s = Screen.width / size.x;


        GetComponent<GUITexture>().pixelInset = new Rect(-size.x*s / 2, -size.y*s / 2, size.x*s, size.y*s);

    }
   
}
