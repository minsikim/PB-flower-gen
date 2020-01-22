using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBehaviour_StateCheck : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FlowerAnimationStates currState = animator.GetComponent<Flower>().GetCurrentState();
        animator.SetTrigger(Enum.GetName(typeof(FlowerAnimationStates), (int)currState));
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
