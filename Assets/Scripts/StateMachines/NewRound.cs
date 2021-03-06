﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewRound : CustomStateMachine
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.Instance.cardsJ1 = new List<Card>();
        GameManager.Instance.cardsJ2 = new List<Card>();
        if (!GameManager.Instance.multiplayer) IA.Instance.NewRound();
        Discard.Instance.stack = new List<Card>();
        TextViewer.Instance.SetText("Distribute Phase");
        GameManager.Instance.state = this;
        GameManager.Instance.gameLogic.SetBool("LookCompleteP1", false);
        GameManager.Instance.gameLogic.SetBool("LookCompleteP2", false);

        TextViewer.Instance.SetEndTurn();
        GameManager.Instance.endRoundPlayer = Card.Owner.Deck;

        GameManager.Instance.gameLogic.SetInteger("FirstToPlay", GameManager.Instance.firstToPlay);
        GameManager.Instance.gameBegin = true;
    }

    public override void Execute(Card c)
    {
        if (!(GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player2)) Deck.Instance.InitRound();
    }

    public override void ChangePhase()
    {
        // nothing to do here
    }

    public override bool CanDeleteCard()
    {
        return false;
    }
}

