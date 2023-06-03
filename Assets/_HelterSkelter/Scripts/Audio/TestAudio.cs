using UnityEngine;
using System.Collections;

public class TestAudio : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;
    void Start()
    {
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
