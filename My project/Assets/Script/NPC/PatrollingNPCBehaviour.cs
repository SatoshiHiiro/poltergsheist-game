using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;
using UnityEngine;

public interface IPatrol
{
    IEnumerator Patrol();
    void MoveToNextAvailablePatrolPoint();
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
    PatrolPointData currentPoint;   // Point where the NPC is located
    PatrolPointData nextPatrolPoint; // Next NPC patrol point
    //private bool rightFloor;    // Does the NPC on the right floor to do is patrol
    //private bool isWalkingBack;    // Does the NPC go up the stairs
    private bool isPatrolling;


    // Public properties to access from other scripts
    public bool IsBlocked => isBlocked;
    public bool IsInRoom => isInRoom;

    private Coroutine returnToFloor;
    private Coroutine patrolling;
    protected override void Start()
    {
        base.Start();
        indexPatrolPoints = 0;
        animator = GetComponent<Animator>();
        isBlocked = false;
        isWaiting = false;
        isInRoom = false;
        //rightFloor = true;
        //isWalkingBack = false;
        isPatrolling = false;   
        currentPoint = null;
        if(patrolPoints.Length > 0)
        {
            nextPatrolPoint = patrolPoints[indexPatrolPoints];
        }
        
    }
    protected override void Update()
    {
        DetectMovingObjects();
        CheckMirrorReflection();

        // Priority 1: If the NPC is blocked but the room is no longer blocked, get unstuck
        if(isBlocked && currentPoint != null && !IsRoomBlocked(currentPoint))
        {
            StartCoroutine(GetUnstuck());
            return;
        }
        // Priority 2: Handle investigation queue if we're not currently investigating or getting unstuck
        if(investigationQueue.Count > 0 && !isInvestigating && !isWaiting)
        {
            if(returnToFloor != null)
            {
                StopCoroutine(returnToFloor);
                returnToFloor = null;
            }
            isInvestigating = true;
            //StopCoroutine("HandleWaiting");
            //StopCoroutine("Patrol");
            StopCoroutine(patrolling);
            isPatrolling = false;
            IEnumerator investigationCoroutine = investigationQueue.Dequeue();
            StartCoroutine(RunInvestigation(investigationCoroutine));
        }
        // Priority 3: Return to starting floor if we need to
        //else if(investigationQueue.Count == 0 && !isInvestigating && !rightFloor && isWalkingBack)
        //{
        //    isWalkingBack = false;
        //    returnToFloor = StartCoroutine(ReturnRightFloor());
        //}
        // Priority 4: Patrol if we're able to and should be
        else if(investigationQueue.Count == 0 && !isInvestigating && !isWaiting && !isBlocked && !isPatrolling)
        {
            patrolling = StartCoroutine(Patrol());
        }

    }

    protected override IEnumerator RunInvestigation(IEnumerator investigation)
    {
        // Wait until we're not blocked, not in a room, and not getting unstuck
        while (isBlocked || isInRoom || isWaiting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        //isWalkingBack = true;
        //rightFloor = false;
        print(investigation.ToString());
        yield return StartCoroutine(investigation);
        isInvestigating = false;        
    }

    // Start the investigation of the sound
    //public override void InvestigateSound(SoundDetection objectsound, bool replaceObject, float targetFloor)
    //{
    //    investigationQueue.Enqueue(InvestigateFallingObject(objectsound, replaceObject, targetFloor));      
    //}

    public IEnumerator ReturnRightFloor()
    {
        yield return StartCoroutine(npcMovementController.ReachFloor(currentFloorLevel, initialFloorLevel));//ReachFloor(initialFloorLevel));
        if (FloorLevel == initialFloorLevel)
        {
            //rightFloor = true;
        }

    }

    public IEnumerator Patrol()
    {
        if (patrolPoints.Length == 0 || nextPatrolPoint == null) yield break;   // If there is no patrolPoint
        isPatrolling = true;
        // Get movement direction
        Vector2 destination = new Vector2(nextPatrolPoint.Point.position.x, transform.position.y);

        currentPoint = null;
        yield return npcMovementController.ReachTarget(destination, currentFloorLevel, nextPatrolPoint.FloorLevel);

        // NPC has arrived to the patrol point
        currentPoint = nextPatrolPoint;
        yield return HandleWaiting(currentPoint);
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
                canSee = false;
                yield return new WaitForSeconds(currentPoint.WaitTime); // Waiting in the room

                // Check again if the room became blocked while waiting
                if (!IsRoomBlocked(currentPoint))
                {
                    animator.SetTrigger("ExitRoom");
                    yield return new WaitForSeconds(0.5f);
                    isInRoom = false;
                    canSee = true;
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
            //isWaiting = false;
            yield return new WaitForSeconds(currentPoint.WaitTime);
        }
        isWaiting = false;
        isPatrolling = false;
        MoveToNextAvailablePatrolPoint();
    }

    // NPC is not blocked anymore
    protected IEnumerator GetUnstuck()
    {
        //isGettingUnstuck = true;
        // Animation of NPC coming out of the room
        isWaiting = true;
        isBlocked = false;
        animator.SetTrigger("ExitRoom");
        yield return new WaitForSeconds(0.5f);
        canSee = true;
        animator.SetBool("EnterRoom", false);
        isWaiting = false;
        print("ISWAITING2 " + isWaiting); 
        isInRoom = false;
        isPatrolling = false;


        // After getting unstuck, move to the next patrol point
        //isGettingUnstuck = false;
        MoveToNextAvailablePatrolPoint();
    }
}
