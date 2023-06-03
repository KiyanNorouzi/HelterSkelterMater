using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimControllerStatus
{
    idle,
    playing,
    paused,
    finished,
}

public class Anim2DController : MonoBehaviour
{
    public List<Anim2D> anims = new List<Anim2D>();

    public Anim2D autoStartAnim = null;

    [HideInInspector]
    public AnimControllerStatus status = AnimControllerStatus.idle;

    public SpriteRenderer viewFrame;

    Anim2D curAnim = null;
    float timeCounter = 0;

    int curFrame = 0;
    int startingFrame = 0;

    float tc = 1;

    float speedCoef = 1;

    int newFrame = -1;

    // Use this for initialization
    void Start()
    {
        if (autoStartAnim != null)
        {
            PlayAnim(autoStartAnim);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (status == AnimControllerStatus.playing)
        {
            timeCounter += speedCoef * Time.deltaTime;

            newFrame = GetFrameByTime(timeCounter);

            if (newFrame == -1)
            {
                Stop();
            }
            else
            {
                if (curFrame != newFrame)
                {
                    curFrame = newFrame;
                    viewFrame.sprite = curAnim.sprites[curFrame];
                }
            }
        }
    }


    public void PlayAnim(string _animName, float _speedCoef)
    {
        string anName = _animName;

        for (int i = 0; i < anims.Count; i++)
        {
            if (anims[i].animName == anName)
            {
                PlayAnim(anims[i], _speedCoef);
                return;
            }
        }

        Debug.LogError("Can NOT find anim: " + anName + ", for " + gameObject.ToString());
    }

    public void PlayAnim(string _animName)
    {
        PlayAnim(_animName, 1);
    }

    public void PlayAnim(Anim2D _anim2D, float _speedCoef)
    {
        if (curAnim == _anim2D && status == AnimControllerStatus.playing)
            return;

        if (status == AnimControllerStatus.playing)
            Stop();

        curAnim = _anim2D;

        curFrame = -1;

        timeCounter = 0;

        status = AnimControllerStatus.playing;

        speedCoef = _speedCoef;
    }

    public void PlayAnim(Anim2D _anim2D)
    {
        PlayAnim(_anim2D, 1);
    }

    public void Pause()
    {
        status = AnimControllerStatus.paused;
    }

    public void Resume()
    {
        if (curAnim == null)
            return;

        status = AnimControllerStatus.playing;
    }

    public void Stop()
    {
        timeCounter = 0;
        curFrame = -1;

        status = AnimControllerStatus.finished;
    }

    int GetFrameByTime(float _time)
    {
        if (curAnim.sprites.Count == 0)
        {
            Debug.LogError("Anim: " + curAnim.ToString() + "sprites are not set.");
            return -1;
        }

        float time = _time;

        #region OneTime
        if (curAnim.animType == AnimType.OneTime)
        {
            int newFr = (int)(time / curAnim.frameTime);

            if (newFr >= curAnim.TotalNumOfFrames)
                newFr = -1;

            return newFr;
        }
        #endregion

        #region Loop
        if (curAnim.animType == AnimType.Loop)
        {
            float animMaxTime = curAnim.TotalTime;
            float lTime = time % animMaxTime;

            int newFr = (int)(lTime / curAnim.frameTime);

            return newFr;
        }
        #endregion

        #region Backward
        if (curAnim.animType == AnimType.Backward)
        {
            int newFr = (int)((curAnim.TotalTime - time) / curAnim.frameTime);

            if (newFr < 0)
                newFr = -1;

            return newFr;
        }
        #endregion

        #region BackwardLoop
        if (curAnim.animType == AnimType.Backward_Loop)
        {
            float animMaxTime = curAnim.TotalTime;
            float lTime = (animMaxTime - time) % animMaxTime;

            if (lTime < 0)
            {
                lTime += animMaxTime;
            }

            int newFr = (int)(lTime / curAnim.frameTime);

            return newFr;
        }
        #endregion

        #region PingPong
        if (curAnim.animType == AnimType.PingPong)
        {
            int newFr = (int)(time / curAnim.frameTime);

            if (newFr < curAnim.TotalNumOfFrames)
                return newFr;

            if (newFr >= curAnim.TotalNumOfFrames && newFr < 2 * curAnim.TotalNumOfFrames)
            {
                return 2 * curAnim.TotalNumOfFrames - 1 - newFr;

            }

            if (newFr >= 2 * curAnim.TotalNumOfFrames)
                return -1;
        }
        #endregion

        #region PingPongLoop
        if (curAnim.animType == AnimType.PingPong_Loop)
        {
            float animMaxTime = 2 * curAnim.TotalTime;
            float lTime = time % animMaxTime;

            int newFr = (int)(lTime / curAnim.frameTime);

            if (newFr < curAnim.TotalNumOfFrames)
                return newFr;

            if (newFr >= curAnim.TotalNumOfFrames && newFr < 2 * curAnim.TotalNumOfFrames)
            {
                return 2 * curAnim.TotalNumOfFrames - 1 - newFr;
            }
        }
        #endregion

        return -1;
    }
}
