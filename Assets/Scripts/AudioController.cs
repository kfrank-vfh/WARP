﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {

	private static AudioMixer audioMixer;
	private static bool instantiated = false;

	private static AudioClip footstepClip;
	private static AudioClip jumpClip;
	private static AudioClip noiseClip;
	private static AudioClip blowClip;
	private static AudioClip woundedClip;
	private static AudioClip warpClip;
	private static AudioClip doorOpenClip;
	private static AudioClip lightsOnClip;
	private static AudioClip buttonHoverClip;
	private static AudioClip buttonAcceptClip;
	private static AudioClip buttonBackClip;
	private static AudioClip letterClip;

	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		if(initPerGameObject() & !instantiated) {
			initOnce();
		}
	}

	private void initOnce() {
		// init audio mixer if not already done
		audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
		// init clips
		footstepClip = PersistenceManager.loadAudioClip("footstep_metallic");
		jumpClip = PersistenceManager.loadAudioClip("jump_metallic");
		noiseClip = PersistenceManager.loadAudioClip("noise");
		blowClip = PersistenceManager.loadAudioClip("blow");
		woundedClip = PersistenceManager.loadAudioClip("wound");
		warpClip = PersistenceManager.loadAudioClip("gaze");
		doorOpenClip = PersistenceManager.loadAudioClip("dooropen");
		lightsOnClip = PersistenceManager.loadAudioClip("lights on");
		buttonHoverClip = PersistenceManager.loadAudioClip("buttonhover");
		buttonAcceptClip = PersistenceManager.loadAudioClip("buttonaccept");
		buttonBackClip = PersistenceManager.loadAudioClip("buttoncancel");
		letterClip = PersistenceManager.loadAudioClip("letter");
	}

	private bool initPerGameObject() {
		// init audio source
		audioSource = GetComponent<AudioSource>();
		if(audioSource ==  null) {
			Debug.Log("No AudioSource attached to GameObject: " + gameObject);
			Destroy(this);
			return false;
		}
		// check if audio mixer group is attached
		AudioMixerGroup audioMixerGroup = audioSource.outputAudioMixerGroup;
		if(audioMixerGroup == null) {
			Debug.Log("No AudioMixerGroup attached to AudioSource of GameObject: " + gameObject);
			Destroy(this);
			return false;
		}
		return true;
	}

	public void playFootstepSound() {
		audioSource.pitch = Random.Range(0.95f, 1.05f);
		audioSource.PlayOneShot(footstepClip, Random.Range(0.5f, 0.7f));
	}

	public void playJumpSound() {
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(jumpClip);
	}

	public void playMonitorNoise(float duration) {
		StartCoroutine(monitorNoiseCoroutine(duration));
	}

	private IEnumerator monitorNoiseCoroutine(float duration) {
		audioSource.PlayOneShot(noiseClip, 0.8f);
		yield return new WaitForSeconds(duration);
		audioSource.Stop();
	}

	public void playDropWoundedSound() {
		audioSource.PlayOneShot(blowClip);
		audioSource.PlayOneShot(woundedClip);
	}

	public void playWarpSound() {
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(warpClip);
	}

	public void playDoorOpenSound() {
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(doorOpenClip);
	}

	public void playLightsOnSound() {
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(lightsOnClip);
	}

	public void playButtonHoverClip() {
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(buttonHoverClip);
	}

	public void playButtonAcceptClip() {
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(buttonAcceptClip);
	}

	public void playButtonBackClip() {
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(buttonBackClip);
	}

	public void playLetterClip(float pitch = 1f) {
		audioSource.pitch = pitch;
		audioSource.PlayOneShot(letterClip, 0.5f);
	}

	public static void setMasterVolume(float volume) {
		if(audioMixer != null) {
			audioMixer.SetFloat("volume", volume);
		}
	}
}
