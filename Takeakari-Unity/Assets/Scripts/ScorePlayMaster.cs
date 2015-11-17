using UnityEngine;
using System.Collections;

 public class ScorePlayMaster : SingletonMonoBehaviour<ScorePlayMaster> {

    public ScorePlayer score;
    private ScorePlayer currentPlaying;

    public ScorePlayer[] scores;

    public ColorModeChanger cmc;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
         
	}

    public void SetScore(int i){
        if (i >= scores.Length)
            return;

        cmc.mode = ColorModeChanger.Mode.ScorePlay;
        score = scores [i];
        Play();
    }

    public void Play(){
        if (currentPlaying && currentPlaying != score)
        {
            currentPlaying.Stop();
        }

        currentPlaying = score;
        currentPlaying.Play();
    }

    public void Stop(){
        if (currentPlaying )
        {
            currentPlaying.Stop();
            currentPlaying = null;
        }

    }
}
