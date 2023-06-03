using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour {

	Dictionary< string, Dictionary<string, int> > playerScores;

	int changeCounter = 0;

	void Init() {
		if(playerScores != null)
			return;

		playerScores = new Dictionary<string, Dictionary<string, int>>();
	}

	public void Reset() {
		changeCounter++;
		playerScores = null;
	}

	public int GetScore(string username, string scoreType) {
		Init ();

		if(playerScores.ContainsKey(username) == false) {

			return 0;
		}


		if(playerScores[username].ContainsKey(scoreType) == false) {
			return 0;
		}

		return playerScores[username][scoreType];
	}

	public void SetScore(string username, string scoreType, int value) {
		Init ();

		changeCounter++;

		if(playerScores.ContainsKey(username) == false) {
			playerScores[username] = new Dictionary<string, int>();
		}

		playerScores[username][scoreType] = value;
	}

	public void ChangeScore(string username, string scoreType, int amount) {
		Init ();
		int currScore = GetScore(username, scoreType);
		SetScore(username, scoreType, currScore + amount);
	}

	public string[] GetPlayerNames() {
		Init ();
		return playerScores.Keys.ToArray();
	}
	
	public string[] GetPlayerNames(string sortingScoreType) {
		Init ();

		return playerScores.Keys.OrderByDescending( n => GetScore(n, sortingScoreType) ).ToArray();
	}

	public int GetChangeCounter() {
		return changeCounter;
	}


}
