using UnityEngine;
using System.Collections;

public class BloodParticle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem dropsParticle;
    [SerializeField]
    private ParticleSystem splatParticle;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Play();
    }

    public void Play()
    {
        if (dropsParticle.isPlaying) return;
        dropsParticle.Play();
        splatParticle.Play();
    }
}
