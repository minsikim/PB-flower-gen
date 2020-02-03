using System;
using UnityEngine;
using static Plant;

//TODO 이걸로 다른 StateBehaviour 다 통합 할 수도 있을듯. 일단 만들어만 놓음
public class StateBehaviour_Animations : StateMachineBehaviour
{
    private float lastUpdateTime;
    private float updateTime = .1f;
    AnimationMethod MainAnimationMethod;

    private void OnEnable()
    {

    }
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
                FlowerAnimationState currState = animator.GetComponent<Plant>().GetCurrentState();
                animator.SetTrigger(Enum.GetName(typeof(FlowerAnimationState), (int)currState));
            }
            else
            {
                //TODO 이부분고치고
                MainAnimationMethod(progression);
                animator.SetFloat("Progression", progression);
            }
            lastUpdateTime = Time.time;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.GetComponent<Plant>().InitParticles();
    }
}
