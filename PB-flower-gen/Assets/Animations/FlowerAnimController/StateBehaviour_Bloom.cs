﻿using System;
using UnityEngine;

public class StateBehaviour_Bloom : StateMachineBehaviour
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
                FlowerAnimationState currState = animator.GetComponent<Plant>().GetCurrentState();
                animator.SetTrigger(Enum.GetName(typeof(FlowerAnimationState), (int)currState));
            }
            else
            {
                currentFlower.Bloom(progression);
                animator.SetFloat("Progression", progression);
            }
        }
    }
}
