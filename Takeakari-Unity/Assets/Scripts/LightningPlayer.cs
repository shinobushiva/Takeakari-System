using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LightningPlayer : MonoBehaviour {

    public Color[] rotation = new Color[]{Color.red, Color.yellow, Color.green, Color.blue, Color.black};
    private float rotationTime = 1;

    public LightSensorInput sensorInput;

    [System.Serializable]
    public class Partial{
        public float startTime;
        public float duration;
        public float pow;
    }

    [System.Serializable]
    public class PartialRow
    {
        public Partial[] parts;
    }

    public PartialRow[] partials;

    private ModuleArray array;
    private ColorModeChanger cmc;

    public Module m;

    public bool random = true;
    public float divBy = 50;

    public Color color = Color.yellow;


	// Use this for initialization
	void Start () {
        cmc = FindObjectOfType<ColorModeChanger>();
        array = FindObjectOfType<ModuleArray>();
        //m = GetComponent<Module>();

        rotationTime = Random.Range(1f, 5f);

        Play();

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

    public int num = 0;

    private IEnumerator _Play(){

//        float time = Time.time;

        while (playing)
        {
            if(sensorInput && !sensorInput.onOffs[3]){
                Color c = Color.white;
                SetColor(m, c);
                yield return new WaitForEndOfFrame();
                continue;
            }

//            if(time + rotationTime < Time.time){
//                color = 
//                time = Time.time;
//            }

            PartialRow pr = partials[num];//[Random.Range(0, partials.Length)];
            if(random){
                pr = partials[Random.Range(0, partials.Length)];
            }

            float startTime = Time.time;
            //print("st:"+startTime);
            float time = 0;

            Partial pe = pr.parts[pr.parts.Length-1];
            float endTime = pe.startTime+pe.duration;

            SetColor(m, Color.black);

            //print("Started");
            while (time <= endTime)
            {
                if(!playing)
                    break;

                foreach (Partial p in pr.parts)
                {
                    if (p.startTime <= time && p.startTime + p.duration > time)
                    {
                        SetColor(m, color * p.pow/divBy * ControlPanel.Instance.LightMultiplier);
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

    static Partial CreatePartial(float s, float d, float pow)
    {
        Partial p = new Partial();
        p.startTime = s;
        p.duration = d;
        p.pow = pow;

        return p;
    }

    public void ReadScore(string score){
        string line = "";
  
        List<PartialRow> rows = new List<PartialRow>();

        StringReader sr = new StringReader(score);
         
        int idx = 0;
        while ((line = sr.ReadLine()) != null)
        {
            Debug.Log(line);
            string[] cells = line.Split(',');

            List<string> cs = new List<string>();
            foreach(string c in cells){
                if(c.Trim().Length > 0)
                   cs.Add(c);
            }
            cells = cs.ToArray();
            print(cells.Length);

            if(cells.Length <= 0)
                continue;

            PartialRow row = new PartialRow();
            rows.Add(row);

            List<Partial> l = new List<Partial>();
            for(int i=0; i<cells.Length-2; i+=2){
                //print(""+cells[i]+":"+cells[i+1]);

                float t = float.Parse(cells[i].Trim());
                float pow = float.Parse(cells[i+1].Trim());
                float d = float.Parse(cells[i+2].Trim()) - t;
                Partial p = CreatePartial(t, d, pow);
                l.Add(p);
            }
            row.parts = l.ToArray();
        }

        partials = rows.ToArray();

    }
}
