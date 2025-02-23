using System.Collections;
using UnityEngine;

public interface IPatrol
{
    void Patrol();
}
public class PatrollingNPCBehaviour : HumanNPCBehaviour, IPatrol
{
    // Enemy patrol variables
    [Header("Patrol variables")]
    [SerializeField] protected PatrolPointData[] patrolPoints;    // Points were the NPC patrol
    protected int indexPatrolPoints;    // Next index patrol point    
    protected Animator animator;
    protected bool isBlocked;   // Is the NPC blocked in a room
    protected bool isWaiting;   // We wait for the animation to finish
    protected bool isInRoom;    // Is the NPC in a room
    protected bool isEnteringOrExiting; // Is the NPC entering or exiting a room?
    PatrolPointData currentPoint;   // Point where the NPC is located
    PatrolPointData nextPatrolPoint; // Next NPC patrol point

    protected override void Start()
    {
        base.Start();
        indexPatrolPoints = 0;
        animator = GetComponent<Animator>();
        isBlocked = false;
        isWaiting = false;
        isInRoom = false;
        currentPoint = null;
        nextPatrolPoint = patrolPoints[indexPatrolPoints];
    }
    protected override void Update()
    {
        // À CHANGER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Pour le sons
        base.Update();

        if(isBlocked && currentPoint != null && !IsRoomBlocked(currentPoint))
        {
            StartCoroutine(GetUnstuck());
            return;
        }
        if(!isWaiting && !isBlocked)
        {
            if (!isInvestigating)
            {
                Patrol();
            }
            //Patrol();
        }
        
    }
    // Start the investigation of the sound
    public override void InvestigateSound(GameObject objectsound, bool replaceObject)
    {       
       StartCoroutine(WaitBeforeInvestigate(objectsound, replaceObject));      
    }
    // When the NPC is no longer blocked or in a room he will then go and investigate
    protected IEnumerator WaitBeforeInvestigate(GameObject objectsound, bool replaceObject)
    {
        while(isBlocked || isInRoom)
        {
            yield return new WaitForSeconds(0.5f);
        }
        isInvestigating = true;
        isWaiting = false; // NOT SURE!!!
        StopAllCoroutines();
        StartCoroutine(InvestigateFallingObject(objectsound, replaceObject));
    }

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
        transform.position = Vector2.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);

        // Verify if the NPC has arrived
        if (Mathf.Abs(nextPatrolPoint.Point.position.x - transform.position.x) <= 0.2f)
        {
            // NPC has arrived to the patrol point
            currentPoint = nextPatrolPoint;
            StartCoroutine(HandleWaiting(currentPoint));
        }
        else
        {
            currentPoint = null;    // He's walking to the next patrol point
        }
    }

    // Check if there is a possessed object in front of a room
    protected bool IsRoomBlocked(PatrolPointData point)
    {
        if (point.PatrolPointType != PatrolPointType.Room || point.SpriteRenderer == null) return false;


        // Get the room sprite bounds
        Bounds roomBounds = point.SpriteRenderer.bounds;

        // Get the sprite width and height
        float roomWidth = roomBounds.size.x;
        float roomHeight = roomBounds.size.y;

        // Get object colliders in front of the room
        Collider2D[] colliders = Physics2D.OverlapBoxAll(point.SpriteRenderer.transform.position,
                                                        new Vector2(roomWidth,roomHeight),
                                                        0f, detectObjectLayer
                                                        );

        float blockedWidth = 0f;

        foreach (Collider2D collider in colliders)
        {
            // Skip if the object is not tall enough
            if (collider.bounds.size.y < point.MinimumBlockHeight)          
                continue;
            
                

            // Calculate how much width of the room is the object taking
            float objectWidth = Mathf.Min(collider.bounds.max.x, roomBounds.max.x)
                                - Mathf.Max(collider.bounds.min.x, roomBounds.min.x);
            if(objectWidth > 0)
            {
                blockedWidth += objectWidth;
            }
        }

        // Calculate what percentage of the entrance is blocked
        float blockPercentage = blockedWidth / roomWidth;

        return blockPercentage >= point.BlockingThreshold;
    }

    protected void MoveToNextAvailablePatrolPoint()
    {
        int patrolPointPossibilities = patrolPoints.Length;
        //bool findNextPoint = false;
        indexPatrolPoints++;
        if (indexPatrolPoints >= patrolPoints.Length)
        {
            indexPatrolPoints = 0;
        }
        nextPatrolPoint = patrolPoints[indexPatrolPoints];
        //for (int i = 0; i < patrolPointPossibilities; i++)
        //{
        //    indexPatrolPoints++;
        //    if (indexPatrolPoints >= patrolPoints.Length)
        //    {
        //        indexPatrolPoints = 0;
        //    }
        //    nextPatrolPoint = patrolPoints[indexPatrolPoints];
        //    //PatrolPointData nextPoint = patrolPoints[indexPatrolPoints];
        //    //if (nextPoint.PatrolPointType != PatrolPointType.Room || !IsRoomBlocked(nextPoint))
        //    //{
        //    //    nextPatrolPoint = nextPoint;
        //    //    findNextPoint = true;
        //    //    break;
        //    //}
        //}
        //if (!findNextPoint)
        //{
        //    nextPatrolPoint = null;    // All patrol points are blocked
        //}

    }

    protected IEnumerator HandleWaiting(PatrolPointData currentPoint)
    {
        isWaiting = true;
        if (currentPoint.PatrolPointType == PatrolPointType.Room)
        {
            // Case 1: NPC is not in a room and the room is not blocked
            if(!isInRoom && !IsRoomBlocked(currentPoint))
            {
                animator.SetBool("EnterRoom", true);
                isInRoom = true;
                yield return new WaitForSeconds(0.5f);                
                yield return new WaitForSeconds(currentPoint.WaitTime); // Waiting in the room

                // Check again if the room became blocked while waiting
                if (!IsRoomBlocked(currentPoint))
                {
                    animator.SetTrigger("ExitRoom");
                    yield return new WaitForSeconds(0.5f);
                    isInRoom = false;
                    animator.SetBool("EnterRoom", false);
                }
                else
                {
                    // The NPC is stuck
                    isWaiting = false;
                    isBlocked = true;
                    yield break;
                }
            }
            // Case 2: The NPC is not in a room and the room is blocked
            else if(!isInRoom && IsRoomBlocked(currentPoint))
            {
                yield return new WaitForSeconds(currentPoint.WaitTimeBlocked);
            }
            // Case 3: NPC is in a room and the room is blocked
            else if(isInRoom && IsRoomBlocked(currentPoint))
            {
                // The NPC is stuck
                isWaiting = false;
                isBlocked = true;
                yield break;
            }
        }
        else
        {
            yield return new WaitForSeconds(currentPoint.WaitTime);
        }
        isWaiting = false;
        MoveToNextAvailablePatrolPoint();
    }

    // NPC is not blocked anymore
    protected IEnumerator GetUnstuck()
    {
        // Animation of NPC coming out of the room
        isWaiting = true;
        isBlocked = false;
        animator.SetTrigger("ExitRoom");
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("EnterRoom", false);
        isWaiting = false;
        isInRoom = false;
        

        // After getting unstuck, move to the next patrol point
        MoveToNextAvailablePatrolPoint();
    }
}
