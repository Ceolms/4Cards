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
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(selectedCard2 != null) // TODO WAIT otherPlayer
        {
            if(GameManager.Instance.firstToPlay == 1) GameManager.Instance.gameLogic.SetTrigger("LookCompleteP1");
            selectedCard1.SetHidden(true);
            selectedCard2.SetHidden(true);
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (Card c in cardsList)
        {
            if (c != null)
            {
                c.SetParticles(false);
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
}
