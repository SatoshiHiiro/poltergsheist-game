using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoundDetection : MonoBehaviour
{
    // Manage the detection of the sounds

    [SerializeField] protected float soundRadius;       // Radius in which the sound is projected
    [SerializeField] protected float floorThreshold;    // Distance between two floor
    [SerializeField] protected LayerMask layerToDetect;
    [SerializeField] protected float floorLevel;
    protected bool firstNPCNotified = false;    // Is there an NPC that was already notified of the sound

    // Notify every enemies in the zone of the sound
    protected void NotifyNearbyEnemies(float floorY, GameObject objectSound)
    {
        // Find all NPC within range
        Collider2D[] colliderNearbyNPC = Physics2D.OverlapCircleAll(transform.position, soundRadius, layerToDetect);

        List<HumanNPCBehaviour> availableNPCs = new List<HumanNPCBehaviour>();
        List<HumanNPCBehaviour> blockedNPCs = new List<HumanNPCBehaviour>();

        foreach (Collider2D colliderNPC in colliderNearbyNPC)
        {
            HumanNPCBehaviour npc = colliderNPC.GetComponent<HumanNPCBehaviour>();
            if (npc != null)
            {
                PatrollingNPCBehaviour npcPatrol = npc.GetComponent<PatrollingNPCBehaviour>();
                // Check if the NPC is blocked
                if(npcPatrol != null && (npcPatrol.IsBlocked || npcPatrol.IsInRoom))
                {
                    blockedNPCs.Add(npcPatrol);
                }
                else
                {
                    // The NPC is availabe to go an investigate
                    availableNPCs.Add(npc);
                }
            }
        }

        if(availableNPCs.Count > 0)
        {
            NotifyNPCs(availableNPCs, floorLevel, objectSound);
        }
        else if(blockedNPCs.Count > 0)
        {
            // Every NPCs are blocked
            NotifyNPCs(blockedNPCs, floorLevel, objectSound);
        }
    }

    private void NotifyNPCs(List<HumanNPCBehaviour> npcsList, float floorLevel, GameObject objectSound)
    {
        foreach(HumanNPCBehaviour npc in npcsList)
        {
            bool isFirst = !firstNPCNotified;
            npc.InvestigateSound(objectSound,isFirst,floorLevel);
            firstNPCNotified = true;
        }
        firstNPCNotified = false;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}
