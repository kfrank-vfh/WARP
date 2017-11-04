using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpController : MonoBehaviour {

	private GameObject player;
	private GameObject cameraObject;
	private GameObject particleObject;

	private ParticleSystem particleComponent;

	private bool leftMouseReleased = true;


	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		cameraObject = player.transform.Find("FirstPerson/Camera").gameObject;
		particleObject = GameObject.Find("WarpTargetMarker");
		particleComponent = particleObject.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		// check if richt mouse button is pressed
		bool rightMouseButtonClicked = Input.GetAxis("Fire2") == 1.0f;
		RaycastHit hit;
		if(!rightMouseButtonClicked) {
			// if not, deactivate particle system
			updateParticleSystem(null);
			return;
		}
		// else do raycast and update particle system
		hit = doRaycast();
		updateParticleSystem(hit);
		// check if left mouse button is pressed
		bool leftMouseButtonClicked = Input.GetAxis("Fire1") == 1.0f;
		if(!leftMouseButtonClicked) {
			leftMouseReleased = true;
			return;
		}
		if(!leftMouseReleased) {
			return;
		}
		leftMouseReleased = false;
		// execute warp
		executeWarp(hit);
	}

	private RaycastHit doRaycast() {
		Vector3 origin = cameraObject.transform.position;
		Vector3 direction = cameraObject.transform.forward;
		RaycastHit hit;
		Physics.Raycast(origin, direction, out hit);
		return hit;
	}

	private void updateParticleSystem(RaycastHit? hit) {
		// start or stop particle system
		if(!hit.HasValue || hit.Value.collider == null) {
			stopParticleSystem();
			return;
		}
		startParticleSystem();
		// set position and rotation
		particleObject.transform.position = hit.Value.point;
		particleObject.transform.rotation = Quaternion.LookRotation(hit.Value.normal);
		// set particle color
		ParticleSystem.MainModule main = particleComponent.main;
		main.startColor = hit.Value.transform.gameObject.tag == "WarpGround" ? Color.green : Color.red;
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
}
