using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClick : MonoBehaviour // This class is used to check if a card is clicked or double clicked
{
    public Card card;
    private bool isRunning = false;
    [SerializeField]
    private int count = 0;
    public Card.Owner playerID;

    public DoubleClick(Card c)
    {
        this.card = c;
    }

    public void Click(Card.Owner p)
    {
        if (!isRunning)
        {
            playerID = p;
            count += 1;
            StartCoroutine(CheckDoubleClick_Routine());
        }
        else count += 1;
    }

    public IEnumerator CheckDoubleClick_Routine() // is the card is clicked only once , we can execute the action
    {
        isRunning = true;
        yield return new WaitForSeconds(0.6f);
        if (count < 2)
        {
            GameManager.Instance.selectedCard = null;
            GameManager.Instance.state.Execute(this.card);
        }
        else if(GameManager.Instance.multiplayer && GameManager.Instance.selectedCard.owner == MultiPlayerController.LocalPlayerInstance.playerID)
        {
            GameManager.Instance.selectedCard = null;
            GameManager.Instance.TryDeleteCard(this.card, playerID);
        }
        else if(!GameManager.Instance.multiplayer)
        {
            GameManager.Instance.selectedCard = null;
            GameManager.Instance.TryDeleteCard(this.card, playerID);
        }
        Destroy(this);
    }
}

public interface IExecute
{
    void Execute();
}