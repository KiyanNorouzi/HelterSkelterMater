using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Text timeUI;

    float startTime;
    float ellapsedTime;
    bool startCounter;
    bool stopCounter;

    int minutes;
    int seconds;

    void Start()
    {
        startCounter = false;
        timeUI = GetComponent<Text>();
    }

    void Update()
    {
        if (startCounter)
        {
            ellapsedTime = Time.time - startTime;
            timeUI.text = GetTimeString(ellapsedTime);
        }

        StartTimeCounter();

    }


    public void StartTimeCounter()
    {
        startTime = Time.deltaTime;
        startCounter = true;
    }

    public void StopTimeCounter()
    {
        startCounter = false;
    }



    public static string GetTimeString(float _timeVal)
    {
        float timeVal = _timeVal;

        int mins = (int)(timeVal / 60);
        int seconds = (int)(timeVal - mins * 60);
        int miliseconds = (int)((timeVal - mins * 60 - seconds) * 100);

        string minsString = "";
        string secsString = "";

        if (mins == 0)
        {
            minsString = "00";
        }

        if (mins > 0 && mins < 10)
        {
            minsString = "0" + mins.ToString();
        }

        if (mins >= 10)
        {
            minsString = mins.ToString();
        }



        if (seconds == 0)
        {
            secsString = "00";
        }

        if (seconds > 0 && seconds < 10)
        {
            secsString = "0" + seconds.ToString();
        }

        if (seconds >= 10)
        {
            secsString = seconds.ToString();
        }

        return minsString + ":" + secsString ;
    }


}
