using UnityEngine;
using System.Collections;

public class PixelPerfectScale : MonoBehaviour
{
    int screenVertPixels = 256;

    bool preferUncroppedMate = true;

    private float screenPixelsY;

    private float ratio;

    void Update()
    {
        //if (preferUncroppedMate)
        //{
        //    screenPixelsY = (float)Screen.height;
        //    float screenRatio = screenPixelsY / screenVertPixels;
        //    ratio = Mathf.Floor(screenRatio) / screenRatio;
        //    transform.localScale = Vector3.one * ratio;
        //}
        //else
        //{
        //    screenPixelsY = (float)Screen.height;
        //    float screenRatio = screenPixelsY / screenVertPixels;
        //    ratio = Mathf.Ceil(screenRatio) / screenRatio;
        //    transform.localScale = Vector3.one * ratio;
        //}
    }
}
