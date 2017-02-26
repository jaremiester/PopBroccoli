using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartBox : MonoBehaviour {


	Vector3 launchVelocity;

	void Start () {
		launchVelocity = this.transform.up * 10f;

	}
		

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.GetComponent<GrabbableObject>()){
			//shouldn't have to destroy dart just reset pos/rot/vel/angvel
			GrabbableObject dart = collider.gameObject.GetComponent<GrabbableObject>();
			dart.gameObject.transform.position = this.gameObject.transform.GetChild (0).transform.position;
			dart.gameObject.transform.rotation = this.gameObject.transform.rotation;
			dart.rigidBody.velocity = launchVelocity;
			dart.rigidBody.angularVelocity = Vector3.zero;
		}
	}
}
