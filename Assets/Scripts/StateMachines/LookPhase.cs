using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPhase : CustomStateMachine
{
    private int cardsSelected;
    public Card selectedCard1;
    public Card selectedCard2;
    private List<Card> cardsList;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TextViewer.Instance.SetText("Look Phase");

        cardsList = new List<Card>();
        cardsList.Add(GameManager.Instance.FindByPosition(Card.Position.Player_Slot1));
        cardsList.Add(GameManager.Instance.FindByPosition(Card.Position.Player_Slot2));
        cardsList.Add(GameManager.Instance.FindByPosition(Card.Position.Player_Slot3));
        cardsList.Add(GameManager.Instance.FindByPosition(Card.Position.Player_Slot4));
        cardsList.Add(GameManager.Instance.FindByPosition(Card.Position.Player_Slot5));
        cardsList.Add(GameManager.Instance.FindByPosition(Card.Position.Player_Slot6));

        foreach (Card c in cardsList)
        {
            if (c != null)
            {
                c.SetParticles(true);
            }
        }
        cardsSelected = 0;
        GameManager.Instance.state = this;
        if (GameManager.Instance.gameType == "IA")
        {
            IA.Instance.LookPhase();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (selectedCard2 != null)
        {
            GameManager.Instance.gameLogic.SetBool("LookCompleteP1", true);
            selectedCard1.SetHidden(true);
            selectedCard2.SetHidden(true);
            foreach (Card c in cardsList)
            {
                if (c != null)
                {
                    c.SetParticles(false);
                }
            }
        }
    }

    public override void Execute(Card c)
    {
        if (c.owner == Card.Owner.Player)
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
