using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatsController : MonoBehaviour {

	private static string playerName;
	private static float playStartTime = -1f;
	private static float playEndTime = -1f;

	private Animator storyAnimator;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}

	void OnLevelWasLoaded() {
		string sceneName = SceneManager.GetActiveScene().name;
		Debug.Log("GameStatsController on Scene " + sceneName);
		if (sceneName.Equals("StoryScene")) {
			storyAnimator = GameObject.Find("Canvas").GetComponent<Animator>();
			if(playStartTime < 0) {
				// show Intro
				StartCoroutine(introCoroutine());
			} else {
				// show Outro
				// TODO
			}
		} else if (sceneName.Equals("LevelScene")) {
			playStartTime = Time.time;
			if(playerName == null || playerName.Length == 0) {
				playerName = "Unbekannt";
			}
		}
	}

	private IEnumerator introCoroutine() {
		storyAnimator.SetTrigger("showIntro");
		yield return new WaitForSeconds(7f);
		SceneManager.LoadScene("LevelScene");
	}
	
	public static void setPlayerName(string name) {
		playerName = name;
	}

	public static void setPlayEndTime(float time) {
		playEndTime = time;
	}
}
