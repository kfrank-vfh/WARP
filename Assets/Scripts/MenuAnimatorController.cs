using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuAnimatorController : MonoBehaviour {

	private Animator menuAnimator;

	private Dictionary<string, int> menuToId;

	// Use this for initialization
	void Start () {
		menuAnimator = gameObject.GetComponent<Animator>();
		menuToId = new Dictionary<string, int>();
		menuToId.Add("title", 0);
		menuToId.Add("options", 1);
		menuToId.Add("credits", 2);
		menuToId.Add("highscore", 3);
		menuToId.Add("entername", 4);
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

	public void QuitGame() {
		Application.Quit();
	}
}
