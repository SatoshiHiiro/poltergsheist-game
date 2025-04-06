using UnityEngine;

public class BallAnimationManager : MonoBehaviour
{
    public float rotationSpeed;

    //Associated gameObjects
    PossessionController posCon;
    Rigidbody2D rBody;

    private void OnEnable()
    {
        if (this.transform.parent.TryGetComponent<PossessionController>(out posCon)) { }
        else if (this.transform.parent.parent.TryGetComponent<PossessionController>(out posCon)) { }
        else { Debug.Log("No PossessionController for the ball"); }

        if (posCon.TryGetComponent<Rigidbody2D>(out rBody)) { }
        else { Debug.Log("No Rigidbody2D on the gameObject with the PossessionController for the ball"); }
    }

    private void FixedUpdate()
    {
        if (rBody != null)
        {
            float velocityX = rBody.linearVelocityX;
            if (velocityX > 0)
                transform.Rotate(new Vector3(0, 0, rotationSpeed * -velocityX), Space.Self);
            else if (velocityX < 0)
                transform.Rotate(new Vector3(0, 0, rotationSpeed * -velocityX), Space.Self);
        }
    }
}
