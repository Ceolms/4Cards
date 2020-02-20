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

        if(GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player2)
        {
            foreach (Card c in GameManager.Instance.cardsJ2)
            {
                c.SetParticles(true);
            }
        }
        else
        {
            foreach (Card c in GameManager.Instance.cardsJ1)
            {
                c.SetParticles(true);
            }
        }
        
        cardsSelected = 0;
        GameManager.Instance.state = this;
        if (!GameManager.Instance.multiplayer)
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

            if (GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player2)
            {
                GameManager.Instance.gameLogic.SetBool("LookCompleteP2", true);
                MultiPlayerController.LocalPlayerInstance.photonView.RPC("SetAnimatorBool", PhotonTargets.Others, "LookCompleteP2",true);
                foreach (Card c in GameManager.Instance.cardsJ2)
                {
                    c.SetParticles(false);
                }
            }
            else
            {
                  if (GameManager.Instance.multiplayer) MultiPlayerController.LocalPlayerInstance.photonView.RPC("SetAnimatorBool", PhotonTargets.Others, "LookCompleteP1", true);
                GameManager.Instance.gameLogic.SetBool("LookCompleteP1", true);
                foreach (Card c in GameManager.Instance.cardsJ1)
                {
                    c.SetParticles(false);
                }
            }
            
        }
    }

    public override void Execute(Card c)
    {

        if (GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player2)
        {
            if (c.owner == Card.Owner.Player2)
            {
                cardsSelected += 1;
                if (cardsSelected == 1) selectedCard1 = c;
                else if (cardsSelected == 2) selectedCard2 = c;
                c.SetHidden(false);
                c.SetParticles(false);
                  if (GameManager.Instance.multiplayer) MultiPlayerController.LocalPlayerInstance.photonView.RPC("ShakeCard", PhotonTargets.Others, c.name);
            }
        }
        else
        {
            if (c.owner == Card.Owner.Player1)
            {
                cardsSelected += 1;
                if (cardsSelected == 1) selectedCard1 = c;
                else if (cardsSelected == 2) selectedCard2 = c;
                c.SetHidden(false);
                c.SetParticles(false);
                  if (GameManager.Instance.multiplayer) MultiPlayerController.LocalPlayerInstance.photonView.RPC("ShakeCard", PhotonTargets.Others, c.name);
            }
        }
       
    }

    public override void ChangePhase()
    {
        // nothing to do here
    }

    public override bool CanDeleteCard()
    {
        return false;
    }
}
