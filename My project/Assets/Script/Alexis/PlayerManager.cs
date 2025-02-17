using UnityEngine;

public class PlayerManager : SpriteManager
{
    [Header("Float Animation")]
    [SerializeField] public float floatDuration;        //Duration of going from a to b in seconds
    [SerializeField] public float floatMin;             //Minimum y
    [SerializeField] public float floatMax;             //Maximum y
    float startTime;
    bool isGoingUp;

    protected override void Start()
    {
        base.Start();
        startTime = Time.time;
        isGoingUp = true;
    }

    protected override void Update()
    {
        base.Update();

        //To alternate between floatMin and floatMax
        if (Mathf.FloorToInt(Time.time - startTime) == floatDuration)
        {
            startTime = Time.time;
            isGoingUp = !isGoingUp;
        }

        FloatingSprite(isGoingUp);
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
