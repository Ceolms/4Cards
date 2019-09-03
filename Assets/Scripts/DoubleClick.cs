using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClick : MonoBehaviour // This class is used to check if a card is clicked or double clicked
{
    public Card card;
    private bool isRunning = false;
    public int count = 0;

    public DoubleClick(Card c)
    {
        this.card = c;
    }

    public void CheckDoubleClick()
    {
        if(!isRunning)
        {
           // Debug.Log("Click First");
            count += 1;
            StartCoroutine(CheckDoubleClick_Routine());
        } 
    }

    public IEnumerator CheckDoubleClick_Routine() // is the card is clicked only once , we can execute the action
    {
        Debug.Log("DoubleClick Routine Started");
        isRunning = true;
        yield return new WaitForSeconds(0.6f);
        Debug.Log("Routine click count : " + count);
        if (count < 2)
        {
            Debug.Log("Routine ended , Execute()");
            GameManager.Instance.selectedCard = null;
            GameManager.Instance.state.Execute(this.card);
        }
        else
        {
            Debug.Log("Routine ended , double click detected");
            GameManager.Instance.selectedCard = null;
            GameManager.Instance.TryDeleteCard(this.card);
        }
        Destroy(this);
    }
}

public interface IExecute
{
    void Execute();
}