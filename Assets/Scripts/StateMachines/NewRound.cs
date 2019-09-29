
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewRound : CustomStateMachine
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("New Round StateEnter");
        TextViewer.Instance.SetText("Distribute Phase");
        GameManager.Instance.state = this;
        GameManager.Instance.gameLogic.SetBool("LookCompleteP1", false);
        GameManager.Instance.gameLogic.SetBool("LookCompleteP2", false);
        GameObject button = GameObject.Find("ActionButton");
        button.GetComponentInChildren<Text>().text = "End Round";
        GameManager.Instance.endRoundPlayer = "null";

        Deck.Instance.InitRound();
    }

    public override void Execute(Card c)
    {
     // nothing to do here
    }

    public override void ChangePhase()
    {
        // nothing to do here
    }
}

