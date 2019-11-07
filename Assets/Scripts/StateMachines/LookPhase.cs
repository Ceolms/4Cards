using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPhase : CustomStateMachine
{
    private int cardsSelected;
    public Card selectedCard1;
    public Card selectedCard2;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TextViewer.Instance.SetText("Look Phase");
        selectedCard1 = null;
        selectedCard2 = null;

        foreach (Card c in GameManager.Instance.cardsJ1)
        {
            c.SetParticles(true);
        }
        cardsSelected = 0;
        GameManager.Instance.state = this;
        if (GameManager.Instance.gamemode == "IA")
        {
            IA.Instance.LookPhase();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (selectedCard2 != null)
        {
            selectedCard1.SetHidden(true);
            selectedCard2.SetHidden(true);
            
            foreach (Card c in GameManager.Instance.cardsJ1)
            {
                c.SetParticles(false);
            }
            GameManager.Instance.gameLogic.SetBool("LookCompleteP1", true);
        }
    }

    public override void Execute(Card c)
    {
        if (c.owner == Card.Owner.Player1)
        {
            cardsSelected += 1;
            if (cardsSelected == 1) selectedCard1 = c;
            else if (cardsSelected == 2) selectedCard2 = c;
            c.SetHidden(false);
            c.SetParticles(false);
        }
    }

    public override void ChangePhase()
    {
    }
}
