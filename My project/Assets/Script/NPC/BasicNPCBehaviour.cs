using UnityEngine;
using System.Collections;

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
    [SerializeField] protected float currentFloorLevel;   // Floor where the npc is located
    protected float initialFloorLevel;
    protected NPCMovementController npcMovementController;
    protected SpriteRenderer npcSpriteRenderer;


    // Getters
    public float FloorLevel { get { return currentFloorLevel; } }
    public NPCMovementController NpcMovementController { get { return npcMovementController; } }

    protected virtual void Start()
    {
        initialFloorLevel = currentFloorLevel;
        fieldOfViewAngle = 180f;
        isCurrentlyObserving = false;
        isObjectMoving = false;
        npcSpriteRenderer = GetComponent<SpriteRenderer>();
        npcMovementController = GetComponent<NPCMovementController>();
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

    // Debug method only
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
