using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Corpse : PixelObj
{

    [HideInInspector]
    public bool isIdentified = false;

    public Transform particlePosTr;

    public ParticleSystem bloodParticle;

    public List<Transform> raycastTargetPoses = new List<Transform>();

    string animName_Death = "Death";

    bool shouldBeRemoved = false;
    float removeDelay = 5;

    SoundType dieSound;

    public void Init(SoundType _dieSound)
    {
        dieSound = _dieSound;
    }

    // Use this for initialization
    void Start()
    {
        MapManager.Instance.AddCorpse(this);

        PlayAnimForCurSide(animName_Death);

        ParticleSystem parti = Instantiate(bloodParticle, particlePosTr.position, particlePosTr.rotation) as ParticleSystem;
        parti.Stop();
        parti.Play();

        AudioManager.Instance.Play(dieSound, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldBeRemoved)
        {
            removeDelay = MathFPlus.DecDeltaTimeToZero(removeDelay);

            if(removeDelay == 0)
            {
                MapManager.Instance.RemoveCorpse(this);
            }
        }
    }

    public void SetIdentified()
    {
        if (isIdentified)
            return;

        isIdentified = true;

        MapManager.Instance.AddCorpseToNotReportedIdentifiedList(this);
    }

    public void SetShouldBeRemoved()
    {
        shouldBeRemoved = true;
    }
}
