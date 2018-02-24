using UnityEngine;
using System.Collections;
using WebSocketSharp;
using MiniJSON;
using System.Collections.Generic;

public class WebSocketConnect : MonoBehaviour {

    public string url = "https://damp-basin-9745.herokuapp.com/";
    private WebSocket ws ;

    public CloudLamp lamp;
    public float divBy = 100000f;

    public float value;

	// Use this for initialization
	void Start () {
        Connect();
	}
	
    
    private Color toColor = Color.black;

	// Update is called once per frame
	void Update () {
        lamp.col = Color.Lerp(lamp.col, toColor, 0.5f);
	}

    public Color[] colors = new Color[]{Color.black, Color.blue, Color.green, Color.red};

    private Color GetColor(float val){

        val = Mathf.Clamp(val, 0, 1);

        for (int i=0; i<colors.Length-1; i++)
        {
            float unit = (1f/(colors.Length-1));
            if(val < unit * (i+1)){
                return Color.Lerp(colors[i], colors[i+1], (val-unit*i)/unit );
            }
        }

        return Color.black;
    }


    void Connect(){
        //接続
        //ポートの指定はしなくても大丈夫だった。80で接続してるはず。
        ws =  new WebSocket(url);

        ws.OnOpen += (sender, e) => {
            //接続したら自分を制御用PCとしてサーバーに教える
            //SendSetPCMessage();
            print("Connected");
        };
        ws.OnMessage += (sender, e) => {
            string s = e.Data;
            //Debug.Log (s);
            Dictionary<string, object> dict = Json.Deserialize (s) as Dictionary<string, object>;
            //Debug.Log (dict["power"].GetType() );
            long val = (long)dict["power"];
            //Debug.Log ("val="+val);
            toColor = GetColor(val/divBy);

            value =  val/divBy;
        };
        ws.Connect();

        ws.OnClose += (sender, e) => {
            print("Closed");
        };

        ws.OnError += (sender, e) => {
            print("Error : " + e.Message);
        };
    }

    void Disconnect(){
        ws.Close();
    }

    void OnDestroy(){
        Disconnect();
    }

}
