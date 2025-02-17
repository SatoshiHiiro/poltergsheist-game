using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Float Animation")]
    [SerializeField] public float floatDuration;        //Duration of going from a to b in seconds
    [SerializeField] public float floatMin;             //Minimum y
    [SerializeField] public float floatMax;             //Maximum y
    float startTime;
    bool isGoingUp;

    [Header("Rotation Animation")]
    [SerializeField] public float rotationSpeed;        //Multiplier for the number of degrees to turn each frame
    Quaternion direction = new Quaternion(0, 0, 0, 1);

    //Shortcut
    Rigidbody2D rb2d;

    void Start()
    {
        rb2d = transform.parent.GetComponent<Rigidbody2D>();
        startTime = Time.time;
        isGoingUp = true;
    }

    void Update()
    {
        //To alternate between floatMin and floatMax
        if (Mathf.FloorToInt(Time.time - startTime) == floatDuration)
        {
            startTime = Time.time;
            isGoingUp = !isGoingUp;
        }

        FloatingSprite(isGoingUp);

        //To change target rotation depending on velocity
        if (rb2d.linearVelocityX > 0)
            direction = new Quaternion(0, 0, 0, 1);
        else if (rb2d.linearVelocityX < 0)
            direction = new Quaternion(0, 1, 0, 0);

        RotateSprite(direction);
    }

    //The actual rotation on the y axis
    void RotateSprite(Quaternion _direction)
    {
        var step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.localRotation, _direction, step);
    }

    //The actual translation on y axis
    void FloatingSprite(bool _isGoingUp)
    {
        float time = (Time.time - startTime) / floatDuration;

        if (isGoingUp)
            transform.localPosition = new Vector3(0, Mathf.SmoothStep(floatMin, floatMax, time), 0);
        else
            transform.localPosition = new Vector3(0, Mathf.SmoothStep(floatMax, floatMin, time), 0);
    }
}
