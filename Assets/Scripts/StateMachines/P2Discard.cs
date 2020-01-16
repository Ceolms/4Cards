using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Discard : CustomStateMachine
{
    Card card;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GameManager.Instance.multiplayer)
        {
            TextViewer.Instance.SetText("IA Discard");
        }
        else
        {
            TextViewer.Instance.SetText(GameManager.Instance.namePlayer2 + " Discard");
        }
        Discard.Instance.ShowParticles(false);
        Deck.Instance.ShowParticles(false);
        GameManager.Instance.state = this;

        if (!GameManager.Instance.multiplayer)
        {
            IA.Instance.DiscardPhase();
        }
    }

    public void DiscardCard()
    {
        if (card.owner == Card.Owner.Player2 && card.position != Card.Position.Player2Choice) // If the player want to discard one of his cards
        {
            Card.Position p = card.position;
            if (GameManager.Instance.multiplayer)
            {
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("DiscardCard", PhotonTargets.Others, p, MultiPlayerController.LocalPlayerInstance.playerID);
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("MoveCardToHand", PhotonTargets.Others, p, MultiPlayerController.LocalPlayerInstance.playerID);
            }

            card.MoveTo(Card.Position.Discard); // move old card to discard
            card.SetHidden(false);
            Card c = GameManager.Instance.FindByPosition(Card.Position.Player2Choice); // take the new one to slot

            c.SetHidden(true);
            c.MoveTo(p);

            if (card.value == "Q") GameManager.Instance.UsePower('Q');
            if (card.value == "J") GameManager.Instance.UsePower('J');
            GameManager.Instance.ChangePhaseLong();
        }
        else if (card.owner == Card.Owner.Player2 && card.position == Card.Position.Player2Choice) // else if it's the one he drawn
        {
            if (GameManager.Instance.multiplayer)
            {
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("DiscardCard", PhotonTargets.Others, card.position, MultiPlayerController.LocalPlayerInstance.playerID);
            }
            card.MoveTo(Card.Position.Discard);
            card.SetHidden(false);
            if (card.value == "Q") GameManager.Instance.UsePower('Q');
            if (card.value == "J") GameManager.Instance.UsePower('J');
            { IA.Instance.opponentKnownCards.Remove(card); }
            GameManager.Instance.ChangePhaseLong();
        }
    }


    public override void Execute(Card c)
    {
        if (GameManager.Instance.powerChar == 'N' && GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player2)
        {
            card = c;
            DiscardCard();
        }
    }

    public override void ChangePhase()
    {
        if (GameManager.Instance.endRoundPlayer == Card.Owner.Player1) GameManager.Instance.gameLogic.SetTrigger("EndRound");
        else GameManager.Instance.gameLogic.SetTrigger("DiscardComplete");
    }

    public override bool CanDeleteCard()
    {
        return true;
    }
}
