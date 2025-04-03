using UnityEngine;
using System.Collections;

public abstract class SpriteManager : MonoBehaviour
{
    protected Vector2 lastPos;
    protected Vector2 input;
    protected bool goesLeft;
    protected bool goesRight;
    bool hasPlayerCon;
    bool hasPosCon;


    //Shortcut
    protected PlayerController player;
    protected PossessionController posCon;

    protected virtual void Awake()
    {
        goesLeft = false;
        goesRight = false;
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
        //lastPos = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float posDiff = lastPos.x - transform.position.x;

        if (input.x < 0)
        {
            goesRight = false;
            goesLeft = true;
        }
        else if (input.x > 0)
        {
            goesLeft = false;
            goesRight = true;
        }

        if (hasPlayerCon)
            input = player.move.ReadValue<Vector2>();
        else if (hasPosCon)
            input = posCon.move.ReadValue<Vector2>();
        else
            input = new Vector2(-posDiff, 0);
    }

    private void LateUpdate()
    {
        //lastPos = transform.position;
    }

    IEnumerator ResetBoolAnimation(Animator anim, string param)
    {
        yield return new WaitForSecondsRealtime(.05f);
        anim.SetBool(param, false);
    }

    protected void CollisionConditionsForManager(string param, MovementController controller, Animator anim)
    {
        if (param == controller.landParam)
        {
            float temp = (controller.lastPosY - controller.transform.position.y) / 10f;
            anim.SetFloat("velocityY", temp);
            anim.SetTrigger("squashLand");
        }
        else if (param == controller.jumpParam)
        {
            anim.SetFloat("jumpSpeed", controller.jumpSpeed);
            anim.SetTrigger("stretch");
        }
        else if (param == controller.bonkRightParam)
        {
            float temp = controller.lastVelocityX / controller.maxSpeed;
            anim.SetFloat("velocityX", temp);
            anim.SetTrigger("squashRight");
            //anim.SetBool(temp, true);
            //StartCoroutine(ResetBoolAnimation(anim, temp));
        }
        else if (param == controller.bonkLeftParam)
        {
            float temp = controller.lastVelocityX / controller.maxSpeed;
            anim.SetTrigger("squashLeft");
            
            //anim.SetFloat("velocityX", temp);
            //anim.SetBool(temp, true);
            //StartCoroutine(ResetBoolAnimation(anim, temp));
        }
        else if (param == controller.cantWalkLeftParam)
        {
            Debug.Log("Left");
            anim.SetFloat("velocityX", -1f);
            anim.SetTrigger("moveFail");
        }
        else if (param == controller.cantWalkRightParam)
        {
            Debug.Log("Right");
            anim.SetFloat("velocityX", 1f);
            anim.SetTrigger("moveFail");
        }
        else if (param == controller.possessParam)
        {
            
        }
        else if (param == controller.depossessParam)
        {
            //player.lastPossession.GetComponent<PossessionController>().onDepossess -= FindFirstObjectByType<PlayerManager>().EventParameter;
        }
    }
}
