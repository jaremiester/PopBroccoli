using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

	Text text;

	void Start () {
		text = GetComponent<Text> ();
	}

	void Update () {
		float fps = Mathf.Round (1f / Time.smoothDeltaTime);
		text.text = fps.ToString ();
	}
}
