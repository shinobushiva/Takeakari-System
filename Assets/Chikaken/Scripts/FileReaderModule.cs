using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class FileReaderModule : ReaderModule
{

	public string filename = "localhost:33333";
	private string csv;

	public override void Initialize ()
	{
		csv = "";

		Debug.Log ("Loading Data from server...the server may not be responding.");
		string[] ss = filename.Split (':');
		csv = ReadFromServer (ss [0], int.Parse (ss [1]));
		Debug.Log ("Data is Loaded from server. Press 'Start' to begin ");
	}

	public override void Read (Module[] modules)
	{
	
		StringReader sr = new StringReader (csv);
		
		int idx = 0;
		string line = null;
		while ((line = sr.ReadLine()) != null) {
			int c = 0;
			string[] ss = line.Split (',');
			
			if (idx >= modules.Length)
				continue;
			
			Module m = modules [idx];
			m.moduleName = ss [c++];
			
			int num = (ss.Length - 1) / 4;
			m.partials = new Module.Partial[num];
			
			for (int i=0; i<num; i++) {
				Module.Partial p = new Module.Partial ();
				p.start = int.Parse (ss [c++]);
				
				p.sColor = ToColor (int.Parse (ss [c++]));
				p.end = int.Parse (ss [c++]);
				p.eColor = ToColor (int.Parse (ss [c++]));
				m.partials [i] = p;
			}
			
			idx++;
		}
	}

	private string ReadFromServer (string host, int port)
	{
		
		//TcpClientを作成し、サーバーと接続する
		System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient (host, port);
		
		//NetworkStreamを取得する
		System.Net.Sockets.NetworkStream ns = tcp.GetStream ();
		
		//サーバーから送られたデータを受信する
		System.IO.MemoryStream ms = new System.IO.MemoryStream ();
		byte[] resBytes = new byte[256];
		do {
			//データの一部を受信する
			int resSize = ns.Read (resBytes, 0, resBytes.Length);
			//Readが0を返した時はサーバーが切断したと判断
			if (resSize == 0) {
				Debug.Log ("サーバーが切断しました。");
				break;
			}
			//受信したデータを蓄積する
			ms.Write (resBytes, 0, resSize);
		} while (ns.DataAvailable);
		//受信したデータを文字列に変換
		System.Text.Encoding enc = System.Text.Encoding.UTF8;
		string resMsg = enc.GetString (ms.ToArray ());
		ms.Close ();
		//閉じる
		ns.Close ();
		tcp.Close ();
		
		return resMsg;
	}


}
