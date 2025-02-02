using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // Enemy vision variables
    protected float fieldOfViewAngle;
    [SerializeField]  protected float detectionRadius;
    [SerializeField] protected bool facingRight;
    [SerializeField] protected LayerMask detectLayer;   // Layer of objects to be detected by the NPC
    [SerializeField] protected LayerMask ignoreLayer;    // Layer to ignore when raycasting
    [SerializeField] protected GameObject player;

    private bool isCurrentlyObserving;

    // Enemy patrol variables
    [SerializeField] protected Transform[] patrolPoints;
    private void Start()
    {
        fieldOfViewAngle = 180f;
        detectionRadius = 10f;

        isCurrentlyObserving = false;
    }
    private void Update()
    {
        DetectMovingObjects();
    }

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
                    
                    // Check if the object is moving
                    PossessionController possessedObject = obj.GetComponent<PossessionController>();

                    if(possessedObject != null && possessedObject.IsMoving)
                    {
                        isObjectMoving = true;
                        Renderer objRenderer = obj.GetComponent<Renderer>();
                        objectSize = Mathf.Max(objRenderer.bounds.size.x, objRenderer.bounds.size.y);
                        print(objectSize);
                    }
                    break;  // Only one object can be possesed at the time
                }
            }
        }

        if(isObjectMoving && !isCurrentlyObserving)
        {
            isCurrentlyObserving = true;
            SuspicionManager.Instance.AddParanormalObserver();
        }
        else if(!isObjectMoving && isCurrentlyObserving)
        {
            isCurrentlyObserving = false;
            SuspicionManager.Instance.RemoveParanormalObserver();
        }
        else if(isObjectMoving && isCurrentlyObserving)
        {

            SuspicionManager.Instance.UpdateMovementSuspicion(objectSize);
        }
        
    }

    // Debug method only
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


}
