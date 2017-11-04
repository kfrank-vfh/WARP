using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpController : MonoBehaviour {

	private GameObject cameraObject;
	private GameObject particleObject;

	private ParticleSystem particleComponent;


	// Use this for initialization
	void Start () {
		cameraObject = GameObject.Find("Player/FirstPerson/Camera");
		particleObject = GameObject.Find("WarpTargetMarker");
		particleComponent = particleObject.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		bool rightMouseButtonClicked = Input.GetAxis("Fire2") == 1.0f;
		RaycastHit hit;
		if(!rightMouseButtonClicked) {
			updateParticleSystem(null);
			return;
		}
		hit = doRaycast();
		updateParticleSystem(hit);
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
}
