using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerManager : SpriteManager
{
    [Header("Float Animation")]
    [SerializeField] float floatDuration;        //Duration of going from a to b in seconds
    [SerializeField] float floatMin;             //Minimum y
    [SerializeField] float floatMax;             //Maximum y
    [SerializeField] iTween.EaseType easeType;
    private Vector3[] animationTargets;
    bool isGoingUp;

    [Header("Collision Animation")]
    [SerializeField] Vector3 scalePunch;
    private Vector3 scaleIni;
    [SerializeField] float collisionDuration;
    float iniPos;
    bool isFinishedJumping;
    bool isLanding;
    bool isFirsTime;

    //General animation variables
    float startTime;
    PlayerController player;
    Animator heigth;

    AnimationClip squashLand;

    protected override void Start()
    {
        base.Start();
        scaleIni = this.transform.lossyScale;
        startTime = Time.time;
        isFinishedJumping = true;
        isLanding = false;
        isGoingUp = true;
        isFirsTime = true;
        animationTargets = new Vector3[]{ new Vector3(0, transform.localPosition.y + floatMin, 0), new Vector3(0, transform.localPosition.y + floatMax, 0) };
        FloatingSpriteAnimation();
    }

    void OnEnable()
    {
        player = FindFirstObjectByType<PlayerController>();
        heigth = player.transform.GetChild(0).GetComponent<Animator>();
        player.onJump += CollisionConditionsForManager;
        player.onLand += CollisionConditionsForManager;
        player.onBonkR += CollisionConditionsForManager;
        player.onBonkL += CollisionConditionsForManager;
    }

    void OnDisable()
    {
        player.onJump -= CollisionConditionsForManager;
        player.onLand -= CollisionConditionsForManager;
        player.onBonkR -= CollisionConditionsForManager;
        player.onBonkL -= CollisionConditionsForManager;
    }

    protected override void Update()
    {
        base.Update();


        /*if (true)
        {
            //To alternate between floatMin and floatMax
            if ((Time.time - startTime) >= floatDuration)
            {
                startTime = Time.time;
                isGoingUp = !isGoingUp;
            }

            FloatingSprite(isGoingUp);
        }*/
    }

    void FloatingSpriteAnimation()
    {
        if (isGoingUp)
        {
            isGoingUp = false;
            iTween.MoveTo(this.gameObject, iTween.Hash("name", "FloatAnimation" , "easetype", easeType, "time", floatDuration, "islocal", true, "position", animationTargets[1], "oncomplete", "FloatingSpriteAnimation"));
        }
        else
        {
            isGoingUp = true;
            iTween.MoveTo(this.gameObject, iTween.Hash("name", "FloatAnimation", "easetype", easeType, "time", floatDuration, "islocal", true, "position", animationTargets[0] , "oncomplete", "FloatingSpriteAnimation"));
        }
    }

    void CollisionConditionsForManager(string param)
    {
        if (param == player.jumpParam)
        {
            heigth.SetTrigger("stretch");
        }
        else if (param == player.landParam)
        {
            float temp = (player.lastPosY - player.transform.position.y) / 10f;
            heigth.SetFloat("velocityY", temp);
            heigth.SetTrigger("squashLand");
        }
        else if (param == player.bonkRightParam)
        {
            float temp = player.lastVelocityX / player.maxSpeed;
            heigth.SetFloat("velocityX", temp);
            heigth.SetTrigger("squashRight");
        }
        else if (param == player.bonkLeftParam)
        {
            float temp = player.lastVelocityX / player.maxSpeed;
            heigth.SetFloat("velocityX", temp);
            heigth.SetTrigger("squashLeft");
        }
    }

    public void VariablesToDefaultValues()
    {
        this.transform.localScale = scaleIni;
    }
}
