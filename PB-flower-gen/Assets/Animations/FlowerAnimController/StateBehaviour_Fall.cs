using System;
using UnityEngine;

public class StateBehaviour_Fall : StateMachineBehaviour
{
    private float lastUpdateTime;
    private float updateTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastUpdateTime = Time.time;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lastUpdateTime + 5f > Time.time) return;
        else
        {
            Debug.Log(lastUpdateTime + "Fall Update" + Time.time);

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
                currentFlower.Fall(progression);
                animator.SetFloat("Progression", progression);
            }

            lastUpdateTime = Time.time;
        }

    }
}
