using System;
using UnityEngine;

public class StateBehaviour_Fall : StateMachineBehaviour
{
    private float lastUpdateTime;
    private float updateTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastUpdateTime = Time.time;
        animator.GetComponent<Plant>().OnFallStart();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lastUpdateTime + 5f > Time.time) return;
        else
        {
            Plant currentFlower = animator.GetComponent<Plant>();
            float progression = currentFlower.GetProgression();
            if (progression >= 1 || progression < 0)
            {
                currentFlower.SwitchToNextState();
                FlowerAnimationState currState = animator.GetComponent<Plant>().GetCurrentState();
                animator.SetTrigger(Enum.GetName(typeof(FlowerAnimationState), (int)currState));
            }
            else
            {
                currentFlower.Fall(progression);
                animator.SetFloat("Progression", progression);
            }

            lastUpdateTime = Time.time;
        }

    }
}
