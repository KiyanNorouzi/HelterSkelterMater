using UnityEngine;
using System.Collections;


public enum GameState
{
    START_MENU,
    EXIT_FROM_MENU,
    START_GAME,
    EXIT_FROM_GAME,
}

public class Menu : MonoBehaviour
{

    public GameState gameState = GameState.START_MENU;

    void Update()
    {
        #region Game state

        #region START_MENU
        if (IsState(GameState.START_MENU))
        {

            SetState(GameState.START_GAME);


            SetState(GameState.EXIT_FROM_MENU);

        }
        #endregion

        #region START_GAME
        if (IsState(GameState.START_GAME))
        {

            //if exit
            SetState(GameState.EXIT_FROM_MENU);
        }
        #endregion

        #region EXIT_FROM_MENU
        if (IsState(GameState.EXIT_FROM_MENU))
        {
           
            SetState(GameState.START_MENU);

        }
        #endregion

        #endregion

    }


    #region Game States 
    void SetState(GameState _gameState)
    {
        gameState = _gameState;
    }

    bool IsState(GameState _gameState)
    {
        return gameState == _gameState;
    }

    #endregion


}
