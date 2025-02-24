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
        return GeometryUtility.TestPlanesAABB(planes, objCollider.bounds);
    }

    // Check if there's anything blocking the mirror's view of the player
    public bool IsMirrorReflectionBlocked(Collider2D playerCollider)
    {
        //// Check if there is no object blocking the sight of the NPC
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, (obj.transform.position - transform.position).normalized, detectionRadius, ~ignoreLayerDetectObject);

        //// Is the path from the npc to the object clear?
        //if (hit.collider != null && hit.collider == obj)
        Vector2 directionToObject = (playerCollider.transform.position - transform.position).normalized;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, directionToObject, reflectionDistance, ~ignoreLayer);
        if(raycastHit.collider != null && raycastHit.collider != playerCollider)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
