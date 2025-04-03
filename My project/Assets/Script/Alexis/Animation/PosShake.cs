using UnityEngine;
using System.Collections;

public class PosShake : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetFloat("jumpSpeed") == 0)
        {
            iTween.PunchPosition(animator.transform.gameObject, iTween.Hash("name", "failStretch", "y", .2f, "islocal", true, "time", 1f / stateInfo.speed , "delay", .1f));
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entered");
        iTween.Stop(animator.transform.gameObject);
        //animator.transform.position = new Vector3(0, -.5f, 0);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        // Implement code that processes and affects root motion
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
