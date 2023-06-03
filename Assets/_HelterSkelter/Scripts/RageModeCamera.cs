using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class RageModeCamera : MonoBehaviour
{
    public static RageModeCamera Instance;

    Animator animatorr;
    NoiseAndGrain noiseAndGrain;
    BloomOptimized bloom;
    Fisheye fishEye;
    Twirl twirl;
    NoiseAndScratches noiseAndScratches;
    ColorCorrectionCurves colorCor;

    int step = -1;

    bool shouldStopRageMode = false;
    bool shouldStartRageModeInQueue = false;

    // Use this for initialization
    void Awake()
    {
        Instance = this;

        animatorr = GetComponent<Animator>();
        noiseAndGrain = GetComponent<NoiseAndGrain>();
        bloom = GetComponent<BloomOptimized>();
        fishEye = GetComponent<Fisheye>();
        twirl = GetComponent<Twirl>();
        noiseAndScratches = GetComponent<NoiseAndScratches>();
        colorCor = GetComponent<ColorCorrectionCurves>();
    }

    // Update is called once per frame
    void Update()
    {
        if(step == -1)
        {
            if(shouldStartRageModeInQueue)
            {
                shouldStartRageModeInQueue = false;
                step = 0;
            }
        }

        if (step == 0)
        {
            noiseAndGrain.enabled = true;
            bloom.enabled = true;
            fishEye.enabled = true;
            twirl.enabled = true;
            noiseAndScratches.enabled = true;
            colorCor.enabled = true;

            animatorr.SetInteger("State", 1);

            step = 1;
        }
        
        if(step == 1)
        {
            if (shouldStopRageMode)
                step = 2;
        }

        if(step == 2)
        {
            animatorr.SetInteger("State", 2);
            step = 3;
        }

        if(step == 3)
        {
            if(noiseAndGrain.generalIntensity == 0)
            {
                step = 1000;
            }
        }

        if(step == 1000)
        {
            noiseAndGrain.enabled = false;
            bloom.enabled = false;
            fishEye.enabled = false;
            twirl.enabled = false;
            noiseAndScratches.enabled = false;
            colorCor.enabled = false;

            animatorr.SetInteger("State", 0);

            shouldStopRageMode = false;
            step = -1;
        }

    }

    public void StartRageMode()
    {
        if (step == -1)
            step = 0;
        else
            shouldStartRageModeInQueue = true;
    }

    public void ShouldStopRageMode()
    {
        shouldStopRageMode = true;
    }

    public void SetRageModeIntensity(float _intensity)
    {

    }
}
