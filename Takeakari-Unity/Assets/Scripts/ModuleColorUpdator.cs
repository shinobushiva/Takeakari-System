using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ModuleColorUpdator : MonoBehaviour {


    public string server = "localhost:33334";

    public ControlPanel cp;
    protected bool updating;

    //
    protected System.Net.Sockets.TcpClient tcp;
    protected System.Net.Sockets.NetworkStream ns;
    protected byte[] masks;
    protected byte[] buf;

    public bool isConnectedToServer = false;

    protected Module[] modules;

    public ModuleArray moduleArray;

    protected Dictionary<Module, Color> colorMap = new Dictionary<Module, Color> ();

	// Use this for initialization
	void Start () {
        StartCoroutine (UpdateModuleColors ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy ()
    {
        //SaveSettings ();
        DisconnectFromServer ();
    }


    public void ConnectToServer ()
    {
        modules = moduleArray.modules;

        foreach (Module m in modules) {
            if (m == null)
                continue;
            colorMap [m] = m.light.color;
        }
        
        try {
            string[] ss = server.Split (':');
            tcp = new System.Net.Sockets.TcpClient (ss [0], int.Parse (ss [1]));
            //NetworkStreamを取得する
            ns = tcp.GetStream ();
            
            masks = new byte[(int)(Mathf.Ceil (modules.Length / 7f))];
            buf = new byte[modules.Length * 5];
        } catch (Exception e) {
            print (e);
            cp.logText = "Could't connect to Server";
            return;
        }
        
        cp.logText = "Connection established!";
        isConnectedToServer = true;
    }
    
    public void DisconnectFromServer ()
    {
        print("Disconnect form Server");
        
        if (ns != null) {
            byte[] ba = new byte[]{0x65,0x6e,0x64};//System.Text.Encoding.Unicode.GetBytes ("end");
            ns.Write (ba, 0, ba.Length);
            ns.Flush();
            //閉じる
            ns.Close ();
            tcp.Close ();
            
            cp.logText = "Connection closed.";
        }
        
        isConnectedToServer = false;
    }


    public IEnumerator UpdateModuleColors ()
    {
        WaitForSeconds wfeof = new WaitForSeconds (33f/1000f);
        while (true) {
            if (cp.applyImmidiate) {
                
                //print("applyImmidiate");
                
                if (!updating) {
                    updating = true;
                    _UpdateModuleColors2 ();
                }
            }
            yield return wfeof;
        }
    }

    public int counterOffset = 0;

    public virtual int ModulesToBuffer()
    {
        int idx = 0;
        int counter = counterOffset;
        foreach (Module m in modules)
        {
            counter++;
            if (counter % 8 == 0)
            {
                counter++;
            }
            if (cp.useCompress && colorMap[m] == m.light.color)
                continue;
            /*
            if(UnityEngine.Random.Range(0,3) != 0)
                continue;
                */if (m == null)
                continue;
            Color col = m.light.color;
            colorMap[m] = col;
            buf[idx++] = 0x3F;
            buf[idx++] = (byte)(((byte)(col.r * 127)) | 0x80);
            buf[idx++] = (byte)(((byte)(col.g * 127)) | 0x80);
            buf[idx++] = (byte)(((byte)(col.b * 127)) | 0x80);
            //if(m.moduleId > 0){
            //    buf [idx++] = ((byte)(((byte)m.moduleId) | 0x80));
            //}else{
            //print(""+counter);

            if(m.moduleId < 0){
                buf[idx++] = ((byte)(((byte)counter) | 0x80));
            }else{
                buf[idx++] = ((byte)(((byte)m.moduleId) | 0x80));
            }
            //}
            
//            if(counter >= 34){
//                counter+= 56;
//            }
        }
        return idx;
    }
    
    private void _UpdateModuleColors2 ()    
    {
        
        //print("_UpdateModuleColors2:"+isConnectedToServer);
        
        if (!isConnectedToServer)
        {
            updating = false; 
            return;
        }
        
        //print("_UpdateModuleColors2");
        
        int len = ModulesToBuffer();
        //print("len:" + len);

        if (len > 0) {
            ns.Write (buf, 0, len);
            ns.Flush ();
            ns.ReadByte ();
        }
        
        updating = false;
    }
    

}
