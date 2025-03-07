using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using static UnityEngine.GraphicsBuffer;

public abstract class BasicNPCBehaviour : MonoBehaviour
{
    // NPC vision variables
    [Header("Field of view")]
    [SerializeField] protected float detectionRadius = 10f;  // NPC detection radius
    [SerializeField] protected bool facingRight;        // Is the NPC Sprite facing right    
    [SerializeField] protected LayerMask detectObjectLayer;   // Layer of objects to be detected by the NPC    
    [SerializeField] protected LayerMask ignoreLayerSightBlocked;   // Layer to ignore when raycasting to check if the view is blocked
    protected bool isObjectMoving;    // Is there an object moving in front of him?
    protected bool isCurrentlyObserving;    // Is the NPC already watching an object moving?
    protected float fieldOfViewAngle;

    [Header("NPC global variables")]
    [SerializeField] protected float movementSpeed = 6f;

    protected SpriteRenderer npcSpriteRenderer;



    protected virtual void Start()
    {
        fieldOfViewAngle = 180f;
        isCurrentlyObserving = false;
        isObjectMoving = false;
        npcSpriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Update()
    {
        DetectMovingObjects();
    }

    // Movement detection of the NPC
    protected virtual void DetectMovingObjects()
    {
        isObjectMoving = false;
        float objectSize = 0f;

        // Find all the possible possessed object in the room
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectObjectLayer);
        foreach (Collider2D obj in objects)
        {

            if (this.IsObjectInFieldOfView(obj))
            {
                // Check if there is no object blocking the sight of the NPC
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (obj.transform.position - transform.position).normalized, detectionRadius, ~ignoreLayerSightBlocked);

                // Is the path from the npc to the object clear?
                if (hit.collider != null && hit.collider == obj)
                {
                    // Get object size
                    Renderer objRenderer = obj.GetComponent<Renderer>();
                    objectSize = Mathf.Max(objRenderer.bounds.size.x, objRenderer.bounds.size.y);

                    // Check if the object is moving
                    PossessionController possessedObject = obj.GetComponent<PossessionController>();

                    if (possessedObject != null)
                    {
                        // Check if the object is moving in front of him
                        if (possessedObject.IsMoving)
                        {
                            isObjectMoving = true;
                        }

                        HandleChangedPositionSuspicion(possessedObject, objectSize);
                    }


                }
            }
        }
        HandleMovementSuspicion(objectSize);
    }
    protected virtual void HandleChangedPositionSuspicion(PossessionController possessedObject, float objectSize)
    {
        // Empty default implementation
        // Will be override in HumanNPCBehaviour
    }

    protected virtual void HandleMovementSuspicion(float objectSize)
    {
        // Empty default implementation
        // Will be override in HumanNPCBehaviour
    }

    protected virtual bool IsObjectInFieldOfView(Collider2D obj)
    {
        // Check if the object is in the line of sight of the NPC
        Vector2 directionToObject = (obj.transform.position - transform.position).normalized;
        float angle = Vector2.Angle(facingRight ? Vector2.right : Vector2.left, directionToObject);
        return angle <= fieldOfViewAngle / 2;
    }

    // Change the sprite depending on the direction the npc is walking
    protected void UpdateSpriteDirection(Vector2 destination)
    {        
        // Flip sprite based on direction
        Vector2 npcDirection = (destination - (Vector2)transform.position).normalized;
        // Sprite face the right direction
        npcSpriteRenderer.flipX = npcDirection.x < 0;
        facingRight = !npcSpriteRenderer.flipX;
    }

    // The NPC walks horizontally to a given destination
    protected IEnumerator HorizontalMovementToTarget(Vector2 destination)
    {
        // The NPC must walk until it reaches its destination
        while (Mathf.Abs(transform.position.x - destination.x) > 0.1f)
        {
            // NPC walk towards the destination
            transform.position = Vector2.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Debug method only
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
