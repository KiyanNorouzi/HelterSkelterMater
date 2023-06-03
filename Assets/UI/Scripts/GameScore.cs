using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{

    Text scoreTextUI;
    public int score;

    public int Score
    {
        get
        {
            return this.score;
        }

        set
        {
            this.score = value;
            UpdateScoreTextUI();

        }
    }

    void Start()
    {
        //Get the Text UI component of this gameObject 
        scoreTextUI = GetComponent<Text>();
    }


    public void UpdateScoreTextUI()
    {
        string scoreStr = string.Format("{0.0000}",score);
        scoreTextUI.text = scoreStr;
    }


}
