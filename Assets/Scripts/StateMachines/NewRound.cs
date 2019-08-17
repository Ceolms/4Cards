using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRound : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("New Round StateEnter");
       // TextViewer.Instance.SetText("Distribute Phase");
        GameManager.Instance.InitRound();  
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }



}

