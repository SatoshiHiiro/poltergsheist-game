using System.Collections;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    private JointAngleLimits2D openDoorLimits;
    private JointAngleLimits2D closeDoorLimits;
    private HingeJoint2D hingeJoint2D;
    private BoxCollider2D trapCollider;
    [SerializeField] private LayerMask possessedObjectLayer;
    [SerializeField] private float minimumBlockHeight;
    [SerializeField] private float blockingThreshold;
    private void Awake()
    {
        hingeJoint2D = GetComponent<HingeJoint2D>();
        openDoorLimits = hingeJoint2D.limits;
        closeDoorLimits = new JointAngleLimits2D { min = 0f, max = 0f };
        trapCollider = GetComponent<BoxCollider2D>();
        CloseDoor();
    }
    public void OpenDoor()
    {
        hingeJoint2D.limits = openDoorLimits;
        StartCoroutine(WaitBeforeClosing());
    }

    private void CloseDoor()
    {
        hingeJoint2D.limits = closeDoorLimits;
    }

    private IEnumerator WaitBeforeClosing()
    {
        yield return new WaitForSeconds(3f);
        CloseDoor();
    }

    private void Update()
    {
        IsTrapDoorBlocked();
    }

    public bool IsTrapDoorBlocked()
    {
        // Get the sprite bounds
        Bounds trapBounds = trapCollider.bounds;

        // Get stair width and height
        float trapWidth = trapBounds.size.x;
        float detectionHeight = trapBounds.size.y;

        // Get object colliders in front of the stair
        Collider2D[] colliders = Physics2D.OverlapBoxAll(trapBounds.center,
                                                        new Vector2(trapWidth, detectionHeight),
                                                        0f, possessedObjectLayer
                                                        );

        print(colliders.Length);
        return true;
        //float blockedWidth = 0f;

        //foreach (Collider2D collider in colliders)
        //{
        //    // Skip if the object is not tall enough
        //    if (collider.bounds.size.y < minimumBlockHeight)
        //        continue;

        //    // Calculate how much width of the room is the object taking
        //    float objectWidth = Mathf.Min(collider.bounds.max.x, trapBounds.max.x)
        //                        - Mathf.Max(collider.bounds.min.x, trapBounds.min.x);
        //    if (objectWidth > 0)
        //    {
        //        blockedWidth += objectWidth;
        //    }
        //}

        //// Calculate what percentage of the entrance is blocked
        //float blockPercentage = blockedWidth / stairWidth;
        //return blockPercentage >= blockingThreshold;
    }
}
