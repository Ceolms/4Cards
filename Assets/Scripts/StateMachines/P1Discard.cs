using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Discard : CustomStateMachine
{
    Card card;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GameManager.Instance.multiplayer)
        {
            TextViewer.Instance.SetText("Player 1 Discard");
        }
        else
        {
            TextViewer.Instance.SetText(GameManager.Instance.namePlayer1 + " Discard");
        }
        Discard.Instance.ShowParticles(false);
        Deck.Instance.ShowParticles(false);
        GameManager.Instance.state = this;
    }

    public void DiscardCard()
    {
        if (card.owner == Card.Owner.Player1 && card.position != Card.Position.Player1Choice) // If the player want to discard one of his cards
        {
            Card.Position p = card.position;
            if (GameManager.Instance.multiplayer)
            {
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("DiscardCard", PhotonTargets.Others, p, MultiPlayerController.LocalPlayerInstance.playerID);
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("MoveCardToHand", PhotonTargets.Others, p, MultiPlayerController.LocalPlayerInstance.playerID);
            }

            card.MoveTo(Card.Position.Discard); // move old card to discard

            Card c = GameManager.Instance.FindByPosition(Card.Position.Player1Choice); // take the new one to slot

            c.SetHidden(true);
            c.MoveTo(p);


            if (card.value == "Q") GameManager.Instance.UsePower('Q');
            if (card.value == "J") GameManager.Instance.UsePower('J');
            if (!GameManager.Instance.multiplayer && IA.Instance.opponentKnownCards.Contains(card))
            {
                IA.Instance.opponentKnownCards.Remove(card);
            }
            GameManager.Instance.ChangePhaseLong();
        }
        else if (card.owner == Card.Owner.Player1 && card.position == Card.Position.Player1Choice) // else if it's the one he drawn
        {
            if (GameManager.Instance.multiplayer)
            {
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("DiscardCard", PhotonTargets.Others, card.position, MultiPlayerController.LocalPlayerInstance.playerID);
            }
            card.MoveTo(Card.Position.Discard);

            if (card.value == "Q") GameManager.Instance.UsePower('Q');
            if (card.value == "J") GameManager.Instance.UsePower('J');
            if (!GameManager.Instance.multiplayer && IA.Instance.opponentKnownCards.Contains(card))
            { IA.Instance.opponentKnownCards.Remove(card); }
            GameManager.Instance.ChangePhaseLong();
        }
    }

    public override void Execute(Card c)
    {
        card = c;
        if (GameManager.Instance.powerChar == 'N')
        {
            if (GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player1)
            {
                DiscardCard();
            }
            else if (!GameManager.Instance.multiplayer)
            {
                DiscardCard();
            }
        }
    }

    public override void ChangePhase()
    {
        if (GameManager.Instance.endRoundPlayer == Card.Owner.Player2) GameManager.Instance.gameLogic.SetTrigger("EndRound");
        else GameManager.Instance.gameLogic.SetTrigger("DiscardComplete");
    }

    public override bool CanDeleteCard()
    {
        return true;
    }
}
