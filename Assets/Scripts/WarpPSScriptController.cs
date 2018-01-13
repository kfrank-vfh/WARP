using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPSScriptController : MonoBehaviour {

	private GameObject warpPS;
	private Animator warpPSAnimator;
	private ParticleSystem ps;
	private PlayerController playerController;
	private GameObject cameraObject;
	private FPCameraController cameraController;
	private TV_AnimationController monitorController;
	private bool animationStarted = false;

	// Use this for initialization
	void Start () {
		warpPS = transform.parent.gameObject;
		warpPSAnimator = warpPS.GetComponent<Animator>();
		ps = warpPS.GetComponent<ParticleSystem>();
		playerController = GameObject.Find("/Player").GetComponent<PlayerController>();
		cameraObject = GameObject.Find("/Player/FirstPerson/Camera");
		monitorController = GameObject.Find("/Rooms/Room3/Monitor4").GetComponent<TV_AnimationController>();
		cameraController = cameraObject.GetComponent<FPCameraController>();
	}

	void OnTriggerEnter(Collider other) {
		OnTriggerStay(other);
	}

	void OnTriggerStay(Collider other) {
		// check if animation already started
		if(animationStarted) {
			return;
		}

		animationStarted = true;
		playerController.pauseGame(true);
		StartCoroutine(warpPSCoroutine());
	}

	private IEnumerator warpPSCoroutine() {
		cameraController.lerpTo(warpPS.transform.position, 1f);
		yield return new WaitForSeconds(1f);
		warpPSAnimator.SetTrigger("animate");
		yield return new WaitForSeconds(2.5f);
		ps.Stop();
		yield return new WaitForSeconds(1f);
		Destroy(warpPS);
		playerController.enableWarping();
		playerController.pauseGame(false);
		monitorController.startTvAnimation();
	}
}
