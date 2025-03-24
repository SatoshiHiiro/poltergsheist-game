using UnityEngine;

public class PossessableManager : SpriteManager
{
    Animator animator;

    protected override void Awake()
    {
        base.Awake();

    }

    private void OnEnable()
    {
        posCon.onJump += EventParameter;
        posCon.onLand += EventParameter;
        posCon.onBonkL += EventParameter;
        posCon.onBonkR += EventParameter;
        posCon.onPossess += EventParameter;
        posCon.onDepossess += FindFirstObjectByType<PlayerManager>().EventParameter;
    }

    private void OnDisable()
    {
        posCon.onJump -= EventParameter;
        posCon.onLand -= EventParameter;
        posCon.onBonkL -= EventParameter;
        posCon.onBonkR -= EventParameter;
        posCon.onPossess -= EventParameter;
        posCon.onDepossess -= FindFirstObjectByType<PlayerManager>().EventParameter;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        animator = this.transform.parent.GetComponent<Animator>();
        animator.WriteDefaultValues();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    void EventParameter(string param)
    {
        CollisionConditionsForManager(param, posCon, animator);
    }
}
