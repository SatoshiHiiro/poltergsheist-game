using UnityEngine;

public class PlayerManager : SpriteManager
{
    [Header("Float Animation")]
    [SerializeField] float floatDuration;        //Duration of going from a to b in seconds
    [SerializeField] float floatMin;             //Minimum y
    [SerializeField] float floatMax;             //Maximum y
    bool isGoingUp;

    [Header("Fall Animation")]
    [SerializeField] float recoveryDuration;
    float iniPos;
    bool isFinishedJumping;
    bool isLanding;
    bool isFirsTime;

    //General animation variables
    float startTime;
    PlayerController player;

    protected override void Start()
    {
        base.Start();
        startTime = Time.time;
        isFinishedJumping = true;
        isLanding = false;
        isGoingUp = true;
        isFirsTime = true;
    }

    void OnEnable()
    {
        player = FindFirstObjectByType<PlayerController>();
        //player.onJump += CollisionConditionsForManager;
        //player.onLand += CollisionConditionsForManager;
    }

    void OnDisable()
    {
        //player.onJump -= CollisionConditionsForManager;
    }

    protected override void Update()
    {
        base.Update();
        float posDiff = lastPos.y - transform.position.y;
        if (/*isFinishedJumping*/ true)
        {
            //To alternate between floatMin and floatMax
            if ((Time.time - startTime) >= floatDuration)
            {
                startTime = Time.time;
                isGoingUp = !isGoingUp;
            }

            FloatingSprite(isGoingUp);
        }
        /*else
        {
            if (isFirsTime)
            {
                startTime = Time.time;
                iniPos = transform.localPosition.y;
                isFirsTime = false;
            }
            if ((Time.time - startTime) >= recoveryDuration)
            {
                isFinishedJumping = true;
                isFirsTime = true;
            }

            if (player.GetComponent<Rigidbody2D>().linearVelocityY >= 0 && !player.isInContact)
            {
                RecoverySprite(floatMax);
            }
            else
            {
                RecoverySprite(floatMin);
            }
        }*/
    }

    //The actual translation on y axis
    void FloatingSprite(bool _isGoingUp)
    {
        float time = (Time.time - startTime) / floatDuration;
        if (_isGoingUp)
            transform.localPosition = new Vector3(0, Mathf.SmoothStep(floatMin, floatMax, time), 0);
        else
            transform.localPosition = new Vector3(0, Mathf.SmoothStep(floatMax, floatMin, time), 0);
    }

    /*bool RecoverySprite(float target)
    {
        bool landingFinished = false;
        float time = (Time.time - startTime) / recoveryDuration;
        transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(0, target, 0), time);

        if (true)
        {
            
        }

        return landingFinished;
        //transform.localPosition = new Vector3(0, Mathf.SmoothStep(iniPos, floatMin, time), 0);
    }

    void CollisionConditionsForManager(string param)
    {
        if (param == player.jumpParam)
        {
            isFinishedJumping = false;
            Debug.Log("Jumped");
        }
        if (param == player.landParam)
        {
            isLanding = true;
            Debug.Log("Landed");
        }
    }*/
}
