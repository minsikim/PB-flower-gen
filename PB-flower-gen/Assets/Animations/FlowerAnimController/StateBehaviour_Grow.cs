﻿using System;
using UnityEngine;

public class StateBehaviour_Grow : StateMachineBehaviour
{
    private float lastUpdateTime;
    private float updateTime = .1f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastUpdateTime = Time.time;
        animator.GetComponent<Plant>().OnGrowStart();
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
                currentFlower.Grow(progression);
            }
        }
    }
}
