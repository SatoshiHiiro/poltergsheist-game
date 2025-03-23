using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class HumanNPCBehaviour : BasicNPCBehaviour
{
    
    // Variable manage suspicion of the NPC
    [SerializeField] protected float minSuspiciousRotation; // Minimum rotation change in degrees to trigger suspicion
    [SerializeField] protected float minSuspiciousPosition; // Minimum position change to trigger suspicion
    protected bool canSee;  // Ability of the player to see

    [Header("Mirror")]
    [SerializeField] protected LayerMask mirrorLayer;   // Layer of the mirrors

    protected GameObject player;
    

    [Header("Investigation Variables")]
    [SerializeField] protected float surpriseWaitTime = 2f;
    [SerializeField] protected float investigationWaitTime = 3f;

    protected bool isInvestigating = false; // Is the NPC investigating a suspect sound
    public AudioSource audioSource;  // Source of the surprised sound

    protected Vector2 initialPosition;  // Initial position of the NPC
    private bool initialFacingRight; // He's he facing right or left

    protected Queue<IEnumerator> investigationQueue = new Queue<IEnumerator>();
    private bool isAtInitialPosition = false;
    private Coroutine currentInvestigation = null;

    [Header("Lighting Variable")]
    [SerializeField] float detectionRadiusLight = 20f;
    [SerializeField] LayerMask lightLayer;  // Layer of the gameobject light
    [SerializeField] LayerMask wallFloorLayer;   // Layer of the gameobject wall
    string visibleLayer = "Default";
    string notVisibleLayer = "NotVisible";
    int visibleLayerID;
    int notVisibleLayerID;


    protected bool seePolterg = false;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player");
        audioSource = GetComponent<AudioSource>();        
        initialPosition = transform.position;
        initialFacingRight = !npcSpriteRenderer.flipX;
        initialFloorLevel = currentFloorLevel;
        isAtInitialPosition = true;
        canSee = true;

        visibleLayerID = SortingLayer.NameToID(visibleLayer);
        notVisibleLayerID = SortingLayer.NameToID(notVisibleLayer);
    }

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

    // Verify if the object is in the field of view of the NPC
    protected override bool IsObjectInFieldOfView(Collider2D obj)
    {
        if (canSee)
        {
            // Check if any part of the object is seen 
            Vector2[] colliderPoints = LightUtility.GetSamplePointsFromObject(obj);

            return IsPointInFieldOfView(colliderPoints, obj);
        }
        return false;
       
        //foreach (Vector2 point in colliderPoints)
        //{
        //    // Check if the object is in the line of sight of the NPC
        //    Vector2 directionToPoint = (point - (Vector2)transform.position).normalized;
        //    float angle = Vector2.Angle(facingRight ? Vector2.right : Vector2.left, directionToPoint);

        //    // If the object is not within view angle, return false immediately
        //    if (angle > fieldOfViewAngle / 2)
        //    {
        //        continue;
        //    }

        //    // Check if there is light toutching the object
        //    if (!IsObjectLit(obj))
        //    {
        //        continue;
        //    }

        //    // Object is in field of view and area is sufficiently lit
        //    return true;
        //}
        //return false;

    }

    // Verifiy if the object is toutched by a light
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

    // Verifiy if any parts of the object is in the field of view
    protected bool IsPointInFieldOfView(Vector2[] colliderPoints, Collider2D objectCollider)
    {
        foreach (Vector2 point in colliderPoints)
        {


            // Check if the object is in the line of sight of the NPC
            Vector2 directionToPoint = (point - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(facingRight ? Vector2.right : Vector2.left, directionToPoint);
            
            // If the object is not within view angle, return false immediately
            if (angle > fieldOfViewAngle / 2)
            {
                continue;
            }

            SpriteRenderer objectSprite = objectCollider.GetComponent<SpriteRenderer>();
            // Check if there is light toutching the object
            if (!IsObjectLit(objectCollider))
            {
                if(objectSprite != null)
                {
                    objectSprite.sortingLayerID = notVisibleLayerID;
                }
                continue;
            }

            // Object is in field of view and area is sufficiently lit
            if (objectSprite != null)
            {
                int objectSortingLayer = objectSprite.sortingLayerID;
                objectSprite.sortingLayerID = visibleLayerID;
            }
            return true;
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
                Collider2D playerCollider = player.GetComponent<Collider2D>();
                Vector2[] reflectionPoints = mirror.GetReflectionPoints(playerCollider);
                //print(reflectionPoints[0]);
                //print(reflectionPoints.Length);
                //print("Is reflected in mirror");
                if (IsPointInFieldOfView(reflectionPoints, playerCollider))
                {
                    //print("In field of view");
                    // If nothing is blocking the sight of the NPC to the reflection of the player
                    if (!mirror.IsMirrorReflectionBlocked(reflectionPoints, playerCollider) && !seePolterg)
                    {
                        print("see");
                        NPCSeePolterg();
                    }
                } 

            }
        }
    }

    // When the Npc see Polterg it's gameover
    protected void NPCSeePolterg()
    {
        seePolterg = true;
        audioSource.Play();

        SuspicionManager.Instance.UpdateSeeingPoltergSuspicion();
    }

    // Start the investigation of the sound
    public virtual void InvestigateSound(SoundDetection objectsound, bool replaceObject, float targetFloor)
    {
        investigationQueue.Enqueue(InvestigateSoundObject(objectsound, replaceObject, targetFloor));
        //switch (objectsound.ObjectType)
        //{
        //    case SoundEmittingObject.FallingObject:
        //        investigationQueue.Enqueue(InvestigateFallingObject((FallingObject)objectsound, replaceObject, targetFloor));
        //        break;
        //    case SoundEmittingObject.SoundObject:
        //        investigationQueue.Enqueue(InvestigateSoundObject((JukeBox)objectsound,replaceObject, targetFloor));
        //        break;
        //    default:
        //        Debug.Log("Sound emitting object unknown");
        //        break;
        //}

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

    // NPC behaviour for sound emitting object investigation
    protected IEnumerator InvestigateSoundObject(SoundDetection objectsound, bool replaceObject, float targetFloor)
    {
        // Take a surprise pause before going on investigation
        audioSource.Play();
        yield return new WaitForSeconds(surpriseWaitTime);

        yield return (npcMovementController.ReachTarget(objectsound.transform.position, currentFloorLevel, targetFloor));

        // We can't find a path
        if (!npcMovementController.CanFindPath)
        {
            yield break;
        }

        // Wait a bit of time before going back to normal
        yield return new WaitForSeconds(investigationWaitTime);

        // The NPC who must reset the Object reset it (if it's the case)
        IResetObject resetObject = objectsound.GetComponent<IResetObject>();
        if(resetObject != null)
        {
            if (replaceObject)
            {
                resetObject.ResetObject();
            }
        }
    }

    //// NPC behaviour for the falling object investigation
    //protected IEnumerator InvestigateFallingObject(FallingObject objectsound, bool replaceObject, float targetFloor)
    //{

    //    // Take a surprise pause before going on investigation
    //    audioSource.Play();
    //    yield return new WaitForSeconds(surpriseWaitTime);

    //    yield return (npcMovementController.ReachTarget(objectsound.transform.position, currentFloorLevel ,targetFloor));

    //    // We can't find a path
    //    if (!npcMovementController.CanFindPath)
    //    {
    //        yield break;
    //    }
    //    //yield return StartCoroutine(ReachTarget(objectsound.transform.position, targetFloor));

    //    // Wait a bit of time before going back to normal
    //    yield return new WaitForSeconds(investigationWaitTime);

    //    // One NPC must replace the object to it's initial position
    //    FallingObject fallingObject = objectsound.GetComponent<FallingObject>();
    //    if (fallingObject != null && replaceObject)
    //    {
    //        fallingObject.ReplaceObject();
    //        // Animation ICI!
    //        fallingObject.FinishReplacement();
    //    }
   
    //}

    //protected IEnumerator InvestigateSoundObject(JukeBox soundObject, bool replaceObject, float targetFloor)
    //{
    //    // Take a surprise pause before going on investigation
    //    audioSource.Play();
    //    yield return new WaitForSeconds(surpriseWaitTime);

    //    yield return (npcMovementController.ReachTarget(soundObject.transform.position, currentFloorLevel, targetFloor));

    //    // We can't find a path
    //    if (!npcMovementController.CanFindPath)
    //    {
    //        yield break;
    //    }

    //    // Wait a bit of time before going back to normal
    //    yield return new WaitForSeconds(investigationWaitTime);
    //    if (replaceObject)
    //    {
    //        soundObject.StopSound();
    //    }
    //}



    // Return the NPC to it's initial position and facing direction
    public  IEnumerator ReturnToInitialPosition()
    {
        yield return StartCoroutine(npcMovementController.ReachTarget(initialPosition, currentFloorLevel, initialFloorLevel));//ReachTarget(initialPosition, initialFloorLevel));


        // Restore initial facing direction
        if(npcSpriteRenderer.flipX == initialFacingRight)
        {
            npcSpriteRenderer.flipX = !initialFacingRight;
            FlipFieldOfView();
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadiusLight);

        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
