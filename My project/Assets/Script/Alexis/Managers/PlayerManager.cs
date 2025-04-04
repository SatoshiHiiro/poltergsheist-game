using UnityEngine;

public class PlayerManager : RotationManager
{
    //General animation variables
    Animator baseHeight;
    private Vector3 scaleIni;

    protected override void Start()
    {
        base.Start();
        scaleIni = this.transform.lossyScale;
    }

    void OnEnable()
    {
        baseHeight = player.transform.GetChild(0).GetComponent<Animator>();
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
        baseHeight.SetBool("inContact", player.isInContact);
    }

    public void EventParameter(string param)
    {
        CollisionConditionsForManager(param, player, baseHeight);
    }

    public void VariablesToDefaultValues()
    {
        this.transform.localScale = scaleIni;
    }
}
