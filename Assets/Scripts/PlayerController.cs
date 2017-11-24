using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// PUBLIC CONFIGURATION VARIABLES
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	// PRIVATE STATE VARIABLES
	private State state = State.DISABLED;
	private Vector3 moveDirection = Vector3.zero;
	private bool leftMouseReleased = true;
	private List<RaycastHit> warpPath;

	// GAME OBJECTS
	private GameObject player;
	private GameObject cameraObject;
	private GameObject particleObject;

	// COMPONENTS
	private CharacterController controller;
	private ParticleSystem particleComponent;

	void Start () {
		// init game objects
		player = GameObject.Find("Player");
		cameraObject = player.transform.Find("FirstPerson/Camera").gameObject;
		particleObject = GameObject.Find("WarpTargetMarker");
		// init components
		controller = GetComponent<CharacterController>();
		particleComponent = particleObject.GetComponent<ParticleSystem>();
		// set initial state
		state = State.MOVING;
	}
	
	void Update () {
		// always process application quit input
		processApplicationQuit();
		// process moving input if state = MOVING
		if(state == State.MOVING) {
			processMovingInput();
			processWarpInput();
		}
		// process warping if state = WARPING
		if(state == State.WARPING) {
			executeWarp(warpPath[warpPath.Count-1]);
			state = State.MOVING;
		}
	}

	private void processApplicationQuit() {
		if(Input.GetKeyDown("escape")) {
			Application.Quit();
		}
	}

	private void processMovingInput() {
		if(controller.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if(Input.GetButton("Jump")) {
				moveDirection.y = jumpSpeed;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}

	private void processWarpInput() {
		// check if right mouse button is pressed
		bool rightMouseButtonClicked = Input.GetAxis("Fire2") == 1.0f;
		if(!rightMouseButtonClicked) {
			// if not, deactivate particle system
			updateParticleSystem(new List<RaycastHit>());
			return;
		}
		// else do raycast and update particle system
		List<RaycastHit> rayPath = doRaycast(cameraObject.transform.position, cameraObject.transform.forward);
		updateParticleSystem(rayPath);
		// check if left mouse button is pressed
		bool leftMouseButtonClicked = Input.GetAxis("Fire1") == 1.0f;
		if(!leftMouseButtonClicked) {
			leftMouseReleased = true;
			return;
		}
		RaycastHit targetPoint = rayPath[rayPath.Count - 1];
		if(!leftMouseReleased || targetPoint.transform.gameObject.tag != "WarpGround") {
			return;
		}
		leftMouseReleased = false;
		// execute warp
		warpPath = rayPath;
		state = State.WARPING;
	}

	private List<RaycastHit> doRaycast(Vector3 origin, Vector3 direction) {
		// do raycast
		RaycastHit hit;
		Physics.Raycast(origin, direction, out hit);
		// check if ray hit something
		if(hit.collider == null) {
			return null;
		}
		// create ray path list and add current ray hit
		List<RaycastHit> rayPath = new List<RaycastHit>();
		rayPath.Add(hit);
		// check if ray hit mirror
		if(hit.transform.gameObject.tag == "Mirrors") {
			// do raycast again with reflected ray
			Vector3 reflectedDirection = Vector3.Reflect(direction, hit.normal);
			List<RaycastHit> followingHits = doRaycast(hit.point, reflectedDirection);
			// add all following hits to ray path list
			rayPath.AddRange(followingHits);
		}
		// else return ray path list
		return rayPath;
	}

	private void updateParticleSystem(List<RaycastHit> rayPath) {
		// check if raycast hit exists
		if(rayPath.Count == 0) {
			// if not, stop particle system
			stopParticleSystem();
		} else {
			// else start particle system
			startParticleSystem();
			// set position and rotation
			RaycastHit hit = rayPath[rayPath.Count - 1];
			particleObject.transform.position = hit.point;
			particleObject.transform.rotation = Quaternion.LookRotation(hit.normal);
			// set particle color
			ParticleSystem.MainModule main = particleComponent.main;
			main.startColor = hit.transform.gameObject.tag == "WarpGround" ? Color.green : Color.red;
		}
	}

	private void startParticleSystem() {
		if(!particleComponent.isPlaying) {
			particleComponent.Play();
		}
	}

	private void stopParticleSystem() {
		if(particleComponent.isPlaying) {
			particleComponent.Stop();
		}
	}

	private void executeWarp(RaycastHit hit) {
		if(hit.collider == null) {
			return;
		}
		player.transform.position = new Vector3(hit.point.x, hit.point.y + 1.0f, hit.point.z);
	}

	enum State {
		DISABLED, MOVING, WARPING
	}
}
