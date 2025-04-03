using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PlayerAnimationManager : RotationManager
{
    [Header("Float Animation")]
    [SerializeField] float floatDuration;        //Duration of going from a to b in seconds
    [SerializeField] float floatMin;             //Minimum y
    [SerializeField] float floatMax;             //Maximum y
    //[SerializeField] iTween.EaseType easeType;
    private Vector3[] animationTargets;
    bool isGoingUp;

    [Header("Collision Animation")]
    [SerializeField] Vector3 scalePunch;
    private Vector3 scaleIni;
    [SerializeField] float collisionDuration;

    //General animation variables
    Animator height;

    protected override void Start()
    {
        base.Start();
        scaleIni = this.transform.lossyScale;
        animationTargets = new Vector3[]{ new Vector3(0, transform.localPosition.y + floatMin, 0), new Vector3(0, transform.localPosition.y + floatMax, 0) };
        //FloatingSpriteAnimation();
    }

    void OnEnable()
    {
        height = player.transform.GetChild(0).GetComponent<Animator>();
        player.onJump += EventParameter;
        player.onLand += EventParameter;
        player.onBonkR += EventParameter;
        player.onBonkL += EventParameter;
    }

    void OnDisable()
    {
        player.onJump -= EventParameter;
        player.onLand -= EventParameter;
        player.onBonkR -= EventParameter;
        player.onBonkL -= EventParameter;
    }

    protected override void Update()
    {
        base.Update();
    }

    /*void FloatingSpriteAnimation()
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
    }*/

    public void EventParameter(string param)
    {
        CollisionConditionsForManager(param, player, height);
    }

    public void VariablesToDefaultValues()
    {
        this.transform.localScale = scaleIni;
    }
}
