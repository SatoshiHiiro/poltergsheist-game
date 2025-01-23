using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    protected float fieldOfViewAngle;
    protected float sightRadius;
    protected bool playerVisible;
    protected Vector3 playerPosition;
    [SerializeField] protected bool facingRight;
    [SerializeField] protected LayerMask playerLayer;

    private void Start()
    {
        fieldOfViewAngle = 180f;
        sightRadius = 20f;
        playerVisible = false;
    }
    private void Update()
    {
        PlayerInFieldOfView();
    }

    protected virtual void PlayerInFieldOfView()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sightRadius, playerLayer);

        foreach (Collider2D hit in hits)
        {
            // Calculate the direction between the NPC and the object detected
            Vector2 directionTarget = (hit.transform.position - transform.position).normalized;

            // Check if the detected object is in the field of view
            float angle = Vector2.Angle(facingRight ? Vector2.right : Vector2.left, directionTarget);
            if (angle <= fieldOfViewAngle / 2)
            {
                Debug.Log($"Target in view: {hit.name}");
                playerVisible = true;
                playerPosition = hit.transform.position;
                // Ajoute ici la logique pour poursuivre ou attaquer la cible
            }
        }
    }

    // Debug method only
    private void OnDrawGizmos()
    {
        if (transform.position != null)
        {
            if(playerPosition != null)
            {
                Gizmos.DrawLine(transform.position, playerPosition);
            }
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.position, sightRadius);
        }

    }


}
