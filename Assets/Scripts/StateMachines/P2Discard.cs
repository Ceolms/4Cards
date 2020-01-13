using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Discard : CustomStateMachine
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TextViewer.Instance.SetText("Player 2 Discard");
        Discard.Instance.ShowParticles(false);
        Deck.Instance.ShowParticles(false);
        GameManager.Instance.state = this;

        if (!GameManager.Instance.multiplayer)
        {
            IA.Instance.DiscardPhase();
        }
        else
        {

        }
    }

    public override void Execute(Card c)
    {
    }

    public override void ChangePhase()
    {
        if (GameManager.Instance.endRoundPlayer == Card.Owner.Player1) GameManager.Instance.gameLogic.SetTrigger("EndRound");
        else GameManager.Instance.gameLogic.SetTrigger("DiscardComplete");
    }
}
