using UnityEngine;
using System.Collections;

public class LoadingManager : MonoBehaviour {

	public GameObject loadingPage;

	void Start () 
    {
		loadingPage.SetActive (false);
	}
	

    public void GoTo(string levelName)
    {
        StartCoroutine(LoadLevel(levelName));
    }

    public void Exit()
    {
        Application.Quit();
    }

	IEnumerator LoadLevel(string str) {
		loadingPage.SetActive (true);
		AsyncOperation async = Application.LoadLevelAsync (str);

		if(!async.isDone)
			yield return null;
	}
}
