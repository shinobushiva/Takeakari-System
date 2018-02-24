using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ScorePlayer : MonoBehaviour {

    [System.Serializable]
    public class Partial{
        public int num;
        public float startTime;
        public float duration;
        public Color color;
    }

    [System.Serializable]
    public class PartialRow
    {
        public int num;
        public Partial[] parts;
    }

    public PartialRow[] partials;

    private ModuleArray array;
    private ColorModeChanger cmc;


	// Use this for initialization
	void Start () {
        cmc = FindObjectOfType<ColorModeChanger>();
        array = FindObjectOfType<ModuleArray>();

	}
    bool playing = false;

    private void SetColor(Module m, Color c){
        m.LightColor = c;
        CloudLamp cl = m.GetComponent<CloudLamp>();
        if (cl)
        {
            cl.col = c;
        }
    }

    private IEnumerator _Play(){

        while (playing)
        {
            float startTime = Time.time;
            //print("st:"+startTime);
            float time = 0;

            float endTime = 0;

            foreach(PartialRow pr in partials){
                float v = pr.parts[pr.parts.Length-1].startTime+pr.parts[pr.parts.Length-1].duration;
                endTime = Mathf.Max(endTime, v);
            }

            foreach (Module m in array.modules)
            {
                SetColor(m, Color.black);
            }

            //print("Started");
            while (time <= endTime)
            {
                if(!playing)
                    break;

                foreach (PartialRow pr in partials)
                {
                    foreach (Partial p in pr.parts)
                    {
                        if (p.startTime <= time && p.startTime + p.duration > time)
                        {

                            //print("hit");
                            Module m = array.modules [p.num];
                            SetColor(m, p.color * ControlPanel.Instance.LightMultiplier);
                        }
                    }
                }
                time = (Time.time - startTime)/ControlPanel.Instance.Speed;
                yield return new WaitForEndOfFrame();
            }
            //print("t:"+time);
            //print("et:"+endTime);
        }
    }

    public void Play(){
        if (!playing)
        {
            playing = true;
            StartCoroutine(_Play());
        }
    }

    public void Stop(){
        playing = false;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    static Partial CreatePartial(string id, int s, int e, string[] cols)
    {
        Partial p = new Partial();
        p.num = int.Parse(id.Trim());
        float r = int.Parse(cols[0].Trim()) / 255f;
        float g = int.Parse(cols[1].Trim()) / 255f;
        float b = int.Parse(cols[2].Trim()) / 255f;
        p.color = new Color(r, g, b, 1f);
        p.startTime = s;
        p.duration = e - s;

        return p;
    }

    public void ReadScore(string score){
        string line = "";
        int maxCol = 0;
        int rowNum = 0;
        
        StringReader sr1 = new StringReader(score);
        while ((line = sr1.ReadLine()) != null)
        {
            string[] cells = line.Split(',');
            
            if(cells.Length <= 0 || cells[0].Contains("//"))
                continue;

            maxCol = Mathf.Max(maxCol, cells.Length);
            rowNum++;
        }
        sr1.Close();

        partials = new PartialRow[rowNum];

        StringReader sr = new StringReader(score);
         
        Dictionary<string, List<Partial>> dic = new Dictionary<string, List<Partial>>();

        int idx = 0;
        while ((line = sr.ReadLine()) != null)
        {
            //Debug.Log(line);
            string[] cells = line.Split(',');

            if(cells.Length <= 0 || cells[0].Contains("//"))
                continue;


            string id = cells[0];
            if(!dic.ContainsKey(id)){
                dic.Add(id, new List<Partial>());
            }

            for(int i=1;i<cells.Length;i++){
                if(cells[i].Trim().Length > 0){
                    if(i == cells.Length-1){
                        //last cell
                        Debug.Log(""+id+":"+i+"-"+maxCol);
                        Debug.Log(""+cells[i]);

                        string[] cols = cells[i].Split(';');
                        Partial p = CreatePartial(id, i-1, maxCol-1, cols);
                        dic[id].Add(p);

                    }else {
                        for(int j=i+1;i<cells.Length;j++){
                            if(cells[j].Trim().Length > 0 || j == cells.Length-1){

                                Debug.Log(""+id+":"+i+"-"+j);
                                Debug.Log(""+cells[i]);
                                
                                string[] cols = cells[i].Split(';'); 
                                Partial p = CreatePartial(id, i-1, j-1, cols); 
                                dic[id].Add(p);

                                i = j-1;
                                break;
                            }
                        }
                    }
                }
            }
        }

        for (int i=0; i<rowNum; i++)
        {
            partials[i] = new PartialRow();
            partials[i].num = i;
            partials[i].parts = dic[""+i].ToArray();
        }

 
    }
}
