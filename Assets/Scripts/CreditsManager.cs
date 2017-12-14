using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour {

	void Start () {
		Text creditsText = transform.Find("Scroll View").Find("Viewport").Find("Content").Find("Text").GetComponent<Text>();
		string text = PersistenceManager.readTextResource("credits");
		creditsText.text = text;
	}
}
