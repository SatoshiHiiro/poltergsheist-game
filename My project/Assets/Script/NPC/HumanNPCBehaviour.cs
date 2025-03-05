using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float floorLevel;   // Floor where the npc is located
    private float initialFloorLevel;
    public float FloorLevel { get { return floorLevel; } }
    protected bool isInvestigating = false; // Is the NPC investigating a suspect sound
    protected AudioSource audioSource;  // Source of the surprised sound

    protected Vector2 initialPosition;  // Initial position of the NPC
    private bool initialFacingRight; // He's he facing right or left

    [Header("Lighting Variable")]
    [SerializeField] float detectionRadiusLight = 20f;
    [SerializeField] LayerMask lightLayer;  // Layer of the gameobject light
    [SerializeField] LayerMask wallFloorLayer;   // Layer of the gameobject wall

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player");
        audioSource = GetComponent<AudioSource>();        
        initialPosition = transform.position;
        initialFacingRight = !npcSpriteRenderer.flipX;
        initialFloorLevel = floorLevel;
    }

    protected override void Update()
    {
        base.Update();
       // DetectMovingObjects();
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

    protected override bool IsObjectInFieldOfView(Collider2D obj)
    {
        //Debug.Log("Appel de IsObjectInFieldOfView() dans HumanNPCBehaviour");
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
            UnityEngine.Rendering.Universal.Light2D light = lightCollider.GetComponent<UnityEngine.Rendering.Universal.Light2D>();

            if(light == null || !light.enabled)
                continue;
                        

            if(light.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Global)
            {
                continue;
            }
            else if(light.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Point)
            {
                Vector2[] samplePoints = LightUtility.GetSamplePointsFromObject(objCollider);

                // Check if any parts of the object is hit by light
                foreach(Vector2 point in samplePoints)
                {
                    // Calculate the distance from light to the point position
                    float distance = Vector2.Distance(point, lightCollider.transform.position);
                    if (distance <= light.pointLightOuterRadius)
                    {
                        // Calculate angle between light's forward direction and the point position
                        Vector2 directionLightToPoint = (point - (Vector2) lightCollider.transform.position).normalized;
                        float angle = Vector2.Angle(lightCollider.transform.up, directionLightToPoint);

                        // Check if the point is within the outer spot angle
                        if (angle <= light.pointLightOuterAngle / 2)
                        {
                            //print("HIT by light!!");

                            // Check if walls does not block the light
                            if (!LightUtility.BlockedByWall(lightCollider.transform.position, directionLightToPoint, distance, wallFloorLayer))
                            {
                                return true;
                            }
                        }
                    }
                }
                
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
        isInvestigating = true;
        StopAllCoroutines();
        StartCoroutine(InvestigateAndReturn(objectsound, replaceObject, targetFloor));
    }

    protected IEnumerator InvestigateAndReturn(GameObject objectsound, bool replaceObject, float targetFloor)
    {
        yield return StartCoroutine(InvestigateFallingObject(objectsound, replaceObject, targetFloor));
        yield return StartCoroutine(ReturnToInitialPosition());
    }
    // NPC behaviour for the investigation
    protected IEnumerator InvestigateFallingObject(GameObject objectsound, bool replaceObject, float targetFloor)
    {
        // Take a surprise pause before going on investigation
        audioSource.Play();
        yield return new WaitForSeconds(surpriseWaitTime);

        // Check if the npc need to use the stairs
        if(floorLevel != targetFloor)
        {
            // Find a path using stairs to reach the target floor
            List<StairController> path = FloorNavigation.Instance.FindPathToFloor(this, targetFloor);
            
            foreach(StairController stair in path)
            {
                // NPC must walk to the stair
                Vector2 stairPosition = new Vector2(stair.StartPoint.position.x, transform.position.y);

                // Flip sprite based on direction
                Vector2 npcDirection = (stairPosition - (Vector2)transform.position).normalized;
                // Sprite face the right direction
                npcSpriteRenderer.flipX = npcDirection.x < 0;
                facingRight = !npcSpriteRenderer.flipX;

                // Move to stair
                while (Mathf.Abs(transform.position.x - stairPosition.x) > 0.1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, stairPosition, movementSpeed * Time.deltaTime);
                    yield return null;
                }


                // When the npc reached the stairs
                // Determine if we need to go up or down
                StairDirection stairDirection = (targetFloor > floorLevel) ? StairDirection.Upward : StairDirection.Downward;
                stair.ClimbStair(this.gameObject, stairDirection);
                
                // Wait for stair climbing to finish
                yield return new WaitForSeconds(1f);

                //Update our current floor
                if(stairDirection == StairDirection.Upward)
                {
                    floorLevel = stair.UpperFloor.FloorLevel;
                }
                else if(stairDirection == StairDirection.Downward)
                {
                    floorLevel = stair.BottomFloor.FloorLevel;
                }
            }
        }

        // The NPC is now on the same level has the object

        // Go towards the sound
        Vector2 destination = new Vector2(objectsound.transform.position.x, transform.position.y);
        // Sprite face the right direction
        Vector2 direction = (destination - (Vector2)transform.position).normalized;
        npcSpriteRenderer.flipX = direction.x < 0;
        facingRight = !npcSpriteRenderer.flipX;

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

        // Check if the npc need to use the stairs
        if (floorLevel != initialFloorLevel)
        {
            // Find a path using stairs to reach the target floor
            List<StairController> path = FloorNavigation.Instance.FindPathToFloor(this, initialFloorLevel);
            
            foreach (StairController stair in path)
            {
                // NPC must walk to the stair
                Vector2 stairPosition = new Vector2(stair.StartPoint.position.x, transform.position.y);

                // Flip sprite based on direction
                Vector2 npcDirection = (stairPosition - (Vector2)transform.position).normalized;
                // Sprite face the right direction
                npcSpriteRenderer.flipX = npcDirection.x < 0;
                facingRight = !npcSpriteRenderer.flipX;

                // Move to stair
                while (Mathf.Abs(transform.position.x - stairPosition.x) > 0.1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, stairPosition, movementSpeed * Time.deltaTime);
                    yield return null;
                }


                // When the npc reached the stairs
                // Determine if we need to go up or down
                StairDirection stairDirection = (initialFloorLevel > floorLevel) ? StairDirection.Upward : StairDirection.Downward;
                stair.ClimbStair(this.gameObject, stairDirection);

                // Wait for stair climbing to finish
                yield return new WaitForSeconds(1f);

                //Update our current floor
                if (stairDirection == StairDirection.Upward)
                {
                    floorLevel = stair.UpperFloor.FloorLevel;
                }
                else if (stairDirection == StairDirection.Downward)
                {
                    floorLevel = stair.BottomFloor.FloorLevel;
                }
            }
        }


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

    public void UpdateFloorLevel(float floor)
    {
        floorLevel = floor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadiusLight);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
