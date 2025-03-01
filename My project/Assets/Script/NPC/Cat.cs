using UnityEngine;
using System.Collections;

public class Cat : BasicNPCBehaviour, IPatrol
{
    [Header("Patrolling Variables")]
    [SerializeField] float walkingSpeed = 3f;    
    [SerializeField] PatrolPointData[] patrolPoints;  // All cat patrol destinations
    PatrolPointData nextPatrolPoint;  // Next cat patrol destination
    private int indexPatrolPoints;    // Keep track of patrol points
    private bool canMove;

    [Header("Hunting variables")]
    [SerializeField] float attackTime = 3f;
    [SerializeField] float huntingSpeed = 8f;
    [SerializeField] float maxHeightObject = 0.6f;  // Maximum height of an object that the cat can chase
    [SerializeField] float maxWidthObject = 0.6f;   //Maximum width of an object thtat the cat can chase
    //[SerializeField] float distanceWithTarget;  // Object targeted by the cat
    protected bool isHunting;   // Is the cat chasing an objet
    private bool isAttacking;   // Is the cat attacking the object
    GameObject targetPossessedObject; // Cat hunting target

    AudioSource audioSource;
    Collider2D catCollider;
    
   protected override void Start()
   {
        base.Start();
        indexPatrolPoints = 0;
        nextPatrolPoint = patrolPoints[0];
        isHunting = false;
        canMove = true;

        audioSource = GetComponent<AudioSource>();
        catCollider = GetComponent<Collider2D>();
   }

    protected override void Update()
    {
        if (canMove)
        {
            base.Update();
            if (!isHunting && !isAttacking)
            {
                Patrol();
            }
            else if (isHunting && targetPossessedObject != null)
            {
                ObjectHunting();
            }
        }        
    }

    // Cat movement detection
    protected override void DetectMovingObjects()
    {        
        float objectWidth = 0f;
        float objectHeight = 0f;

        // Find all the possible possessed object in the room
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectObjectLayer);
        foreach (Collider2D obj in objects)
        {

            if (IsObjectInFieldOfView(obj))
            {
                // Check if there is no object blocking the sight of the NPC
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (obj.transform.position - transform.position).normalized, detectionRadius, ~ignoreLayerSightBlocked);

                // Is the path from the npc to the object clear?
                if (hit.collider != null && hit.collider == obj)
                {
                    // Get object size
                    Renderer objRenderer = obj.GetComponent<Renderer>();

                    //objectSize = Mathf.Max(objRenderer.bounds.size.x, objRenderer.bounds.size.y);
                    objectWidth = objRenderer.bounds.size.x;
                    objectHeight = objRenderer.bounds.size.y;

                    // Check if the object is moving
                    PossessionController possessedObject = obj.GetComponent<PossessionController>();

                    if (possessedObject != null)
                    {
                        // Check if the object is moving in front of him
                        if (possessedObject.IsMoving && !isHunting && objectWidth <= maxWidthObject && objectHeight <= maxHeightObject)
                        {
                            isObjectMoving = true;
                            isHunting = true;
                            targetPossessedObject = possessedObject.gameObject;
                        }
                    }
                }
            }
        }
        
    }

    // Patrolling of the cat
    public void Patrol()
    {
        if (patrolPoints.Length == 0 || nextPatrolPoint == null) return;   // If there is no patrolPoint

        // Get movement direction
        Vector2 destination = new Vector2(nextPatrolPoint.Point.position.x, transform.position.y);
        Vector2 direction = (destination - (Vector2)transform.position).normalized;

        // Flip sprite based on direction
        npcSpriteRenderer.flipX = direction.x < 0;
        facingRight = !npcSpriteRenderer.flipX;

        // Move towards destination
        transform.position = Vector2.MoveTowards(transform.position, destination, walkingSpeed * Time.deltaTime);

        // Verify if the NPC has arrived
        if (Mathf.Abs(nextPatrolPoint.Point.position.x - transform.position.x) <= 0.2f)
        {
            // NPC has arrived to the patrol point
            MoveToNextAvailablePatrolPoint();
        }
    }

    // Which patrol point is the new destination of the cat
    public void MoveToNextAvailablePatrolPoint()
    {
        int patrolPointPossibilities = patrolPoints.Length;
        //bool findNextPoint = false;
        indexPatrolPoints++;
        if (indexPatrolPoints >= patrolPoints.Length)
        {
            indexPatrolPoints = 0;
        }
        nextPatrolPoint = patrolPoints[indexPatrolPoints];
    }

    // The cat must chase any object it sees moving
    private void ObjectHunting()
    {
        audioSource.Play();

        // Get movement direction
        Vector2 destination = new Vector2(targetPossessedObject.transform.position.x, transform.position.y);
        Vector2 direction = (destination - (Vector2)transform.position).normalized;

        // Flip sprite based on direction
        npcSpriteRenderer.flipX = direction.x < 0;
        facingRight = !npcSpriteRenderer.flipX;

        // Cat is running towards the object target
        transform.position = Vector2.MoveTowards(transform.position, destination, huntingSpeed * Time.deltaTime);

        // Verify if the Cat toutched the object
        if (catCollider.bounds.Intersects(targetPossessedObject.GetComponent<Collider2D>().bounds))
        {
            isHunting = false;
            //Attack();
            StartCoroutine(AttackObject());
        }
    }

    // The cat attack the possessed object
    private IEnumerator AttackObject()
    {
        isAttacking = true;
        audioSource.Play();
        PossessionManager targetObjectManager = targetPossessedObject.GetComponent<PossessionManager>();
        if (targetObjectManager == null)
        {
            Debug.Log("Error: The possessed object should have a PossessionManager component");
        }
        targetObjectManager.StopPossession();

        yield return new WaitForSeconds(attackTime);


        // After the attack the object is no longer a target
        targetPossessedObject = null;
        isAttacking = false;
    }

    // If the cat enters the cage it remains trapped.
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canMove && collision.gameObject.CompareTag("Cage"))
        {
            if(collision.bounds.Contains(catCollider.bounds.min) && collision.bounds.Contains(catCollider.bounds.max))
            {
                audioSource.Play();
                canMove = false;
                collision.GetComponentInParent<Animator>().SetBool("CloseCage", true);
            }            
        }
    }
}
