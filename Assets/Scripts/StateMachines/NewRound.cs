
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
        GameManager.Instance.cardsJ1 = new List<Card>();
        GameManager.Instance.cardsJ2 = new List<Card>();
        Discard.Instance.stack = new List<GameObject>();
        TextViewer.Instance.SetText("Distribute Phase");
        GameManager.Instance.state = this;
        GameManager.Instance.gameLogic.SetBool("LookCompleteP1", false);
        GameManager.Instance.gameLogic.SetBool("LookCompleteP2", false);

        TextViewer.Instance.SetEndTurn();
        GameManager.Instance.endRoundPlayer = Card.Owner.Deck;

        if (GameManager.Instance.lastWinner == -1)
        {
            int i = Random.Range(0, 2);
            GameManager.Instance.gameLogic.SetInteger("FirstToPlay", i);
        }
        else GameManager.Instance.gameLogic.SetInteger("FirstToPlay", GameManager.Instance.lastWinner);
        GameManager.Instance.gameBegin = true;
    }


    public override void Execute(Card c)
    {
        GameManager.Instance.gameBegin = false;
        Deck.Instance.InitRound();
    }

    public override void ChangePhase()
    {
        // nothing to do here
    }
}

