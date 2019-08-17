using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Discard : StateMachineBehaviour
{
    bool cardSelected = false;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TextViewer.Instance.SetText("Player 1 Discard");
        Discard.Instance.ShowParticles(false);
        Deck.Instance.ShowParticles(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!cardSelected)
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

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    private void CheckTouch(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject cardHit = hit.collider.gameObject;
            Card card = cardHit.GetComponent<Card>();

            if (card.owner == Card.Owner.Player) // If the player want to discard one of his cards
            {
                Card.Position p = card.position;
                card.owner = Card.Owner.Discard;
                card.MoveTo(Card.Position.Discard); // move old card to discard

                Card c = GameManager.Instance.FindByPosition(Card.Position.PlayerChoice); // take the new one to slot
                
                card.owner = Card.Owner.Discard;
                card.MoveTo(Card.Position.Discard);

                c.SetHidden(true);
                c.MoveTo(p);
                c.owner = Card.Owner.Player;
                
                if(card.value == "Q") GameManager.Instance.UsePower('Q');
                if (card.value == "J") GameManager.Instance.UsePower('J'); 
                GameManager.Instance.gameLogic.SetTrigger("DiscardComplete");
            }
            else if (card.position == Card.Position.PlayerChoice) // else if it's the one he drawn
            {
                card.owner = Card.Owner.Discard;
                card.MoveTo(Card.Position.Discard);
                
                if (card.value == "Q") GameManager.Instance.UsePower('Q');
                if (card.value == "J") GameManager.Instance.UsePower('J');
                GameManager.Instance.gameLogic.SetTrigger("DiscardComplete");
            }
        }
    }
}
