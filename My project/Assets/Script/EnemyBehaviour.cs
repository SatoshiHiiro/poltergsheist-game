using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // Enemy vision variables
    protected float fieldOfViewAngle;
    [SerializeField]  protected float detectionRadius;  // NPC detection radius
    [SerializeField] protected bool facingRight;        // Is the NPC Sprite facing right    
    [SerializeField] protected LayerMask detectLayer;   // Layer of objects to be detected by the NPC
    [SerializeField] protected LayerMask ignoreLayer;   // Layer to ignore when raycasting
    [SerializeField] protected GameObject player;

    // Variable manage suspicion of the NPC
    [SerializeField] protected float minSuspiciousRotation; // Minimum rotation change in degrees to trigger suspicion
    [SerializeField] protected float minSuspiciousPosition; // Minimum position change to trigger suspicion
    private bool isCurrentlyObserving;                      // Is the NPC already watching an object moving

    // Enemy patrol variables
    [SerializeField] protected Transform[] patrolPoints;    // Points were the NPC patrol
    protected int indexPatrolPoints;    // Next index patrol point

    [SerializeField] protected float speed;
    private void Start()
    {
        fieldOfViewAngle = 180f;
        detectionRadius = 10f;
        isCurrentlyObserving = false;

        indexPatrolPoints = 0;
    }
    private void Update()
    {
        Patrol();
        DetectMovingObjects();
    }

    // Movement detection of the NPC
    protected virtual void DetectMovingObjects()
    {
        bool isObjectMoving = false;
        float objectSize = 0f;

        // Find all the possible possessed object in the room
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, detectionRadius,detectLayer);
        foreach (Collider2D obj in objects)
        {
            // Check if the object is in the line of sight of the NPC
            Vector2 directionToObject = (obj.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(facingRight ? Vector2.right : Vector2.left, directionToObject);

            if(angle <= fieldOfViewAngle / 2)
            {
                // Check if there is no object blocking the sight of the NPC
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (obj.transform.position - transform.position), detectionRadius, ~ignoreLayer);
                
                // Is the path from the npc to the object clear?
                if (hit.collider != null && hit.collider == obj)
                {
                    // Get object size
                    Renderer objRenderer = obj.GetComponent<Renderer>();
                    objectSize = Mathf.Max(objRenderer.bounds.size.x, objRenderer.bounds.size.y);

                    // Check if the object is moving
                    PossessionController possessedObject = obj.GetComponent<PossessionController>();

                    if(possessedObject != null && possessedObject.IsMoving)
                    {
                        isObjectMoving = true;                        
                    }
                    else
                    {
                        // Check if the object has changed significantly of position and rotation
                        float positionChange = Vector2.Distance(possessedObject.LastKnownPosition, obj.transform.position);
                        float rotationChange = Quaternion.Angle(possessedObject.LastKnownRotation, obj.transform.rotation);

                        // The object moved or rotated too much out of sight of the NPC
                        if(positionChange >= minSuspiciousPosition || rotationChange >= minSuspiciousRotation)
                        {

                            print("HERE!");
                            //possessedObject.UpdateLastKnownPositionRotation();
                            SuspicionManager.Instance.UpdateDisplacementSuspicion(objectSize, rotationChange, positionChange);
                        }
                    }
                    // Update the new position and rotation of the object
                    possessedObject.UpdateLastKnownPositionRotation();
                }
            }
        }
        // If the NPC sees an object moving for the first time
        if(isObjectMoving && !isCurrentlyObserving)
        {
            isCurrentlyObserving = true;
            SuspicionManager.Instance.AddParanormalObserver();
        }
        // If the object has stopped moving
        else if(!isObjectMoving && isCurrentlyObserving)
        {
            isCurrentlyObserving = false;
            SuspicionManager.Instance.RemoveParanormalObserver();
        }
        // If the object is still moving
        if(isObjectMoving && isCurrentlyObserving)
        {
            SuspicionManager.Instance.UpdateMovementSuspicion(objectSize);
        }
        
    }

    protected void Patrol()
    {
        // Get movement direction
        Vector2 destination = new Vector2(patrolPoints[indexPatrolPoints].position.x, transform.position.y);
        Vector2 direction = (destination - (Vector2)transform.position).normalized;

        // Flip sprite based on direction
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = direction.x < 0;
        facingRight = !spriteRenderer.flipX;

        // Move towards destination
        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        // If the enemy reaches his destination, he is given a new destination to patrol
        if (Mathf.Abs(patrolPoints[indexPatrolPoints].position.x - transform.position.x) <= 0.2f)
        {
            indexPatrolPoints++;
            if(indexPatrolPoints >= patrolPoints.Length)
            {
                indexPatrolPoints = 0;
            }
        }
    }



    // Debug method only
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


}
