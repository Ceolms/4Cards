using cakeslice;
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

        GameObject button = GameObject.Find("ActionButton");
        button.GetComponentInChildren<Text>().text = "End Round";
        //Debug.Log("comp: " + GameObject.Find("OutlineBox").GetComponent<cakeslice.Outline>());
        GameObject.Find("OutlineBox").GetComponent<cakeslice.Outline>().enabled = false;

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

