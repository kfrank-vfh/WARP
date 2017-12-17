using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	}

	public void showMenu(string menu) {
		int menuId = menuToId[menu];
		menuAnimator.SetInteger("MenuId", menuId);
	}

	public void QuitGame() {
		Application.Quit();
	}
}
