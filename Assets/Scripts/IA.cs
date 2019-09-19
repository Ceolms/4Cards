using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour
{
    public static IA Instance;

    [SerializeField] private List<Card> knowCards = new List<Card>();
    private int cardCount = 4;
    private int cardsKnowCount = 2;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        for (int i = 0; i <= 5; i++)
        {
            knowCards.Add(null);
        }
    }

    public void LookPhase()
    {
        LookCard(0);
        LookCard(1);
        GameManager.Instance.gameLogic.SetBool("LookCompleteP2", true);
    }

    public void DrawPhase()
    {
        StartCoroutine(DrawCoroutine());
    }

    private IEnumerator DrawCoroutine() // to simulate the IA "thinking" and wait for the discarded card to finish moving
    {
        yield return new WaitForSeconds(1f);
        Card discard = LookDiscard();
        if (discard.ValueToInt() <= 5 || CheckBetterCard(discard)) // take the discard first card
        {
            Card c = Discard.Instance.Draw();
            c.MoveTo(Card.Position.Player2Choice);
            GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
            c.SetHidden(true);
        }
        else // take the deck first card
        {
            Card c = Deck.Instance.Draw();
            c.MoveTo(Card.Position.Player2Choice);
            GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
        }
    }

    public void DiscardPhase()
    {
        Card chooseCard = GameManager.Instance.FindByPosition(Card.Position.Player2Choice);
        int chooseCardValue = chooseCard.ValueToInt();
        Card cardToDelete = chooseCard;
        int cardToDeleteValue = chooseCardValue;

        foreach (Card c in knowCards)
        {
            if (c != null && c.ValueToInt() > cardToDeleteValue)
            {
                cardToDelete = c;
                cardToDeleteValue = cardToDelete.ValueToInt();
            }
        }

        if (cardToDelete.position == Card.Position.Player2Choice) // if IA choose to discard the chooseCard or an unknown one
        {
            bool cardSelection = false;
            for (int i = 0; i < cardCount; i++)
            {
                if (knowCards[i] == null && GameManager.Instance.cardsJ2 != null)
                {
                    cardSelection = true;
                    //  discard an unknown card and keep the new one
                    Card.Position pos = knowCards[i].position;
                    switch (i)
                    {
                        case 0:
                            pos = Card.Position.Player2_Slot1;
                            break;
                        case 1:
                            pos = Card.Position.Player2_Slot2;
                            break;
                        case 2:
                            pos = Card.Position.Player2_Slot3;
                            break;
                        case 3:
                            pos = Card.Position.Player2_Slot4;
                            break;
                        case 4:
                            pos = Card.Position.Player2_Slot5;
                            break;
                        case 5:
                            pos = Card.Position.Player2_Slot6;
                            break;
                    }

                    knowCards[i].MoveTo(Card.Position.Discard);
                    chooseCard.MoveTo(pos);
                    knowCards[i] = chooseCard;
                    break;
                }
            }
            if (!cardSelection) // if the IA already know all his cards and they are betters.
            {
                chooseCard.MoveTo(Card.Position.Discard);
            }
        }
        else // if the IA choose to discard one of his cards
        {
            Card.Position p = cardToDelete.position;
            int i = knowCards.IndexOf(cardToDelete);
            cardToDelete.MoveTo(Card.Position.Discard);
            chooseCard.MoveTo(p);
            knowCards[i] = chooseCard;
        }
        GameManager.Instance.gameLogic.SetTrigger("DiscardComplete");
    }


    private bool CheckBetterCard(Card c)
    {
        foreach(Card card in knowCards)
        {
            if(knowCards != null && c.ValueToInt() < card.ValueToInt())
            {
                return true;
            }
        }
        return false;
    }
    private void LookCard(int slot)
    {
        knowCards[slot] = GameManager.Instance.LookCard(slot);
    }

    private Card LookDiscard()
    {
        return Discard.Instance.stack[0].GetComponent<Card>();
    }
}
