using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

public class LightSensorInput : MonoBehaviour {

    public string port = "/dev/tty.usbmodem1411";

    SerialPort serialPort;

    string data = "";
    string onOff = "";

    public int sensorNum = 5;

    public int[] ths;
    public bool[] onOffs;

    private Thread thread;

	// Use this for initialization
	void Start () {

        onOffs = new bool[sensorNum];

        serialPort = new SerialPort(port, 9600);

        serialPort.ReadTimeout = 1000;
        serialPort.Parity = Parity.None;
        serialPort.StopBits = StopBits.One;
        serialPort.DataBits = 8;
        serialPort.DtrEnable = true;

//        thread = new Thread(Routine);
//        thread.Start();

        StartCoroutine(Routine());
	
	}

    void OnGUI(){
        GUI.Label (new Rect(100,100,200,100),"val:" + data );
        GUI.Label (new Rect(100,130,200,100),"onOff:" + onOff );
    }

    public bool flag = true;


    IEnumerator Routine(){
        
        try{
            serialPort.Open();
        }catch(Exception ex)
        {
//            return;
            print("Start Error Opening : " + ex.ToString());
        }
        
        flag = true;
        while (flag)
        {
            
            if (serialPort.IsOpen)
            {
                
                if (serialPort.BytesToRead > 0) {
                    data = serialPort.ReadLine();
                    //print(data);
                    
                    onOff = "";
                    
                    string[] dd = data.Split(',');
                    if(dd.Length >=sensorNum){
                        for(int i=0;i<sensorNum; i++){
                            onOffs[i] = (int.Parse(dd[i]) < ths[i]);
                            onOff += ""+onOffs[i]+",";
                        }
                    }
                }
                
            } else
            {
                if (serialPort != null)
                {
                    serialPort.Close();
                    serialPort.Open();
                }
                //print("Closing");
                flag = false;
            }
            yield return new WaitForSeconds(0f);
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy(){
        if(serialPort != null)
            serialPort.Close();

    }
}
