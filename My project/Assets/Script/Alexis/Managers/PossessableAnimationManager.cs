using UnityEngine;

public class PossessableAnimationManager : SpriteManager
{
    Animator animPos;
    Animator animPlayer;

    Vector3 defaultYPos;

    protected override void Awake()
    {
        base.Awake();
        animPos = this.transform.parent.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        posCon.onJump += EventParameter;
        posCon.onLand += EventParameter;
        posCon.onBonkL += EventParameter;
        posCon.onBonkR += EventParameter;
        posCon.onShakeL += EventParameter;
        posCon.onShakeR += EventParameter;
        posCon.onPossess += EventParameter;
        posCon.onDepossess += EventParameter;
    }

    private void OnDisable()
    {
        posCon.onJump -= EventParameter;
        posCon.onLand -= EventParameter;
        posCon.onBonkL -= EventParameter;
        posCon.onBonkR -= EventParameter;
        posCon.onShakeL -= EventParameter;
        posCon.onShakeR -= EventParameter;
        posCon.onPossess -= EventParameter;
        posCon.onDepossess -= EventParameter;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        /*animPos = this.transform.parent.GetComponent<Animator>();
        defaultYPos = new Vector3(0, -transform.parent.parent.GetComponent<Collider2D>().bounds.extents.y, 0);

        AnimationCurve curve;

        AnimatorClipInfo[] clipInfo = animPos.GetCurrentAnimatorClipInfo(0);
        AnimationClip clip = clipInfo[0].clip;
        //AnimationClip[] temp = getanimation
        //curve = ;


        //Debug.Log(this.transform.parent.parent.name + ": ");*/
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        animPos.SetBool("canJump", posCon.canJump);
        animPos.SetBool("canMove", posCon.canWalk);
        animPos.SetFloat("animSpeed", (int)posCon.weightOfAnimation / 2f);
    }

    void EventParameter(string param)
    {
        CollisionConditionsForManager(param, posCon, animPos);
        if (param == posCon.possessParam) { }
    }
}
