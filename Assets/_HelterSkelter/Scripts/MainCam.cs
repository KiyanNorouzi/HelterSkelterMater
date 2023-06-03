using UnityEngine;
using System.Collections;

public class MainCam : MonoBehaviour {

    float makhraj = 160;

	// Use this for initialization
	void Start () {

        float scAspect = (float)Screen.width / (float)Screen.height;

        float orthoCoefForAspect = (16f / 9f) / (scAspect);

        float orthoForAspect = GetComponent<Camera>().orthographicSize * orthoCoefForAspect;

        GetComponent<Camera>().orthographicSize = Mathf.Round(orthoForAspect);

        //

        //float result = ((float)(Screen.height)) / makhraj;
        //float resultAsInt = (float)((int)result);

        //if (resultAsInt != result)
        //{
        //    //resultAsInt++;
        //}

        //float orthoForPixPerfect = ((float)(Screen.height) / (resultAsInt * 2));

        //while (orthoForPixPerfect < orthoForAspect)
        //{
        //    resultAsInt--;
        //    orthoForPixPerfect = ((float)(Screen.height) / (resultAsInt * 2));
        //}

        //camera.orthographicSize = orthoForPixPerfect;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
