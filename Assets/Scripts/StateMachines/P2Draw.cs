using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Draw : CustomStateMachine
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TextViewer.Instance.SetText("Player 2 Draw");
        GameManager.Instance.state = this;
        
        if (GameManager.Instance.gamemode.Equals("IA"))
        {
            IA.Instance.CheckDeleteCard();
            IA.Instance.DrawPhase();
        }
        else
        {

        }
    }
    public override void Execute(Card c)
    {

    }

    public override void ChangePhase()
    {
        GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
    }
}
