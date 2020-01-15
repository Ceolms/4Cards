using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Draw : CustomStateMachine
{
    Card card;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TextViewer.Instance.SetText("Player 2 Draw");
        GameManager.Instance.state = this;

        if (!GameManager.Instance.multiplayer)
        {
            IA.Instance.CheckDeleteCard();
            IA.Instance.DrawPhase();
        }
    }


    private void Draw()
    {
        Debug.Log("Draw P2");
        Card c = null;
        Debug.Log(card.position);
        if (card.position == Card.Position.Deck)
        {
            Debug.Log("Card is in Deck");

            c = Deck.Instance.Draw();
            MultiPlayerController.LocalPlayerInstance.photonView.RPC("DrawCard", PhotonTargets.Others, c.position, MultiPlayerController.LocalPlayerInstance.playerID);

            c.MoveTo(Card.Position.Player2Choice);
            c.SetHidden(false);
            Deck.Instance.ShowParticles(false);
            Discard.Instance.ShowParticles(false);
            GameManager.Instance.ChangePhase();
        }
        else if (card.position == Card.Position.Discard)
        {
            Debug.Log("Card is in Discard");
            c = Discard.Instance.Draw();

            MultiPlayerController.LocalPlayerInstance.photonView.RPC("DrawCard", PhotonTargets.Others, c.position, MultiPlayerController.LocalPlayerInstance.playerID);

            c.MoveTo(Card.Position.Player2Choice);
            c.SetHidden(false);
            Deck.Instance.ShowParticles(false);
            Discard.Instance.ShowParticles(false);
            GameManager.Instance.ChangePhase();
        }
    }

    public override void Execute(Card c)
    {
        Debug.Log("Executing P2Draw");
        if (c != null && GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player2)
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
