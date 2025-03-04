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

        foreach (Collider2D colliderNPC in colliderNearbyNPC)
        {
            //EnemyBehaviour enemy = colliderNPC.GetComponent<EnemyBehaviour>();
            HumanNPCBehaviour npc = colliderNPC.GetComponent<HumanNPCBehaviour>();
            if (npc != null)
            {
                // NPCs on the same floor get priority
                if (npc.FloorLevel == floorLevel)
                {

                    if (!firstNPCNotified)
                    {
                        firstNPCNotified = true;
                        npc.InvestigateSound(objectSound, true, floorLevel);
                    }
                    else
                    {
                        npc.InvestigateSound(objectSound, false, floorLevel);
                    }
                }
                else
                {

                    if (!firstNPCNotified)
                    {
                        npc.InvestigateSound(objectSound, true, floorLevel);
                    }
                    else
                    {
                        npc.InvestigateSound(objectSound, false, floorLevel);
                    }

                }

            }
        }

        firstNPCNotified = false;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}
