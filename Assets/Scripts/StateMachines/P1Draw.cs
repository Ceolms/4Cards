﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Draw : StateMachineBehaviour,IExecute
{
    Card card;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TextViewer.Instance.SetText("Player 1 Draw");
        Discard.Instance.ShowParticles(true);
        Deck.Instance.ShowParticles(true);    
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (card ==  null)
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
    }

    private void CheckTouch(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject cardHit = hit.collider.gameObject;
            Card c = cardHit.GetComponent<Card>();  
            if(c.owner == Card.Owner.Deck || c.owner == Card.Owner.Discard)
            {
                card = c; 
                // Double click Checking
                if(GameManager.Instance.gameObject.GetComponent<DoubleClick>() == null)
                {
                    GameManager.Instance.gameObject.AddComponent<DoubleClick>();
                    GameManager.Instance.gameObject.GetComponent<DoubleClick>().state = this;
                }
                GameManager.Instance.gameObject.GetComponent<DoubleClick>().CheckDoubleClick(c);
            }
        }
    }

    public void Draw()
    {
        card.MoveTo(Card.Position.PlayerChoice);
        card.SetHidden(false);
        Deck.Instance.ShowParticles(false);
        Discard.Instance.ShowParticles(false);
        GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
    }

    public void Execute()
    {
        Draw();
    }
}
