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

        if (GameManager.Instance.gameType.Equals("IA"))
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
}
