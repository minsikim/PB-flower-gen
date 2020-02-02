using System;
using UnityEngine;

public class StateBehaviour_Rebloom : StateMachineBehaviour
{
    private float lastUpdateTime;
    private float updateTime = .1f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastUpdateTime = Time.time;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lastUpdateTime + updateTime > Time.time) return;
        else
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
                currentFlower.Rebloom(progression);
                animator.SetFloat("Progression", progression);
            }
        }
    }
}
