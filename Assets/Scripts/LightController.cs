using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

	public float delay = 0f;

	private Light lightComponent;
	private Collider triggerCollider;
	private AudioController audioController;

	// Use this for initialization
	void Start () {
		lightComponent = GetComponent<Light>();
		if(lightComponent == null) {
			Debug.Log("This gameobject '" + gameObject + "' does not have a light component!");
			Destroy(gameObject);
		}
		audioController = GetComponent<AudioController>();

		triggerCollider = GetComponent<Collider>();
		if(triggerCollider == null) {
			// collider does not exist >> show light at start
			lightComponent.enabled = true;
		} else {
			// collider exists >> show light in trigger enter
			triggerCollider.isTrigger = true;
			lightComponent.enabled = false;
		}
	}
	
	void OnTriggerEnter(Collider other) {
		showLight(delay);
		triggerCollider.enabled = false;
	}

	public void showLight(float delay, bool playSound = true) {
		StartCoroutine(showLightCoroutine(delay, playSound));
	}

	private IEnumerator showLightCoroutine(float delay, bool playSound) {
		yield return new WaitForSeconds(delay);
		lightComponent.enabled = true;
		if(playSound) {
			if(audioController == null) {
				Debug.Log("No audio controller attached to light '" + gameObject + "'!");
			} else {
				audioController.playLightsOnSound();
			}
		}
	}
}
