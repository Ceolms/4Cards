using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Draw : CustomStateMachine
{
    Card card;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameManager.Instance.gamemode.Equals("IA"))
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
            c.MoveTo(Card.Position.PlayerChoice);
            c.SetHidden(false);
            Deck.Instance.ShowParticles(false);
            Discard.Instance.ShowParticles(false);
            GameManager.Instance.ChangePhase();
        }
        else if (card.position == Card.Position.Discard)
        {
            c = Discard.Instance.Draw();
            c.MoveTo(Card.Position.PlayerChoice);
            c.SetHidden(false);
            Deck.Instance.ShowParticles(false);
            Discard.Instance.ShowParticles(false);
            if (GameManager.Instance.gamemode.Equals("IA"))
            { IA.Instance.opponentKnownCards.Add(c); }
            GameManager.Instance.ChangePhase();
        }       
    }

    public override void Execute(Card c)
    {

        card = c;
        if (GameManager.Instance.powerChar == 'N')
        {
            Draw();
        }  
    }

    public override void ChangePhase()
    {
        GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
    }
}
