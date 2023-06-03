using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScoreList : MonoBehaviour {

	public GameObject playerScoreEntryPrefab;

	ScoreManager scoreManager;

	int lastChangeCounter;

	void Start () {
		scoreManager = GameObject.FindObjectOfType<ScoreManager>();

		lastChangeCounter = scoreManager.GetChangeCounter();
	}

	void Update () {
		if(scoreManager == null) {
			Debug.LogError("You forgot to add the score manager component to a game object!");
			return;
		}

		if(scoreManager.GetChangeCounter() == lastChangeCounter) {

			return;
		}

		lastChangeCounter = scoreManager.GetChangeCounter();

		while(this.transform.childCount > 0) {
			Transform c = this.transform.GetChild(0);
			c.SetParent(null);  
			Destroy (c.gameObject);
		}

		string[] names = scoreManager.GetPlayerNames("kills");
		
		foreach(string name in names) {
            GameObject go = Instantiate(playerScoreEntryPrefab) as GameObject;
			go.transform.SetParent(this.transform);
            go.transform.localScale = Vector3.one;
			go.transform.Find ("Username").GetComponent<Text>().text = name;
			go.transform.Find ("Kills").GetComponent<Text>().text = scoreManager.GetScore(name, "kills").ToString();
			go.transform.Find ("Scores").GetComponent<Text>().text = scoreManager.GetScore(name, "score").ToString();
		}
	}
}
