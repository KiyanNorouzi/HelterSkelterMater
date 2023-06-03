using UnityEngine;
using System.Collections;

public class TestListen : MonoBehaviour
{

    void Start()
    {
        Messenger.AddListener<int, string>("Application_Start", Hello);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Hello(int i, string str)
    {
        //print(str + " " + i);
    }
}
