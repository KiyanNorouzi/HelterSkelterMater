using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapNPCCollisionArea : MonoBehaviour
{
    public Trap owner;
    [HideInInspector]
    public List<NPC> curInsideNPCs = new List<NPC>();

    void OnTriggerEnter2D(Collider2D col)
    {
        NPC npc = col.GetComponent<NPCSenseTrapTrigger>().owner;

        if (!curInsideNPCs.Contains(npc))
            curInsideNPCs.Add(npc);

        if (owner.shouldFreezeNPC)
        {
            //Civil civ = npc as Civil;

            //if (civ)
            npc.SetFreezedForTrap(owner, owner.trapExplosionDelay);
        }

        if (owner.IsState(TrapState.WAIT))
            owner.SetState(TrapState.EXPLOSION_DELAY_START);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        NPC npc = col.GetComponent<NPCSenseTrapTrigger>().owner;

        if (curInsideNPCs.Contains(npc))
            curInsideNPCs.Remove(npc);
    }
}
