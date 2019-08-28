﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPhase : StateMachineBehaviour
{
    public int cardsSelected;
    private Card selectedCard1;
    private Card selectedCard2;
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
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (cardsSelected < 2)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //Debug.Log("MobileApplication");
                if (Input.touchCount > 0 && Input.touchCount < 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                        CheckTouch(ray);
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                //Debug.Log("WindowsApplication");
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    CheckTouch(ray);
                }
            }
        }
        else if(true) // TODO WAIT otherPlayer
        {
            if(GameManager.Instance.firstToPlay == 1) GameManager.Instance.gameLogic.SetTrigger("LookCompleteP1");
            else GameManager.Instance.gameLogic.SetTrigger("LookCompleteP2");
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

    private void CheckTouch(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray,out hit))
        {
            GameObject cardHit = hit.collider.gameObject;
            Card card = cardHit.GetComponent<Card>();
            Debug.Log("Card Touched:" + card);
            if(card.owner == Card.Owner.Player)
            {
                cardsSelected += 1;
                if (cardsSelected == 1) selectedCard1 = card;
                else if (cardsSelected == 2) selectedCard2 = card;
                card.SetHidden(false);
                card.SetParticles(false);
            }
            
        }
    }
}
