using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class PlayerInput : MonoBehaviour
{

    public Text KillStatUIText; //  refrence to the lives ui text
    public Text scoreUiText; // refrence to the score ui text
    int Killed = 10; // current player lives
    int score;


    void Update()
    {
        Init();
        ChangeLivesText();
        ChangeScore();

    }


    public void Init()
    {
        KillStatUIText.text = Killed.ToString(); // update the lives ui text

    }

    void ChangeLivesText()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Killed > 0)
            {
                Killed--; // subtract one live
            }


            KillStatUIText.text = Killed.ToString();// update kill stats ui text




        }
    }

    void ChangeScore()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            score += 100;
            scoreUiText.text = score.ToString(); // update score ui text

        }


    }





}
