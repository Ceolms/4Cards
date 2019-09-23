﻿using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndRound : CustomStateMachine
{
    int scoreP1 = 0 ;
    int scoreOpponent = 0 ;

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
                scoreOpponent += c.ValueToInt();
            }
        }
        if(scoreP1 == scoreOpponent)
        {
            if(GameManager.Instance.endRoundPlayer.Equals("Player1"))
            {
                TextViewer.Instance.SetText("You Lose !");
                GameManager.Instance.scoreP2 += 1;
            }
            else
            {
                TextViewer.Instance.SetText("You Won !");
                GameManager.Instance.scoreP1 += 1;
            }
        }
        else if(scoreP1 < scoreOpponent)
        {
            TextViewer.Instance.SetText("You Won !");
            GameManager.Instance.scoreP1 += 1;
        }
        else
        {
            TextViewer.Instance.SetText("You Lose !");
            GameManager.Instance.scoreP2 += 1;
        }

        GameObject.Find("TextScoreP1").GetComponent<Text>().text = "Your Score : " + GameManager.Instance.scoreP1;
        GameObject.Find("TextScoreP2").GetComponent<Text>().text = "Opponent Score : " + GameManager.Instance.scoreP2;

        GameObject button = GameObject.Find("ActionButton");
        GameObject.Find("OutlineBox").GetComponent<cakeslice.Outline>().enabled = false;
        button.GetComponentInChildren<Text>().text = "New Round";
    }

    public override void Execute(Card c)
    {
    }

    public override void ChangePhase()
    {
        throw new System.NotImplementedException();
    }
}