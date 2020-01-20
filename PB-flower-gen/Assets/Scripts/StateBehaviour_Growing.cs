using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBehaviour_Growing : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Flower>().OnGrowStart();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Flower>().Grow();
    }
}
