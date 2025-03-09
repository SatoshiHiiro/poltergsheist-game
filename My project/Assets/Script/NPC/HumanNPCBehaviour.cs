using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanNPCBehaviour : BasicNPCBehaviour
{
    
    // Variable manage suspicion of the NPC
    [SerializeField] protected float minSuspiciousRotation; // Minimum rotation change in degrees to trigger suspicion
    [SerializeField] protected float minSuspiciousPosition; // Minimum position change to trigger suspicion

    [Header("Mirror")]
    [SerializeField] protected LayerMask mirrorLayer;   // Layer of the mirrors

    protected GameObject player;
    

    [Header("Investigation Variables")]
    [SerializeField] protected float surpriseWaitTime = 2f;
    [SerializeField] protected float investigationWaitTime = 3f;
    [SerializeField] private float floorLevel;   // Floor where the npc is located
    protected float initialFloorLevel;
    public float FloorLevel { get { return floorLevel; } }
    protected bool isInvestigating = false; // Is the NPC investigating a suspect sound
    public AudioSource audioSource;  // Source of the surprised sound

    protected Vector2 initialPosition;  // Initial position of the NPC
    private bool initialFacingRight; // He's he facing right or left

    protected Queue<IEnumerator> investigationQueue = new Queue<IEnumerator>();
    private bool isAtInitialPosition = false;
    private Coroutine currentInvestigation = null;

    private bool canFindPath = true;

    public bool CanFindPath => canFindPath;

    [Header("Lighting Variable")]
    [SerializeField] float detectionRadiusLight = 20f;
    [SerializeField] LayerMask lightLayer;  // Layer of the gameobject light
    [SerializeField] LayerMask wallFloorLayer;   // Layer of the gameobject wall
    [SerializeField] protected float blindSpeed = 3f;
    protected float normalSpeed;



    protected override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player");
        audioSource = GetComponent<AudioSource>();        
        initialPosition = transform.position;
        initialFacingRight = !npcSpriteRenderer.flipX;
        initialFloorLevel = floorLevel;
        isAtInitialPosition = true;
        normalSpeed = movementSpeed;
    }

    bool test = false;
    private Coroutine returnToInitialPositionCoroutine;
    protected override void Update()
    {
        base.Update();
       // DetectMovingObjects();
        CheckMirrorReflection();

        if(investigationQueue.Count > 0 && !isInvestigating)
        {
            if (returnToInitialPositionCoroutine != null)
            {
                StopCoroutine(returnToInitialPositionCoroutine);
                //StopAllCoroutines();
                returnToInitialPositionCoroutine = null;
            }

            isAtInitialPosition = false;
            IEnumerator investigationCoroutine = investigationQueue.Dequeue();
            currentInvestigation = StartCoroutine(RunInvestigation(investigationCoroutine));
        }
        else if(investigationQueue.Count == 0 && !isInvestigating && !isAtInitialPosition)
        {
            isAtInitialPosition = true;
            returnToInitialPositionCoroutine = StartCoroutine(ReturnToInitialPosition());
        }
    }

    protected override void HandleChangedPositionSuspicion(PossessionController possessedObject, float objectSize)
    {
        if (!isObjectMoving)
        {
            // Check if the object has changed significantly of position and rotation
            float positionChange = Vector2.Distance(possessedObject.LastKnownPosition, possessedObject.transform.position);
            float rotationChange = Quaternion.Angle(possessedObject.LastKnownRotation, possessedObject.transform.rotation);

            // The object moved or rotated too much out of sight of the NPC
            if (positionChange >= minSuspiciousPosition || rotationChange >= minSuspiciousRotation)
            {
                //possessedObject.UpdateLastKnownPositionRotation();
                SuspicionManager.Instance.UpdateDisplacementSuspicion(objectSize, rotationChange, positionChange);
            }
        }
        // Update the new position and rotation of the object
        possessedObject.UpdateLastKnownPositionRotation();
    }

    protected override void HandleMovementSuspicion(float objectSize)
    {
        // If the NPC sees an object moving for the first time
        if (isObjectMoving && !isCurrentlyObserving)
        {
            isCurrentlyObserving = true;
            SuspicionManager.Instance.AddParanormalObserver();
        }
        // If the object has stopped moving
        else if (!isObjectMoving && isCurrentlyObserving)
        {
            isCurrentlyObserving = false;
            SuspicionManager.Instance.RemoveParanormalObserver();
        }
        // If the object is still moving
        if (isObjectMoving && isCurrentlyObserving)
        {
            SuspicionManager.Instance.UpdateMovementSuspicion(objectSize);
        }
    }

    protected override bool IsObjectInFieldOfView(Collider2D obj)
    {
        // Check if the object is in the line of sight of the NPC
        Vector2 directionToObject = (obj.transform.position - transform.position).normalized;
        float angle = Vector2.Angle(facingRight ? Vector2.right : Vector2.left, directionToObject);

        // If the object is not within view angle, return false immediately
        if(angle > fieldOfViewAngle / 2)
        {
            return false;
        }

        // Check if there is light toutching the object
        if (!IsObjectLit(obj))
        {
            return false;
        }

        // Object is in field of view and area is sufficiently lit
        return true;
    }

    protected bool IsObjectLit(Collider2D objCollider)
    {
        Collider2D[] lights = Physics2D.OverlapCircleAll(objCollider.bounds.center, detectionRadiusLight, lightLayer);
        foreach(Collider2D lightCollider in lights)
        {
            if(LightUtility.IsPointHitByLight(lightCollider, objCollider, wallFloorLayer))
            {
                
                return true;
            }
        }
        return false;
    }

    // Check if we can see the player trough the mirror
    protected void CheckMirrorReflection()
    {
        Collider2D[] mirrors = Physics2D.OverlapCircleAll(transform.position, detectionRadius, mirrorLayer);
        foreach (Collider2D mirrorCollider in mirrors)
        {
            Mirror mirror = mirrorCollider.GetComponentInParent<Mirror>();//GetComponent<Mirror>();
            if (mirror == null) continue;

            // Check if the mirror is in the field of view of the NPC
            if (!IsObjectInFieldOfView(mirrorCollider)) continue;
            
            // Check if the player reflection is in the mirror
            if (mirror.IsReflectedInMirror(player.GetComponent<Collider2D>()))
            {
                // If nothing is blocking the sight of the NPC to the reflection of the player
                if (!mirror.IsMirrorReflectionBlocked(player.GetComponent<Collider2D>()))
                {
                    // Player dies
                    print("DIE");
                }

            }
        }
    }
    // Start the investigation of the sound
    public virtual void InvestigateSound(GameObject objectsound, bool replaceObject, float targetFloor)
    {
        investigationQueue.Enqueue(InvestigateFallingObject(objectsound, replaceObject, targetFloor));
    }

    public void EnqueueInvestigation(IEnumerator investigation)
    {
        investigationQueue.Enqueue(investigation);
    }

    protected virtual IEnumerator RunInvestigation(IEnumerator investigation)
    {
        isInvestigating = true;
        yield return StartCoroutine(investigation);
        isInvestigating = false;
        currentInvestigation = null;
    }

    // NPC behaviour for the falling object investigation
    protected IEnumerator InvestigateFallingObject(GameObject objectsound, bool replaceObject, float targetFloor)
    {

        // Take a surprise pause before going on investigation
        audioSource.Play();
        yield return new WaitForSeconds(surpriseWaitTime);

        yield return (ReachTarget(objectsound.transform.position, targetFloor));

        if (!canFindPath)
        {
            yield break;
        }
        //yield return StartCoroutine(ReachTarget(objectsound.transform.position, targetFloor));

        // One NPC must replace the object to it's initial position
        FallingObject fallingObject = objectsound.GetComponent<FallingObject>();
        if (fallingObject != null && replaceObject)
        {
            fallingObject.ReplaceObject();
            // Animation ICI!
            fallingObject.FinishReplacement();
        }
        // Wait a bit of time before going back to normal
        yield return new WaitForSeconds(investigationWaitTime);     
    }

    // Pathfinding of the NPC to reach a target
    public IEnumerator ReachTarget(Vector2 target, float targetFloor)
    {
        canFindPath = true;
        yield return ReachFloor(targetFloor);
        //yield return StartCoroutine(ReachFloor(targetFloor));

        if (!canFindPath)
        {
            yield break;
        }

        // The NPC is now on the same level has the object

        Vector2 destination = new Vector2(target.x, transform.position.y);
        // Flip sprite based on direction
        UpdateSpriteDirection(destination);
        // The NPC must walk to the target
        yield return HorizontalMovementToTarget(destination);
    }

    protected IEnumerator ReachFloor(float targetFloor)
    {
        // Check if the npc need to use the stairs
        if (floorLevel != targetFloor)
        {
            // Find a path using stairs to reach the target floor
            List<StairController> path = FloorNavigation.Instance.FindPathToFloor(this, targetFloor);

            if(path == null)
            {
                canFindPath = false;
                yield break;
            }

            foreach (StairController stair in path)
            {
                StairController currentStair = stair;
                // NPC must walk to the stair
                Vector2 stairPosition = new Vector2(currentStair.StartPoint.position.x, transform.position.y);
                // Flip sprite based on direction
                UpdateSpriteDirection(stairPosition);

                // Determine if we need to go up or down
                StairDirection stairDirection = (targetFloor > floorLevel) ? StairDirection.Upward : StairDirection.Downward;

                StairController nextStairFloor = null;
                bool upward = false;

                if (stairDirection == StairDirection.Upward)
                {
                    nextStairFloor = currentStair.UpperFloor;
                    upward = true;
                }
                else if (stairDirection == StairDirection.Downward)
                {
                    nextStairFloor = currentStair.BottomFloor;
                    upward = false;
                }

                // Move to stair
                yield return HorizontalMovementToTarget(stairPosition);

                // Keep track of stairs we've already tried and found blocked
                List<StairController> blockedStairs = new List<StairController>();

                // Check if the stair is blocked
                if (currentStair.isStairBlocked() || nextStairFloor.isStairBlocked())
                {
                    bool findAlternative = false;
                    blockedStairs.Add(currentStair);    // Remove these stairs from the possible alternative to find a path
                    // As long as we did not find a new path
                    while (!findAlternative)
                    {
                        // Find if there is any other stair on the same level that can reach the target floor
                        for(int i = 0; i < FloorNavigation.Instance.StairsByFloorLevel[floorLevel].Count; i++)
                        {
                            StairController alternativeStair = FloorNavigation.Instance.FindNearestStairToFloor(this, targetFloor, stairDirection, blockedStairs);

                            if (alternativeStair != null)
                            {
                                nextStairFloor = upward ? alternativeStair.UpperFloor : alternativeStair.BottomFloor;
                            }                            
                            // Check if the stair is not blocked and if the upstair or downstair door is also not blocked
                            if (alternativeStair!= null && !alternativeStair.isStairBlocked() && nextStairFloor != null && !nextStairFloor.isStairBlocked())
                            {
                                // NPC must walk to the stair
                                Vector2 alternativeStairPosition = new Vector2(alternativeStair.StartPoint.position.x, transform.position.y);

                                // Flip sprite based on direction
                                UpdateSpriteDirection(alternativeStairPosition);

                                // Move to stair
                                yield return HorizontalMovementToTarget(alternativeStairPosition);

                                // Once the NPC reached the alternative stair check if it's blocked
                                if (!alternativeStair.isStairBlocked())
                                {
                                    currentStair = alternativeStair;
                                    findAlternative = true;
                                    break;
                                }
                                else
                                {
                                    // This stair is blocked and should not be considered anymore
                                    blockedStairs.Add(alternativeStair);
                                }

                            }
                        }
                        // All stairs are blocked so we wait and check if any of them got unblocked
                        yield return new WaitForSeconds(0.5f);
                        blockedStairs.Clear();
                    }
                }
                // Determine if we need to go up or down
                //StairDirection stairDirection = (targetFloor > floorLevel) ? StairDirection.Upward : StairDirection.Downward;
                // When the npc reached the stairs
                currentStair.ClimbStair(this.gameObject, stairDirection);

                // Wait for stair climbing to finish
                yield return new WaitForSeconds(1f);

                //Update our current floor
                if (stairDirection == StairDirection.Upward)
                {
                    floorLevel = currentStair.UpperFloor.FloorLevel;
                }
                else if (stairDirection == StairDirection.Downward)
                {
                    floorLevel = currentStair.BottomFloor.FloorLevel;
                }
            }
        }
    }

    public void UpdateFloorLevel(float currenrFloorLevel)
    {
        floorLevel = currenrFloorLevel;
    }

    // Return the NPC to it's initial position and facing direction
    public  IEnumerator ReturnToInitialPosition()
    {
        yield return StartCoroutine(ReachTarget(initialPosition, initialFloorLevel));
        // Restore initial facing direction
        npcSpriteRenderer.flipX = !initialFacingRight;
    }

    public void ChangeSpeed()
    {
        if (movementSpeed != normalSpeed)
        {
            movementSpeed = normalSpeed;
        }
        else
        {
            movementSpeed = blindSpeed;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadiusLight);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
