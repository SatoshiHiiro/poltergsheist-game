using UnityEngine;

public class HumanNPCBehaviour : BasicNPCBehaviour
{
    // Variable manage suspicion of the NPC
    [SerializeField] protected float minSuspiciousRotation; // Minimum rotation change in degrees to trigger suspicion
    [SerializeField] protected float minSuspiciousPosition; // Minimum position change to trigger suspicion
    protected override void Update()
    {
        // À CHANGER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! MIRROIR!!
        base.Update();
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
}
