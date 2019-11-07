using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndRound : CustomStateMachine
{
    int scoreP1 = 0 ;
    int scoreP2 = 0 ;

    // Start is called before the first frame update
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.Instance.state = this;
        foreach(Card c in GameManager.Instance.cardsJ1)
        {
            if(c != null)
            {
                c.SetHidden(false);
                scoreP1 += c.ValueToInt();
            }
        }
        foreach (Card c in GameManager.Instance.cardsJ2)
        {
            if (c != null)
            {
                c.SetHidden(false);
                scoreP2 += c.ValueToInt();
            }
        }
        if(scoreP1 == scoreP2)
        {
            if(GameManager.Instance.endRoundPlayer == Card.Owner.Player1)
            {
                TextViewer.Instance.SetText("You Lose !");
                GameManager.Instance.scoreP2 += 1;
                GameManager.Instance.firstToPlay = 1;
            }
            else
            {
                TextViewer.Instance.SetText("You Won !");
                GameManager.Instance.scoreP1 += 1;
                GameManager.Instance.firstToPlay = 0;
            }
        }
        else if(scoreP1 < scoreP2)
        {
            TextViewer.Instance.SetText("You Won !");
            GameManager.Instance.scoreP1 += 1;
            GameManager.Instance.firstToPlay = 0;
        }
        else
        {
            TextViewer.Instance.SetText("You Lose !");
            GameManager.Instance.scoreP2 += 1;
            GameManager.Instance.firstToPlay = 1;
        }
        GameManager.Instance.UpdateScoreText();
        TextViewer.Instance.SetNewRound();
    }

    public override void Execute(Card c)
    {
    }

    public override void ChangePhase()
    {
        throw new System.NotImplementedException();
    }
}
