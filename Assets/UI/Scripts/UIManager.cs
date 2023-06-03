using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum UIManagerState
{
    OPENING,
    GAMEPLAY,
    GAMEOVER
}

public class UIManager : MonoBehaviour
{
    public GameObject playButton;
    public GameObject backGround;
    public GameObject GO;
    [HideInInspector]
    public UIManagerState uiState;

    void Start()
    {
        uiState = UIManagerState.OPENING;
    }

    void UpdateUIManagerState()
    {
        switch (uiState)
        {
            case UIManagerState.OPENING:
                //hide game over and set play button visible 
                playButton.SetActive(true);
                break;

            case UIManagerState.GAMEPLAY:
                //hide play button on game play state
                playButton.SetActive(false);
                GO.GetComponent<PlayerInput>().Init();
                break;


            case UIManagerState.GAMEOVER:
                //display game over and change ui manager to opening state 
                break;
        }
    }

    public void SetUIManagerState(UIManagerState _stete)
    {
        uiState = _stete;
        UpdateUIManagerState();
    }


    //por play button will call this function when the user clicks the button.
    public void StartGamePlay()
    {
        uiState = UIManagerState.GAMEPLAY;
        UpdateUIManagerState();
    }

    public void ChangeToOpeningState()
    {
        SetUIManagerState(UIManagerState.OPENING);
    }

}
