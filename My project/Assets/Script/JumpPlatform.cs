using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class JumpPlatform : MonoBehaviour
{
    // This class allow the player to jump on specific objects
    [SerializeField] float rangeUntilObjectBecomesAPlatform;

    private GameObject player;
    private Collider2D objectCollider;
    Collider2D playerCollider;
    private LayerMask playerLayer;
    //[SerializeField]float threshold = 0.4f; // Prevent flickering when we enable the collision
    private void Start()
    {
        objectCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider2D>();
        playerLayer = 1 << player.layer;

        if(player == null)
        {
            Debug.LogError("No player found in the scene!");
        }
    }

    private void Update()
    {
        // Find platform surface position
        //float platformSurface = this.transform.position.y + ((objectCollider.bounds.size.y / 2));
        float platformSurface = transform.position.y + ((/*transform.lossyScale.y */ gameObject.GetComponent<Collider2D>().bounds.size.y) / 2);
        // Find player feet position
        //float playerFeetPosition = player.transform.position.y - (playerCollider.bounds.size.y / 2);
        float playerFeetPosition = player.transform.position.y - player.GetComponent<PlayerController>().halfSizeOfObject;
        // Check if the player is above the platform
        if (playerFeetPosition + rangeUntilObjectBecomesAPlatform > platformSurface)
        {
            EnableCollision();
        }
        else if(playerFeetPosition < platformSurface)
        {
            DisableCollision();
        }
    }
    // Re-enable collision with player layer
    private void EnableCollision()
    {
        objectCollider.excludeLayers &= ~playerLayer;
    }
    // Exclude collision with player layer
    private void DisableCollision()
    {
        objectCollider.excludeLayers |= playerLayer;
    }

    private void OnDrawGizmos()
    {
        if (objectCollider != null)
        {
            float platformSurface = transform.position.y + (objectCollider.bounds.size.y / 2);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                new Vector3(transform.position.x - 0.5f, platformSurface, 0),
                new Vector3(transform.position.x + 0.5f, platformSurface, 0)
            );
        }
    }
}
