﻿using System;
using UnityEngine;

public class StateBehaviour_Grow : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Flower>().OnGrowStart();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Flower currentFlower = animator.GetComponent<Flower>();
        float progression = currentFlower.GetProgression();
        if (progression >= 1 || progression < 0)
        {
            currentFlower.SwitchToNextState();
            FlowerAnimationStates currState = animator.GetComponent<Flower>().GetCurrentState();
            animator.SetTrigger(Enum.GetName(typeof(FlowerAnimationStates), (int)currState));
            Debug.Log("Over : " + progression);
        }
        else
        {
            currentFlower.Grow(progression);
            animator.SetFloat("Progression", progression);
        }
        
    }
}
