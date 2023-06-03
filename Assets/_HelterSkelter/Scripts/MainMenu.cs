using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public GUIControl guiCtrl_Start;
    public GUIControl guiCtrl_Exit;

	// Use this for initialization
	void Start () {
        guiCtrl_Start.Event_Tap += StartGame;
        guiCtrl_Exit.Event_Tap += ExitGame;
	}

    void StartGame(GUIControl _control, TouchUpInfo _tapInfo)
    {
        Application.LoadLevel(1);
    }

    void ExitGame(GUIControl _control, TouchUpInfo _tapInfo)
    {
        Application.Quit();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
