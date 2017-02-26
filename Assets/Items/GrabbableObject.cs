using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour {

	public float velocityFactor = 3500f;
	public float rotationFactor = 100f;
	public float maxVelocity;
	public float maxAngularVelocity = 20f;


	//can interact with these objects
	public Hand attachedHand;

	public Rigidbody rigidBody;

	//status bools
	public bool grabbedStatus;


	void Start(){
		rigidBody = GetComponent<Rigidbody> ();

		velocityFactor /= rigidBody.mass;
		rotationFactor /= rigidBody.mass;
		this.rigidBody.maxAngularVelocity = maxAngularVelocity;

	}
		
}
