using UnityEngine;

public abstract class SoundDetection : MonoBehaviour
{
    // Manage the detection of the sounds

    [SerializeField] protected float soundRadius;       // Radius in which the sound is projected
    [SerializeField] protected float floorThreshold;    // Distance between two floor
    [SerializeField] protected LayerMask layerToDetect;
    protected bool firstNPCNotified = false;    // Is there an NPC that was already notified of the sound

    // Notify every enemies in the zone of the sound
    protected void NotifyNearbyEnemies(float floorY, GameObject objectSound)
    {
        // Find all NPC within range
        Collider2D[] colliderNearbyNPC = Physics2D.OverlapCircleAll(transform.position, soundRadius, layerToDetect);

        foreach(Collider2D colliderNPC in colliderNearbyNPC)
        {
            //EnemyBehaviour enemy = colliderNPC.GetComponent<EnemyBehaviour>();
            HumanNPCBehaviour npc = colliderNPC.GetComponent<HumanNPCBehaviour>();
            if (npc != null)
            {
                // Make sure the npc is on the same floor as the object
                float npcY = npc.transform.position.y;
                if(Mathf.Abs(npcY - floorY) < floorThreshold)
                {
                    // The first NPC is the one who will replace the object to it's original position
                    if (!firstNPCNotified)
                    {
                        firstNPCNotified = true;
                        npc.InvestigateSound(objectSound, true);
                    }
                    else
                    {
                        npc.InvestigateSound(objectSound, false);
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
