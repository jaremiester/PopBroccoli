using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameText : MonoBehaviour {

	Text text;
	LevelManager levelManager;
	string currentScore;


	void Start () {
		text = GetComponent<Text> ();
		levelManager = FindObjectOfType<LevelManager> ();
	}

	void Update () {
		currentScore = levelManager.score.ToString ();
		text.text = "Bits\n" + currentScore;
	}
}
