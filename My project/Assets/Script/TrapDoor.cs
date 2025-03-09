using System.Collections;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    private JointAngleLimits2D openDoorLimits;
    private JointAngleLimits2D closeDoorLimits;
    private HingeJoint2D hingeJoint2D;
    private void Awake()
    {
        hingeJoint2D = GetComponent<HingeJoint2D>();
        openDoorLimits = hingeJoint2D.limits;
        closeDoorLimits = new JointAngleLimits2D { min = 0f, max = 0f };
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
}
