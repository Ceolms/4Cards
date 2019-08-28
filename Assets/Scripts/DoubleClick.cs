using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClick : MonoBehaviour // This class is used to check if a card is clicked or double clicked
{
    public Card card;
    public IExecute state;
    private bool isRunning = false;
    private bool canExecute = true;

    public DoubleClick(IExecute state)
    {
        this.state = state;
    }

    public void CheckDoubleClick(Card c)
    {
        
        if(!isRunning)
        {
            card = c;
            StartCoroutine(CheckDoubleClick_Routine());
        }
        else
        {
            if (c = card) canExecute = false;
        }
    }

    public IEnumerator CheckDoubleClick_Routine() // is the card is clicked only once , we can execute the action
    {
        isRunning = true;
        yield return new WaitForSeconds(0.5f);
        if (canExecute) state.Execute();
        Destroy(this);
    }
}

public interface IExecute
{
    void Execute();
}