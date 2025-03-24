using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class SpriteManager : MonoBehaviour
{
    protected Vector2 lastPos;
    protected Vector2 input;
    bool hasPlayerCon;
    bool hasPosCon;

    //Shortcut
    protected PlayerController player;
    protected PossessionController posCon;

    protected virtual void Awake()
    {
        hasPlayerCon = false;
        hasPosCon = false;
        if (this.transform.parent.TryGetComponent<PlayerController>(out player)) { hasPlayerCon = true; }
        else if (this.transform.parent.parent.TryGetComponent<PlayerController>(out player)) { hasPlayerCon = true; }
        else if (this.transform.parent.TryGetComponent<PossessionController>(out posCon)) { hasPosCon = true; }
        else if (this.transform.parent.parent.TryGetComponent<PossessionController>(out posCon)) { hasPosCon = true; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float posDiff = lastPos.x - transform.position.x;

        if (hasPlayerCon)
            input = player.move.ReadValue<Vector2>();
        else if (hasPosCon)
            input = posCon.move.ReadValue<Vector2>();
        else
            input = new Vector2(-posDiff, 0);
    }

    private void LateUpdate()
    {
        lastPos = transform.position;
    }

    protected void CollisionConditionsForManager(string param, MovementController controller, Animator anim)
    {
        if (param == controller.jumpParam)
        {
            anim.SetTrigger("stretch");
        }
        else if (param == controller.landParam)
        {
            float temp = (controller.lastPosY - controller.transform.position.y) / 10f;
            anim.SetFloat("velocityY", temp);
            anim.SetTrigger("squashLand");
        }
        else if (param == controller.bonkRightParam)
        {
            float temp = controller.lastVelocityX / controller.maxSpeed;
            anim.SetFloat("velocityX", temp);
            anim.SetTrigger("squashRight");
        }
        else if (param == controller.bonkLeftParam)
        {
            float temp = controller.lastVelocityX / controller.maxSpeed;
            anim.SetFloat("velocityX", temp);
            anim.SetTrigger("squashLeft");
        }
        else if (param == controller.possessParam)
        {

        }
        else if (param == controller.depossessParam)
        {
            player.lastPossession.GetComponent<PossessionController>().onDepossess -= FindFirstObjectByType<PlayerManager>().EventParameter;
        }
    }
}
