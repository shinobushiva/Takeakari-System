using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ApproveCancelSetup : MonoBehaviour {

    public Toggle toggle;

	public string sceneToBack = "Start";
	public bool doQuit = false;

	// Use this for initialization
	void Start () {

        ApproveCancelUI ui = GetComponent<ApproveCancelUI>();

		string text = "スタート画面に戻りますか？";
		if (doQuit) {
			text = "終了しますか？";
		}
        ui.Set(text, "はい", "いいえ", 
        () => {
				if (doQuit) {
					Application.Quit();
				}else{
					SceneManager.LoadScene(sceneToBack);
				}
        },
        () => {
			gameObject.GetComponentInChildren<Canvas>().enabled = false;
			gameObject.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
            toggle.isOn = false;
        });
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
