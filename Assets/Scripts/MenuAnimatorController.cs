using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuAnimatorController : MonoBehaviour {

	private Animator menuAnimator;
	private Canvas canvas;
	private PlayerController playerController;

	private Dictionary<string, int> menuToId;

	// Use this for initialization
	void Start () {
		// get menu animator
		menuAnimator = gameObject.GetComponent<Animator>();
		canvas = GetComponent<Canvas>();
		// map sub menu names to ids
		menuToId = new Dictionary<string, int>();
		menuToId.Add("title", 0);
		menuToId.Add("options", 1);
		menuToId.Add("credits", 2);
		menuToId.Add("highscore", 3);
		menuToId.Add("entername", 4);
		// set menu state depending on Scene
		if(SceneManager.GetActiveScene().name.Equals("MainMenu")) {
			menuAnimator.SetBool("MenuVisible", true);
			canvas.enabled = true;
		} else if(SceneManager.GetActiveScene().name.Equals("LevelScene")) {
			playerController = GameObject.Find("/Player").GetComponent<PlayerController>();
		}
	}

	void Update() {
		if(Input.GetKeyDown("escape")) {
			bool visible = menuAnimator.GetBool("MenuVisible");
			visible = !visible;
			menuAnimator.SetBool("MenuVisible", visible);
			if(visible) {
				playerController.pauseGame(true);
			} else {
				continueGame();
			}
		}
	}

	public void showMenu(string menu) {
		int menuId = menuToId[menu];
		menuAnimator.SetInteger("MenuId", menuId);
	}

	public void startGame() {
		Text text = GameObject.Find("MenuCanvas/EnterNamePanel/EnterNameField/Text").GetComponent<Text>();
		GameStatsController.setPlayerName(text.text);
		SceneManager.LoadScene("LevelScene");
	}

	public void continueGame() {
		StartCoroutine(continueGameCoroutine());
	}

	private IEnumerator continueGameCoroutine() {
		menuAnimator.SetBool("MenuVisible", false);
		yield return new WaitForSeconds(0.5f);
		playerController.pauseGame(false);
	}

	public void goToTitleMenu() {
		StartCoroutine(goToTitleMenuCoroutine());
	}

	private IEnumerator goToTitleMenuCoroutine() {
		menuAnimator.SetBool("MenuVisible", false);
		yield return new WaitForSeconds(0.5f);
		playerController.pauseGame(false);
		Destroy(GameObject.Find("GameStats"));
		SceneManager.LoadScene("MainMenu");
	}

	public void QuitGame() {
		Application.Quit();
	}
}
