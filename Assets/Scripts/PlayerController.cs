using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;

public class PlayerController : MonoBehaviour {

	// PUBLIC CONFIGURATION VARIABLES
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	// PRIVATE CONFIGURATION VARIABLES
	private float warpTime = 0.3f;
	private float minWarpSpeed = 20.0f;

	// PRIVATE STATE VARIABLES
	private State state = State.DISABLED;
	private State previousState;
	private Vector3 moveDirection = Vector3.zero;
	private bool leftMouseReleased = true;
	private List<RaycastHit> warpPath;
	private Dictionary<RaycastHit, Vector3> targetPositions;
	private float warpSpeed;
	private bool warpEnabled = false;

	// GAME OBJECTS
	private GameObject player;
	private GameObject cameraObject;
	private GameObject particleObject;

	// COMPONENTS
	private FPCameraController cameraController;
	private CharacterController characterController;
	private ParticleSystem particleComponent;
	private Kino.Motion motionBlurComponent;

	void Start () {
		// init game objects
		player = GameObject.Find("Player");
		cameraObject = player.transform.Find("FirstPerson/Camera").gameObject;
		particleObject = GameObject.Find("WarpTargetMarker");
		// init components
		cameraController = cameraObject.GetComponent<FPCameraController>();
		characterController = GetComponent<CharacterController>();
		particleComponent = particleObject.GetComponent<ParticleSystem>();
		motionBlurComponent = cameraObject.GetComponent<Kino.Motion>();
		// set initial state
		state = State.MOVING;
		motionBlurComponent.enabled = false;
	}
	
	void Update () {
		// process moving input if state = MOVING
		if(state == State.MOVING) {
			processMovingInput();
			if(warpEnabled) {
				processWarpInput();
			}
		}
		// process warping if state = WARPING
		if(state == State.WARPING) {
			processWarping();
			//executeWarp(warpPath[warpPath.Count-1]);
			//state = State.MOVING;
		}
	}

	private void processMovingInput() {
		if(characterController.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if(Input.GetButton("Jump")) {
				moveDirection.y = jumpSpeed;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		characterController.Move(moveDirection * Time.deltaTime);
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
		setWarpPath(rayPath);
		state = State.WARPING;
		motionBlurComponent.enabled = true;
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

	private void setWarpPath(List<RaycastHit> rayPath) {
		warpPath = rayPath;
		// calculate target positions to warp path and warp speed to distance
		float distance = 0.0f;
		targetPositions = new Dictionary<RaycastHit, Vector3>();
		foreach (RaycastHit rayHit in warpPath) {
			// sum up distance
			distance += rayHit.distance;
			// calculate the expected target point
			Vector3 targetPoint = rayHit.point;
			targetPoint += rayHit.normal.y > 0 ? Vector3.up : (rayHit.normal.y < 0 ? Vector3.down : Vector3.zero);
			// check if target point is to close to wall and correct
			foreach (Vector3 direction in new Vector3[] {Vector3.forward, Vector3.back, Vector3.right, Vector3.left}) {
				RaycastHit hit;
				Physics.Raycast(targetPoint, direction, out hit, characterController.radius);
				if(hit.collider != null && hit.transform.gameObject.tag != "Mirrors") {
					targetPoint = hit.point - direction;
				}
			}
			// add target point to position list
			targetPositions.Add(rayHit, targetPoint);
		}
		warpSpeed = distance / warpTime;
		warpSpeed = warpSpeed < minWarpSpeed ? minWarpSpeed : warpSpeed;
	}

	private void processWarping() {
		// check if there is a next warp point
		if(warpPath.Count > 0) {
			// if so, move to the next
			Vector3 targetPosition = targetPositions[warpPath[0]];
			Vector3 playerPosition = player.transform.position;
			Vector3 moveDirection = targetPosition - playerPosition;
			if(moveDirection.magnitude > warpSpeed * Time.deltaTime) {
				moveDirection = moveDirection.normalized * warpSpeed * Time.deltaTime;
			}
			characterController.Move(moveDirection);
		} else {
			// else end the warp
			state = State.MOVING;
			motionBlurComponent.enabled = false;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		// do nothing if not warping
		if(state != State.WARPING) {
			return;
		}
		// check if the game object of the next warp target was hit
		RaycastHit warpPoint = warpPath[0];
		GameObject hitObject = hit.transform.gameObject;
		if(warpPoint.transform.gameObject != hitObject) {
			return;
		}
		// set max distance depending on hit object
		float maxDistance = hitObject.tag == "Mirrors" ? Mathf.Infinity : 0.1f;
		// calculate distance to target position
		Vector3 targetPosition = targetPositions[warpPoint];
		float distance = Vector3.Distance(player.transform.position, targetPosition);
		// check if close enough to the target position
		if(distance < maxDistance) {
			// remove current warp point
			targetPositions.Remove(warpPoint);
			warpPath.RemoveAt(0);
			// adjust camera look if there is a next point
			if(warpPath.Count > 0) {
				targetPosition = targetPositions[warpPath[0]];
				Vector3 lookAtPosition = targetPosition + Vector3.up;
				cameraController.lookAt(lookAtPosition);
			}
		}
	}

	public void enableWarping() {
		warpEnabled = true;
	}

	public void pauseGame(bool pause) {
		if(pause && state != State.DISABLED) {
			//Time.timeScale = 0f;
			previousState = state;
			state = State.DISABLED;
			cameraController.pause(true);
		} else if(!pause && state == State.DISABLED) {
			//Time.timeScale = 1f;
			state = previousState;
			cameraController.pause(false);
		}
	}

	enum State {
		DISABLED, MOVING, WARPING
	}
}
