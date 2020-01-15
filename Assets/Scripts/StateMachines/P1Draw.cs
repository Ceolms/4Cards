using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Draw : CustomStateMachine
{
    Card card;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GameManager.Instance.multiplayer)
        {
            IA.Instance.CheckDeleteCard();
        }
        TextViewer.Instance.SetText("Player 1 Draw");
        Discard.Instance.ShowParticles(true);
        Deck.Instance.ShowParticles(true);
        GameManager.Instance.state = this;
    }

    private void Draw()
    {
        Card c = null;
        if (card.position == Card.Position.Deck)
        {
            c = Deck.Instance.Draw();

            if (GameManager.Instance.multiplayer)
            {
                // DrawCard(Card.Position pos, Card.Owner player)
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("DrawCard", PhotonTargets.Others, c.position, MultiPlayerController.LocalPlayerInstance.playerID);
            }
            c.MoveTo(Card.Position.Player1Choice);
            c.SetHidden(false);
            Deck.Instance.ShowParticles(false);
            Discard.Instance.ShowParticles(false);
            GameManager.Instance.ChangePhase();
        }
        else if (card.position == Card.Position.Discard)
        {
            c = Discard.Instance.Draw();

            if (GameManager.Instance.multiplayer)
            {
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("DrawCard", PhotonTargets.Others, c.position, MultiPlayerController.LocalPlayerInstance.playerID);
            }
            c.MoveTo(Card.Position.Player1Choice);
            c.SetHidden(false);
            Deck.Instance.ShowParticles(false);
            Discard.Instance.ShowParticles(false);
            if (!GameManager.Instance.multiplayer)
            {
                IA.Instance.opponentKnownCards.Add(c);
            }
            GameManager.Instance.ChangePhase();
        }
    }

    public override void Execute(Card c)
    {
        if (c != null)
        {
            card = c;
            if (GameManager.Instance.powerChar == 'N')
            {
                Draw();
            }
        }

    }

    public override void ChangePhase()
    {
        GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
    }
}
