using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    protected float fieldOfViewAngle;
    protected float sightDistance;
    protected bool playerVisible;
    protected Vector3 playerPosition;
    [SerializeField] protected bool facingRight;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;

    [SerializeField] protected GameObject player;
    private void Start()
    {
        fieldOfViewAngle = 180f;
        sightDistance = 20f;
        playerVisible = false;
    }
    private void Update()
    {
        PlayerInFieldOfView();
    }

    // Check if the enemy is able to see the player
    protected virtual void PlayerInFieldOfView()
    {
        // Raycast direction from enemy to player
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, sightDistance, ~enemyLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // Check if the detected object is in the field of view
            float angle = Vector2.Angle(facingRight ? Vector2.right : Vector2.left, directionToPlayer);
            if (angle <= fieldOfViewAngle / 2)
            {
                Debug.Log($"Target in view: {hit.collider.gameObject.name}");
                playerVisible = true;
                playerPosition = hit.transform.position;
                // Ajoute ici la logique pour poursuivre ou attaquer la cible
            }
        }
        else
        {
            playerVisible = false;
            playerPosition = Vector3.zero;
        }



        //Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sightRadius, playerLayer);

        //foreach (Collider2D hit in hits)
        //{
        //    // Calculate the direction between the NPC and the object detected
        //    Vector2 directionTarget = (hit.transform.position - transform.position).normalized;

        //    // Check if the detected object is in the field of view
        //    float angle = Vector2.Angle(facingRight ? Vector2.right : Vector2.left, directionTarget);
        //    if (angle <= fieldOfViewAngle / 2)
        //    {
        //        Debug.Log($"Target in view: {hit.name}");
        //        playerVisible = true;
        //        playerPosition = hit.transform.position;
        //        // Ajoute ici la logique pour poursuivre ou attaquer la cible
        //    }
        //}
    }

    // Debug method only
    private void OnDrawGizmos()
    {
        if (transform.position != null)
        {
            if (player != null)
            {
                Gizmos.color = playerVisible ? Color.red : Color.yellow;
                Gizmos.DrawLine(transform.position, player.transform.position);               
            }
        }

    }


}
