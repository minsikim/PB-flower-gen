using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBehaviour_StateCheck : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FlowerAnimationState currState = animator.GetComponent<Plant>().GetCurrentState();
        animator.SetTrigger(Enum.GetName(typeof(FlowerAnimationState), (int)currState));
        Debug.Log(Enum.GetName(typeof(FlowerAnimationState), (int)currState));
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
