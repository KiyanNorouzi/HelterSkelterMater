using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeWeaponEffector : MonoBehaviour 
{
    public MeleeWeapon owner;

    [HideInInspector]
    public bool isEffective = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!(owner.IsActive() && isEffective))
        {
            return;
        }

        CharacterPublicTrigger charPubTrig = other.GetComponent<CharacterPublicTrigger>();

        if (charPubTrig == null)
            return;

        if (!MapManager.playerWeaponTargetTags.Contains(charPubTrig.owner.gameObject.tag))
            return;

        owner.TryAddNewVictim(charPubTrig.owner);
    }
}
