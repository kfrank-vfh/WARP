using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV_AnimationController : MonoBehaviour {

	public float startDelay = 0f;
	public float noiseDuration = 0f;
	public bool isMonitor2 = false;

	private TV_AnimationController parentController;
	private GameObject playerObject;
	private Animator tvAnimator;

	// Use this for initialization
	void Start () {
		// check for type by existing animator component
		tvAnimator = GetComponent<Animator>();
		if(tvAnimator != null) {
			// game object is TV
		} else if(GetComponent<Collider>() != null) {
			// game object is trigger
			playerObject = GameObject.Find("Player");
			parentController = transform.parent.GetComponent<TV_AnimationController>();
		} else {
			// game object is none of the above >> remove this script
			Debug.Log("TV_AnimationController hängt weder an einem TV noch an einem TriggerCollider");
			Destroy(this);
		}
	}

	void OnTriggerEnter(Collider other) {
		OnTriggerStay(other);
	}

	void OnTriggerStay(Collider other) {
		if(isMonitor2) {
			// check for correct rotation
			float rotation = playerObject.transform.eulerAngles.y;
			if(rotation < 250 && rotation > -70) {
				return;
			}
		}
		parentController.startTvAnimation();
		Destroy(gameObject);
	}

	private void startTvAnimation() {
		StartCoroutine(tvAnimationCoroutine());
	}

	private IEnumerator tvAnimationCoroutine() {
		yield return new WaitForSeconds(startDelay);
		tvAnimator.SetInteger("state", 1);
		yield return new WaitForSeconds(noiseDuration);
		tvAnimator.SetInteger("state", 2);
	}
}
