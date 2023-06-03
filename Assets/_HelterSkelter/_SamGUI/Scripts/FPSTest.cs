using UnityEngine;
using System.Collections;

public class FPSTest : MonoBehaviour
{

    Rect rct = new Rect(50, 50, 100, 100);

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    float tc = 1;
    float maxTc = 1f;
    string txt = "";

    void OnGUI()
    {
        tc += Time.deltaTime;

        if (tc >= maxTc)
        {
            tc = 0;
            txt = (1 / Time.deltaTime).ToString();
        }
        GUI.depth = 1000;

        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 30;

        GUI.color = new Color(0, 0, 0, 1);

        GUI.Label(rct, txt, guiStyle);
    }
}
