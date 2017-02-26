using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour {

	void OnTriggerExit(Collider collider){
		Destroy (collider.gameObject);
	}

	void OnCollisionExit(Collision collision){
		Destroy (collision.gameObject);
	}
}
