using System;
using UnityEngine;

public class StateBehaviour_Sprout : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Plant currentFlower = animator.GetComponent<Plant>();
        float progression = currentFlower.GetProgression();
        if (progression >= 1 || progression < 0)
        {
            currentFlower.SwitchToNextState();
            FlowerAnimationStates currState = animator.GetComponent<Plant>().GetCurrentState();
            animator.SetTrigger(Enum.GetName(typeof(FlowerAnimationStates), (int)currState));
        }
        else
        {
            currentFlower.Sprout(progression);
            animator.SetFloat("Progression", progression);
        }

    }
}
