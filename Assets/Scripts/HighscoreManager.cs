using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
}

[Serializable]
class HighscoreEntry : IComparable<HighscoreEntry> {

	DateTime timestamp;
	string name;
	float playthroughTime;

	public int CompareTo(HighscoreEntry entry) {
		return (int)(playthroughTime - entry.playthroughTime);
	}
}