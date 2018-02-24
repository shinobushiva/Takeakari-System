using UnityEngine;
using System;
using System.Collections;

// https://qiita.com/naoK/items/55fb18bd348cfaa92708
public class GlobalCoroutine : MonoBehaviour {

	public static void Go (IEnumerator coroutine) {
		GameObject obj = new GameObject ();     // コルーチン実行用オブジェクト作成
		obj.name = "GlobalCoroutine";

		GlobalCoroutine component = obj.AddComponent <GlobalCoroutine> ();
		if (component != null) {
			component.StartCoroutine (component.Do (coroutine));
		}
	}

	IEnumerator Do (IEnumerator src) {
		while (src.MoveNext ()) {               // コルーチンの終了を待つ
			yield return null;
		}

		Destroy (this.gameObject);              // コルーチン実行用オブジェクトを破棄
	}
}
