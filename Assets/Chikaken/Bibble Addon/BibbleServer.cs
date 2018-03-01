using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Events;

public class BibbleServer : MonoBehaviour
{
    
    private IPAddress ipAdd = IPAddress.Parse("127.0.0.1");
    public int port = 33334;

    private bool running = false;
    private Thread thread;
    private NetworkStream ns;
    private TcpListener listener;

    public Dictionary<string, BibbleListContent> bibbleMap = new Dictionary<string, BibbleListContent>();
    public Queue<ReceivedData> queue = new Queue<ReceivedData>();

    public Toggle toggle;
    public RectTransform scrollVieport;
    public BibbleListContent bibbleListContentPrefab;

    public DataReceiveEvent onDataReceived;
	public BibbleServerEvent onBibbleServer;

    [System.Serializable]
    public class DataReceiveEvent : UnityEvent<ReceivedData>
    {
    }

	[System.Serializable]
	public class BibbleServerEvent : UnityEvent<bool>
	{
	}


    [System.Serializable]
    public class ReceivedData {
        public string uuid;
        public float distance;
        public int button;

        public override string ToString()
        {
            return string.Format("uuid={0}, distance={1}, button={2}", uuid, distance, button);
        }
    }

    // Use this for initialization
    void Start()
    {
        toggle.isOn = running;

        if (onDataReceived == null)
        {
            onDataReceived = new DataReceiveEvent();
        }

		if (onBibbleServer == null) {
			onBibbleServer = new BibbleServerEvent ();
		}

    }

    void Update() {
        while (queue.Count > 0) {
            ReceivedData rd = queue.Dequeue();
            onDataReceived.Invoke(rd);

            BibbleListContent blc = null;
            if (!bibbleMap.ContainsKey(rd.uuid))
            {
                blc = GameObject.Instantiate<BibbleListContent>(bibbleListContentPrefab);
                blc.transform.SetParent(scrollVieport.transform, false);

                bibbleMap.Add(rd.uuid, blc);
            } else
            {
                blc = bibbleMap [rd.uuid];
            }

            blc.Set(rd.uuid, rd.distance, rd.button);
                
        }
    }


    void OnApplicationQuit() {
        if (thread != null)
        {
            thread.Abort(); 
        }
    }

    public void StartBibbleServer (){
        if (!running) {
            running = true;
            toggle.isOn = running;

			foreach (BibbleListContent blc in bibbleMap.Values) {
				Destroy (blc.gameObject);
			}
            bibbleMap.Clear();

            thread = new Thread(BibbleServerRoutine);
            thread.Start();
			onBibbleServer.Invoke (true);
        }
    }

    public void StopBibbleServer (){

        if (running)
        {
            running = false;
            toggle.isOn = running;

            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
            if (ns != null)
            {
                ns.Close();
                ns = null;
            }
			onBibbleServer.Invoke (false);
            Debug.Log("サーバーを終了しました");
        }
    }

    public void ToggleServer (bool b) {
        if (!b)
        {
            StopBibbleServer();
        } else
        {
            StartBibbleServer();
        }
    }
        

    void BibbleServerRoutine () {
        //Tcpサーバーerオブジェクトを作成する
        listener = new TcpListener(ipAdd, port);

        //サーバーを開始する
        listener.Start();
        Debug.Log(String.Format("サーバーを開始しました({0}:{1})",
            ((System.Net.IPEndPoint)listener.LocalEndpoint).Address,
            ((System.Net.IPEndPoint)listener.LocalEndpoint).Port));

        //接続要求があったら受け入れる
        System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();
        Debug.Log(String.Format("クライアント({0}:{1})と接続しました",
            ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address,
            ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port));

        //NetworkStreamを取得
        ns = client.GetStream();

        //クライアントから送られたデータを受信する
        System.Text.Encoding enc = System.Text.Encoding.UTF8;

        using(StreamReader reader = new StreamReader(ns, enc)) {
            while(reader.Peek() >= 0 && running) {
                
                string resMsg = reader.ReadLine();
                ReceivedData rd = JsonUtility.FromJson<ReceivedData>(resMsg);
//                print(String.Format("ReceivedData: {0},{1},{2}", rd.uuid, rd.distance, rd.button));
                queue.Enqueue(rd);
                 
                if(resMsg == "end"){
                    StopBibbleServer();
                    break;
                }
                Thread.Sleep(0);
            }
        }
        StopBibbleServer();
        Debug.Log("終了しました");

    }
        
}
