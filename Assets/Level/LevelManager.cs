using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public int score = 0;

	public int currentStage;
	int totalStages = 3;

	public GameObject player;
	public Hand rightHand;
	GameObject items;

	public GameObject stage1Orbs;
	public GameObject stage2Orbs;
	public GameObject stage3Orbs;
	GameObject currentStageOrbs;

	Vector3 stage2Position = new Vector3(-13f, 2f, 30f);
	Vector3 stage3Position = new Vector3(8f, 4f, 68f);

	Vector3 teleportLocation;
	bool isFading = false;
	float fadeTime = 0.5f;
	float currentFadeTime = 0f;

	void Start () {
		currentStage = 1;
		currentStageOrbs = stage1Orbs;
	}

	void Update () {
		if (isFading) {
			if (currentFadeTime < fadeTime) {
				currentFadeTime += Time.deltaTime;
			} else {
				player.transform.position = teleportLocation;
				SteamVR_Fade.Start (Color.clear, fadeTime);
				isFading = false;
				currentFadeTime = 0f;
			}
		}

		if (currentStageOrbs.transform.childCount == 0) {
			MoveToNextStage ();
		}
	}


	void MoveToNextStage(){
		currentStage += 1;
		if (currentStage > totalStages) {
			print ("Course Completed!");
		} else if (currentStage == 2) {
			currentStageOrbs = stage2Orbs;
			teleportLocation = stage2Position;
			SteamVR_Fade.Start (Color.black, fadeTime);
			isFading = true;
		} else if (currentStage == 3) {
			currentStageOrbs = stage3Orbs;
			teleportLocation = stage3Position;
			SteamVR_Fade.Start (Color.black, fadeTime);
			isFading = true;
		}
			

	}

}

