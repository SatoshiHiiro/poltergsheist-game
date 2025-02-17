using UnityEngine;

public abstract class SpriteManager : MonoBehaviour
{
    [Header("Rotation Animation")]
    [SerializeField] public float rotationSpeed;        //Multiplier for the number of degrees to turn each frame
    Quaternion direction = new Quaternion(0, 0, 0, 1);
    float lastPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        lastPos = transform.position.x;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float posDiff = lastPos - transform.position.x;

        //To change target rotation depending on velocity
        if (posDiff > 0)
            direction = new Quaternion(0, 1, 0, 0);
        else if (posDiff < 0)
            direction = new Quaternion(0, 0, 0, 1);

        RotateSprite(direction);
        lastPos = transform.position.x;
    }

    //The actual rotation on the y axis
    protected void RotateSprite(Quaternion _direction)
    {
        var step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.localRotation, _direction, step);
    }
}
