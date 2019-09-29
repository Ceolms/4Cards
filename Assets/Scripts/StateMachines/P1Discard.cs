using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Discard : CustomStateMachine
{
    Card card;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TextViewer.Instance.SetText("Player 1 Discard");
        Discard.Instance.ShowParticles(false);
        Deck.Instance.ShowParticles(false);
        GameManager.Instance.state = this;
    }

    public void DiscardCard()
    {
        if (card.owner == Card.Owner.Player && card.position != Card.Position.PlayerChoice) // If the player want to discard one of his cards
        {
            Card.Position p = card.position;
            card.MoveTo(Card.Position.Discard); // move old card to discard
            GameManager.Instance.cardsJ1.Remove(card);

            Card c = GameManager.Instance.FindByPosition(Card.Position.PlayerChoice); // take the new one to slot

            c.SetHidden(true);
            c.MoveTo(p);


            if (card.value == "Q") GameManager.Instance.UsePower('Q');
            if (card.value == "J") GameManager.Instance.UsePower('J');
            if (GameManager.Instance.gameType.Equals("IA") && IA.Instance.opponentKnownCards.Contains(card))
            { IA.Instance.opponentKnownCards.Remove(card); }
            GameManager.Instance.ChangePhaseLong();
        }
        else if (card.owner == Card.Owner.Player && card.position == Card.Position.PlayerChoice) // else if it's the one he drawn
        {
            card.MoveTo(Card.Position.Discard);

            if (card.value == "Q") GameManager.Instance.UsePower('Q');
            if (card.value == "J") GameManager.Instance.UsePower('J');
            if (GameManager.Instance.gameType.Equals("IA") && IA.Instance.opponentKnownCards.Contains(card))
            { IA.Instance.opponentKnownCards.Remove(card); }
            GameManager.Instance.ChangePhaseLong();
        }
    }

    public override void Execute(Card c)
    {
        card = c;
        if (GameManager.Instance.powerChar == 'N')
        {
            DiscardCard();
        }

    }

    public override void ChangePhase()
    {
        if (GameManager.Instance.endRoundPlayer.Equals("Opponent")) GameManager.Instance.gameLogic.SetTrigger("EndRound");
        else GameManager.Instance.gameLogic.SetTrigger("DiscardComplete");
    }
}
