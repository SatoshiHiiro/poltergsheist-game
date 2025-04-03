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
    [SerializeField] private float minimumBlockWidth;
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
        // Get the collider bounds
        Bounds trapBounds = trapCollider.bounds;

        // Get trap width and height
        float trapWidth = trapBounds.size.x;
        float detectionHeight = trapBounds.size.y;

        // Get object colliders in front of the stair
        Collider2D[] colliders = Physics2D.OverlapBoxAll(trapBounds.center,
                                                        new Vector2(trapWidth, detectionHeight),
                                                        0f, possessedObjectLayer
                                                        );
        foreach (Collider2D collider in colliders)
        {
            float objectWidth = collider.bounds.size.x;
            float objectHeight = collider.bounds.size.y;
            print("Object width " + objectWidth);
            print("object height " + objectHeight);
            if(objectWidth >= minimumBlockWidth || objectHeight >= minimumBlockHeight)
            {
                return true;
            }
        }
        return false;
       
    }
}
