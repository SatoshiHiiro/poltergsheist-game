using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] private Camera mirrorCamera;
    private float reflectionDistance;   // Distance between the mirror and it's reflection
    private Plane[] planes;
    [SerializeField] private LayerMask ignoreLayer;

    private void Start()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(mirrorCamera);
        reflectionDistance = mirrorCamera.orthographicSize;
        //print("CAMERA!" + mirrorCamera.orthographicSize);
    }

    // Check if the object is reflected in the mirror
    public bool IsReflectedInMirror(Collider2D objCollider)
    {
        if (objCollider.enabled == false)
        {
            return false;
        }
        planes = GeometryUtility.CalculateFrustumPlanes(mirrorCamera);
        return GeometryUtility.TestPlanesAABB(planes, objCollider.bounds);
    }

    // Check if there's anything blocking the mirror's view of the player
    public bool IsMirrorReflectionBlocked(Collider2D playerCollider)
    {
        //// Check if there is no object blocking the sight of the NPC
        if (playerCollider == null) return true;

        // Get the bounds of the player collider
        Bounds playerBounds = playerCollider.bounds;

        // Get the bounds of the mirror (assuming the mirror has a collider)
        Bounds mirrorBounds = GetComponent<Collider2D>().bounds;

        // Cast rays from multiple points on the mirror to various points on the player's collider
        Vector2[] mirrorPoints = {
        new Vector2(mirrorBounds.center.x, mirrorBounds.center.y),                  // Center
        new Vector2(mirrorBounds.min.x, mirrorBounds.min.y),                        // Bottom-left
        new Vector2(mirrorBounds.max.x, mirrorBounds.min.y),                        // Bottom-right
        new Vector2(mirrorBounds.min.x, mirrorBounds.max.y),                        // Top-left
        new Vector2(mirrorBounds.max.x, mirrorBounds.max.y)};                      // Top-right


        Vector2[] playerPoints = {
        new Vector2(playerBounds.center.x, playerBounds.center.y),                  // Center
        new Vector2(playerBounds.min.x, playerBounds.min.y),                        // Bottom-left
        new Vector2(playerBounds.max.x, playerBounds.min.y),                        // Bottom-right
        new Vector2(playerBounds.min.x, playerBounds.max.y),                        // Top-left
        new Vector2(playerBounds.max.x, playerBounds.max.y)};                      // Top-right



        // Check if any ray from the mirror to the player is unobstructed
        foreach (Vector2 mirrorPoint in mirrorPoints)
        {
            foreach (Vector2 playerPoint in playerPoints)
            {
                Vector2 direction = (playerPoint - mirrorPoint).normalized;
                float distance = Vector2.Distance(mirrorPoint, playerPoint);

                // Cast ray from mirror point to player point
                RaycastHit2D hit = Physics2D.Raycast(mirrorPoint, direction, distance, ~ignoreLayer);

                // If ray directly hits the player without obstruction, reflection is not blocked
                if (hit.collider == playerCollider)
                {
                    return false; // Found at least one unobstructed view
                }
            }
        }

        // All rays were obstructed, so reflection is blocked
        return true;




        //Vector2 directionToObject = (playerCollider.transform.position - transform.position).normalized;
        //RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, directionToObject, reflectionDistance, ~ignoreLayer);
        //if(raycastHit.collider != null && raycastHit.collider != playerCollider)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }
}
