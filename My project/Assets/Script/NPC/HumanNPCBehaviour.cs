using UnityEngine;
using System.Collections;

public class HumanNPCBehaviour : BasicNPCBehaviour
{
    [SerializeField] protected float movementSpeed = 6f;
    // Variable manage suspicion of the NPC
    [SerializeField] protected float minSuspiciousRotation; // Minimum rotation change in degrees to trigger suspicion
    [SerializeField] protected float minSuspiciousPosition; // Minimum position change to trigger suspicion

    [Header("Mirror")]
    [SerializeField] protected LayerMask mirrorLayer;   // Layer of the mirrors

    protected GameObject player;
    

    [Header("Investigation Variables")]
    [SerializeField] protected float surpriseWaitTime = 2f;
    [SerializeField] protected float investigationWaitTime = 3f;
    protected bool isInvestigating = false; // Is the NPC investigating a suspect sound
    protected AudioSource audioSource;  // Source of the surprised sound

    protected Vector2 initialPosition;
    private bool initialFacingRight; // He's he facing right or left
    

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player");
        audioSource = GetComponent<AudioSource>();        
        initialPosition = transform.position;
        initialFacingRight = !npcSpriteRenderer.flipX;
    }

    protected override void Update()
    {
        base.Update();
        CheckMirrorReflection();
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
    public virtual void InvestigateSound(GameObject objectsound, bool replaceObject)
    {
        isInvestigating = true;
        StopAllCoroutines();
        StartCoroutine(InvestigateAndReturn(objectsound, replaceObject));
        //StartCoroutine(InvestigateFallingObject(objectsound, replaceObject));
        //StartCoroutine(ReturnToInitialPosition());
    }

    protected IEnumerator InvestigateAndReturn(GameObject objectsound, bool replaceObject)
    {
        yield return StartCoroutine(InvestigateFallingObject(objectsound, replaceObject));
        yield return StartCoroutine(ReturnToInitialPosition());
    }
    // NPC behaviour for the investigation
    protected IEnumerator InvestigateFallingObject(GameObject objectsound, bool replaceObject)
    {
        // Take a surprise pause before going on investigation
        audioSource.Play();
        yield return new WaitForSeconds(surpriseWaitTime);



        // Go towards the sound
        Vector2 destination = new Vector2(objectsound.transform.position.x, transform.position.y);
        // Sprite face the right direction
        Vector2 direction = (destination - (Vector2)transform.position).normalized;
        npcSpriteRenderer.flipX = direction.x < 0;

        while (Mathf.Abs(transform.position.x - objectsound.transform.position.x) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            yield return null;
        }

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
        isInvestigating = false;

    }

    // Return the NPC to it's initial position and facing direction
    protected IEnumerator ReturnToInitialPosition()
    {
        // Calculate destination
        Vector2 destination = new Vector2(initialPosition.x, transform.position.y);

        // Flip sprite based on direction
        Vector2 direction = (destination - (Vector2)transform.position).normalized;
        npcSpriteRenderer.flipX = direction.x < 0;
        facingRight = !npcSpriteRenderer.flipX;

        // Move to initial position
        while (Mathf.Abs(transform.position.x - initialPosition.x) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            yield return null;
        }
        // Restore initial facing direction
        npcSpriteRenderer.flipX = !initialFacingRight;
    }
}
