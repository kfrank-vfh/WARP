using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TV_AnimationController : MonoBehaviour {

	public float startDelay = 0f;
	public float noiseDuration = 0f;
	public bool isMonitor2 = false;

	private TV_AnimationController parentController;
	private AudioController audioController;
	private GameObject playerObject;
	private Animator tvAnimator;

	private Text timeText;
	private float lastUpdate = -1;
	private string timeDisplay;

	// Use this for initialization
	void Start () {
		// check for type by existing animator component
		tvAnimator = GetComponent<Animator>();
		if(tvAnimator != null) {
			// game object is TV
			audioController = GetComponent<AudioController>();
			timeText = transform.Find("Canvas").Find("Text").GetComponent<Text>();
			timeText.enabled = false;
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

	void Update() {
		if(timeText == null) {
			return;
		}
		if(lastUpdate < 0 || lastUpdate != Time.time) {
			lastUpdate = Time.time;
			timeDisplay = getDisplayStringToTime(Time.time - GameStatsController.getPlayStartTime());
		}
		timeText.text = timeDisplay;
	}

	private string getDisplayStringToTime(float time) {
		int minutes = (int)(time / 60f);
		int seconds = (int)(time % 60);
		int millis = (int)((time * 1000) % 1000);
		string displayString = minutes == 0 ? "" : minutes + ":";
		displayString += seconds + ":" + millis;
		return displayString;
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

	public void startTvAnimation() {
		StartCoroutine(tvAnimationCoroutine());
	}

	private IEnumerator tvAnimationCoroutine() {
		yield return new WaitForSeconds(startDelay);
		tvAnimator.SetInteger("state", 1);
		audioController.playMonitorNoise(noiseDuration);
		yield return new WaitForSeconds(noiseDuration);
		tvAnimator.SetInteger("state", 2);
		timeText.enabled = true;
	}
}
