using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hand : MonoBehaviour {
	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
	private SteamVR_TrackedObject trackedObj;

	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
	private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;


	public GrabbableObject grabbedObject;

	//determining where grabbed object should be
	public Transform interactionPoint;

	GameObject items;
	GameObject inventoryMenu;
	GameObject storeMenu;
	LevelManager levelManager;

	//For Inventory
	string currentInventorySelection; //possible selections: "Dart", "Teleport", 
	int numberOfDarts;



	//For Teleportation
	GameObject projectionMarker;
	public Material markerMaterial;
	Ray raycast;
	RaycastHit hit;
	Vector3 newPos;
	float lineThickness = 0.001f;
	float lineLength = 4f;
	float distance = 50f;
	Vector3 defaultPosition;
	Vector3 defaultScale;
	bool isFading = false;
	float fadeTime = 0.3f;
	float currentFadeTime = 0f;


	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		grabbedObject = null;

		levelManager = FindObjectOfType<LevelManager> ();
		inventoryMenu = GameObject.FindGameObjectWithTag ("InventoryMenu");
		storeMenu = GameObject.FindGameObjectWithTag ("StoreMenu");
		inventoryMenu.SetActive (false);
		storeMenu.SetActive (false);
		items = GameObject.FindGameObjectWithTag ("Items");
		currentInventorySelection = "Dart";

		projectionMarker = GameObject.CreatePrimitive (PrimitiveType.Cube);
		projectionMarker.transform.parent = this.transform;
		projectionMarker.transform.localRotation = Quaternion.identity;
		defaultScale = new Vector3 (lineThickness, lineThickness, lineLength);
		projectionMarker.transform.localScale = defaultScale;
		defaultPosition = new Vector3(0f, 0f, lineLength/2f);
		projectionMarker.transform.localPosition = defaultPosition;

		MeshRenderer markerRenderer = projectionMarker.GetComponent<MeshRenderer> ();
		markerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		markerRenderer.receiveShadows = false;
		markerRenderer.material = markerMaterial;

		Destroy(projectionMarker.GetComponent<BoxCollider>());
		projectionMarker.SetActive (false);


	}

	void FixedUpdate(){
		if (isFading) {
			if (currentFadeTime < fadeTime) {
				currentFadeTime += Time.deltaTime;
			} else {
				this.transform.parent.root.transform.position = newPos;
				SteamVR_Fade.Start (Color.clear, fadeTime);
				isFading = false;
				currentFadeTime = 0f;
			}
		}

		//track grabbedObject with hand
		if (grabbedObject) { //if holding and object and this hand isn't the secondary hand; movement is handled only by primary hand
			Move ();
		}

		if (controller.GetPressDown(menuButton)){
			if (inventoryMenu.activeSelf) {
				inventoryMenu.SetActive (false);
			} else if(storeMenu.activeSelf){
				storeMenu.SetActive (false);
			} else {
				inventoryMenu.SetActive(true);
			}
		}

		if (inventoryMenu.activeSelf) {
			InventoryMenu ();
		} else if (storeMenu.activeSelf){
			StoreMenu ();
		} else if (controller.GetPressDown(touchPad)) {
			InventorySelect ();
		}



		//update teleportation ray
		if (currentInventorySelection == "Teleport") {
			UpdateTeleportRay ();
		}

		//check for item activations
		if (controller.GetPressDown (triggerButton) && currentInventorySelection != "Teleport") {
			SpawnItem ();
		}
		if (controller.GetPressUp (triggerButton)) {
			if (grabbedObject) {
				Drop ();
			}
		}
	}


	void Move(){
		float angle;
		Vector3 axis;
		Vector3 posDelta;
		Quaternion rotDelta;

		//update velocity
		posDelta = this.transform.position - interactionPoint.position; //find intended direction
		grabbedObject.rigidBody.velocity = posDelta * grabbedObject.velocityFactor * Time.fixedDeltaTime; //set velocity to move in said direction

		//update angular velocity
		rotDelta = this.transform.rotation * Quaternion.Inverse (interactionPoint.rotation); //find intended Quaternion rotation
		rotDelta.ToAngleAxis (out angle, out axis); //format Quaternion to angle and axis
		if (angle > 180) {
			angle -= 360;
		}

		if (angle != 0 && axis != Vector3.zero) { //is rotation already correct?
			grabbedObject.rigidBody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * grabbedObject.rotationFactor; //set angular velocity
		}
	}

	public void Grab(GrabbableObject grabObj){
		grabbedObject = grabObj;
		grabbedObject.attachedHand = this;
		grabbedObject.grabbedStatus = true;

		interactionPoint = new GameObject ().transform;
		interactionPoint.position = Vector3.zero;
		interactionPoint.rotation = Quaternion.identity;
		interactionPoint.SetParent (grabbedObject.transform, true);
		interactionPoint.localPosition = new Vector3 (0f, -0.35f, -5.5f);
		interactionPoint.localRotation = Quaternion.identity;
	}

	public void Drop(){
		grabbedObject.rigidBody.maxAngularVelocity = 2f;
		grabbedObject.attachedHand = null;
		grabbedObject.grabbedStatus = false;
		Destroy (interactionPoint.gameObject);
		grabbedObject = null;
	}





	void SpawnItem(){
		GameObject item = Instantiate (Resources.Load (currentInventorySelection)) as GameObject;
		item.transform.position = this.transform.position;
		item.transform.parent = items.transform;
		Grab (item.GetComponent<GrabbableObject> ());
	}


	void UpdateTeleportRay(){
		raycast = new Ray (this.transform.position, this.transform.forward);

		if (Physics.Raycast (raycast, out hit, distance) && hit.collider.gameObject.layer == 8) {
			projectionMarker.transform.localScale = new Vector3(lineThickness, lineThickness, hit.distance);
			projectionMarker.transform.localPosition = new Vector3 (0f, 0f, hit.distance / 2f);

			if (controller.GetPressDown(triggerButton)){
				SteamVR_Fade.Start(Color.black, fadeTime);
				isFading = true;
				newPos = new Vector3(hit.point.x, this.transform.parent.root.transform.position.y, hit.point.z);
			}
		} else {
			projectionMarker.transform.localScale = defaultScale;
			projectionMarker.transform.localPosition = defaultPosition;
		}
			

	}


	void InventorySelect(){

		projectionMarker.SetActive (false);

		Vector2 axis = controller.GetAxis (touchPad);
		float slope = axis.y / axis.x;
		float absVSlope = Mathf.Abs (slope);
		if (absVSlope < 1f) { //will be true if pressing left or right on touchPad
			if (axis.x > 0) { //pressing right

			} else { //pressing left

			}
		} else { // pressing up or down on touchPad
			if (axis.y > 0) { // pressing up
				currentInventorySelection = "Dart";
			} else { //pressing down
				currentInventorySelection = "Teleport";
				projectionMarker.SetActive (true);
			}
		}
	}

	void InventoryMenu(){
		if (controller.GetPressDown(touchPad)){
			inventoryMenu.SetActive(false);
			storeMenu.SetActive(true);
		}
	}

	void StoreMenu(){

	}
}
