﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoor : MonoBehaviour {

	private GameObject finalDoor;
	private PlayerController playerController;
	private GameObject cameraObject;
	private FPCameraController cameraController;
	private Animator doorAnimator;
	private AudioController audioController;

	private bool openDoor = false;
	private bool fogFadeOut = false;
	private float startTime = 0f;

	// Use this for initialization
	void Start () {
		finalDoor = transform.parent.gameObject;
		playerController = GameObject.Find("/Player").GetComponent<PlayerController>();
		cameraObject = GameObject.Find("/Player/FirstPerson/Camera");
		cameraController = cameraObject.GetComponent<FPCameraController>();
		doorAnimator = finalDoor.GetComponent<Animator>();
		audioController = finalDoor.GetComponent<AudioController>();

		fogFadeOut = true;
		startTime = Time.time;
		playerController.pauseGame(true);
	}

	void Update() {
		if(openDoor) {
			RenderSettings.fog = true;
			RenderSettings.fogMode = FogMode.Exponential;
			float f = (Time.time - startTime) / 2;
			f = f > 1 ? 1 : f;
			RenderSettings.fogDensity = f * 0.7f;
			if(f >= 1) {
				openDoor = false;
			}
		}
		if(fogFadeOut) {
			RenderSettings.fog = true;
			RenderSettings.fogMode = FogMode.Exponential;
			float f = (Time.time - startTime) / 2;
			f = f > 1 ? 1 : f;
			RenderSettings.fogDensity = (1 - f) * 0.7f;
			if(f >= 1) {
				fogFadeOut = false;
				RenderSettings.fog = false;
				playerController.pauseGame(false);
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		StartCoroutine(finalCoroutine());
		GameStatsController.setPlayEndTime(Time.time);
		Destroy(GetComponent<CapsuleCollider>());
	}

	private IEnumerator finalCoroutine() {
		playerController.pauseGame(true);
		cameraController.lerpTo(finalDoor.transform.position, 1f);
		yield return new WaitForSeconds(1f);
		doorAnimator.SetTrigger("open");
		startTime = Time.time;
		openDoor = true;
		audioController.playDoorOpenSound();
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene("StoryScene");
	}
}
