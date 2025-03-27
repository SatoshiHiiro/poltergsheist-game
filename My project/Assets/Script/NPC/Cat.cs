using UnityEngine;
using System.Collections;

public class Cat : BasicNPCBehaviour, IPatrol
{
    [Header("Patrolling Variables")]  
    [SerializeField] PatrolPointData[] patrolPoints;  // All cat patrol destinations
    PatrolPointData nextPatrolPoint;  // Next cat patrol destination
    private int indexPatrolPoints;    // Keep track of patrol points
    private bool canMove;
    private PatrolPointData initialPatrolPoint;

    [Header("Hunting variables")]
    [SerializeField] float attackTime = 3f;
    [SerializeField] float huntingSpeed = 8f;
    [SerializeField] float maxHeightObject = 0.6f;  // Maximum height of an object that the cat can chase
    [SerializeField] float maxWidthObject = 0.6f;   //Maximum width of an object thtat the cat can chase
    //[SerializeField] float distanceWithTarget;  // Object targeted by the cat
    protected bool isHunting;   // Is the cat chasing an objet
    private bool isAttacking;   // Is the cat attacking the object
    private bool isPatrolling;
    GameObject targetPossessedObject; // Cat hunting target

    AudioSource audioSource;
    Collider2D catCollider;
    Coroutine patrolCoroutine;

    GameObject cage;    // Cage the cat is trapped in
   protected override void Start()
   {
        base.Start();
        indexPatrolPoints = 0;
        nextPatrolPoint = patrolPoints[0];
        initialPatrolPoint = nextPatrolPoint;
        isHunting = false;
        isAttacking = false;
        canMove = true;
        isPatrolling = false;

        audioSource = GetComponent<AudioSource>();
        catCollider = GetComponent<Collider2D>();
        
   }

    protected override void Update()
    {
        if (canMove)
        {
            base.Update();
            if (!isHunting && !isAttacking && !isPatrolling)
            {
                isPatrolling = true;
                patrolCoroutine = StartCoroutine(Patrol());
            }
            else if (isHunting && targetPossessedObject != null && !isAttacking)
            { 
                //StopAllCoroutines();
                StopCoroutine(patrolCoroutine);
                isPatrolling = false;
                StartCoroutine(ObjectHunting());
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
    public IEnumerator Patrol()
    {
        if (patrolPoints.Length == 0 || nextPatrolPoint == null) yield break;   // If there is no patrolPoint

        // Get movement direction
        Vector3 destination = new Vector3(nextPatrolPoint.Point.position.x, transform.position.y, transform.position.z);
        yield return npcMovementController.ReachTarget(destination, currentFloorLevel, nextPatrolPoint.FloorLevel);
        isPatrolling = false;
        MoveToNextAvailablePatrolPoint();
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
    private IEnumerator ObjectHunting()
    {
        isAttacking = true;
        audioSource.Play();

        // Continue hunting until the cat catches the object or loses track of it
        while (isHunting && targetPossessedObject != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position,
                                     (targetPossessedObject.transform.position - transform.position).normalized,
                                     detectionRadius,
                                     ~ignoreLayerSightBlocked);

            // Check if the possessed object is still in the field of view of the cat
            if (hit.collider == null || hit.collider.gameObject != targetPossessedObject)
            {
                isHunting = false;
                isAttacking = false;
                targetPossessedObject = null;
                //StartCoroutine(Patrol());
                yield break;
            }

            // Get the possessed object's position and movement
            Vector3 objectPosition = targetPossessedObject.transform.position;
            PossessionController possessionController = targetPossessedObject.GetComponent<PossessionController>();

            // Check if cat is directly beneath the object
            bool isCatUnderObject = Mathf.Abs(transform.position.x - objectPosition.x) < 0.5f;

            if (isCatUnderObject && objectPosition.y > transform.position.y)
            {
                // If directly under and object is higher, use object's movement direction
                if (possessionController != null)
                {
                    // Get movement direction from the possessed object
                    Vector2 objectDirection = possessionController.GetMovementDirection();
                    bool faceRight = objectDirection.x >= 0;

                    // Only update facing if the object is actually moving horizontally
                    if (possessionController.IsMoving)
                    {
                        if(faceRight != FacingRight)
                        {
                            npcSpriteRenderer.flipX = !faceRight;
                            FacingRight = faceRight;
                            FlipFieldOfView();
                        }
                        //npcSpriteRenderer.flipX = objectDirection.x < 0;
                        //facingRight = !npcSpriteRenderer.flipX;
                    }
                }
            }
            else
            {
                // Regular behavior - move toward object
                Vector3 destination = new Vector3(objectPosition.x, transform.position.y, objectPosition.z);
                Vector2 direction = (new Vector2(destination.x, destination.y) - (Vector2)transform.position).normalized;
                bool faceRight = direction.x >= 0;

                // Flip sprite based on direction
                if (faceRight != FacingRight)
                {
                    npcSpriteRenderer.flipX = !faceRight;
                    FacingRight = faceRight;
                    FlipFieldOfView();
                }

                // Flip sprite based on direction
                //npcSpriteRenderer.flipX = direction.x < 0;
                //facingRight = !npcSpriteRenderer.flipX;
            }

            // Cat is running towards the object target
            //transform.position = Vector2.MoveTowards(transform.position, destination, huntingSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, objectPosition.z);

            Vector3 moveDestination = new Vector3(objectPosition.x, transform.position.y, objectPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, moveDestination, huntingSpeed * Time.deltaTime);
            // Verify if the Cat toutched the object
            if (catCollider.bounds.Intersects(targetPossessedObject.GetComponent<Collider2D>().bounds))
            {
                isHunting = false;
                yield return AttackObject();
                break;
            }
            yield return null;
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
            isAttacking = false;
            yield break;
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
                fovLight.enabled = false;
                collision.GetComponentInParent<Animator>().SetBool("CloseCage", true);
                cage = collision.gameObject;
            }            
        }
    }

    public override void ResetInitialState()
    {
        base.ResetInitialState();
        StopAllCoroutines();
        isHunting = false;
        isAttacking = false;
        isPatrolling = false;
        nextPatrolPoint = initialPatrolPoint;
        canMove = true;
        fovLight.enabled = true;
        if(cage != null)
        {
            cage.GetComponentInParent<Animator>().SetBool("CloseCage", false);
        }

    }
}
