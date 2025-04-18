using UnityEngine;
using System.Collections;

public class FaceAnimationEventBehavior : MonoBehaviour
{
    RotationManager playerManager;
    Animator anim;
    Coroutine turnBlink;
    Coroutine idleBlink;
    int iteration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = this.GetComponentInParent<RotationManager>();
        anim = this.GetComponent<Animator>();
        iteration = -3;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager.inRotation) 
        {
            if (turnBlink == null) 
            {
                if (idleBlink != null)
                {
                    StopCoroutine(idleBlink);
                    idleBlink = null;
                }
                turnBlink = StartCoroutine(TurnBlink()); 
            }
        }
        else
        {
            if (turnBlink != null) 
            { 
                StopCoroutine(turnBlink);
                anim.SetBool("IsBlinking", false);
                turnBlink = null;
            }
        }
        if (idleBlink == null && turnBlink == null) { idleBlink = StartCoroutine(IdleBlink()); }
    }

    IEnumerator IdleBlink()
    {
        float time = 0;

        if (Random.Range(1 + iteration, 10) > 5)
        {
            Debug.Log("Blink");
            time = Random.Range(.1f, .3f);
            iteration = -3;
            anim.SetBool("IsBlinking", true);
            yield return new WaitForSecondsRealtime(time);
            anim.SetBool("IsBlinking", false);
        }
        else
        {
            Debug.Log("Fail");
            iteration++;
        }

        yield return new WaitForSecondsRealtime(5f - time);
        idleBlink = null;
    }

    IEnumerator TurnBlink()
    {
        iteration = -3;
        anim.SetBool("IsBlinking", true);
        yield return new WaitForSecondsRealtime(.5f);
        anim.SetBool("IsBlinking", false);
        turnBlink = null;
    }
}
