using UnityEngine;
using System.Collections;

public class TestMEssenger : MonoBehaviour
{
    void Start()
    {
        int i = 0;
        Messenger.Broadcast("Application_Start", i, "Hello");

    }
    void Update()
    {

    }
}
