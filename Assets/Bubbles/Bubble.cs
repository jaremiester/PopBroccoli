using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

	public int points = 100;

	LevelManager levelManager;

	void Start(){
		levelManager = FindObjectOfType<LevelManager> ();
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.GetComponent<GrabbableObject> ()) {
			
			levelManager.score += points;

			Destroy (this.transform.parent.gameObject);

		}
	}
}
