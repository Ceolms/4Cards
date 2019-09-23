using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour
{
    public static IA Instance;

    [SerializeField]
    private List<Card> knownCards = new List<Card>();
    public  List<Card> opponentKnownCards = new List<Card>();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
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

    private IEnumerator DrawCoroutine() 
    {
        string cardsString = "";
        foreach(Card c in knownCards)
        {
            cardsString += c;
            cardsString += "    ";
        }
        Debug.Log("IA Draw phase, cards infos : " + cardsString);
        yield return new WaitForSeconds(1f); // to simulate the IA "thinking" and wait for the discarded card to finish moving
        Card discard = LookDiscard();


        if (discard.ValueToInt() <= 5 && HasUnknownCards()) // take the discard first card
        {
            Debug.Log("IA take discard card because under 5 " + discard);
            Card c = Discard.Instance.Draw();
            c.MoveTo(Card.Position.Player2Choice);
            GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
            c.SetHidden(true);
        }
        else if (discard.ValueToInt() <= 8 && CheckBetterCard(discard))
        {
            Debug.Log("IA take discard card because better than hand" + discard);
            Card c = Discard.Instance.Draw();
            c.MoveTo(Card.Position.Player2Choice);
            GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
            c.SetHidden(true);
        }
        else // take the deck first card
        {
            Debug.Log("IA take deck card " + Deck.Instance.stack[0].GetComponent<Card>());
            Card c = Deck.Instance.Draw();
            c.MoveTo(Card.Position.Player2Choice);
            GameManager.Instance.gameLogic.SetTrigger("DrawComplete");
        }
    }

    public void DiscardPhase()
    {
        Debug.Log("IA Discard phase");

        Card chooseCard = GameManager.Instance.FindByPosition(Card.Position.Player2Choice);
        int chooseCardValue = chooseCard.ValueToInt();
        Card cardToDelete = chooseCard;
        int cardToDeleteValue = chooseCardValue;

        foreach (Card c in knownCards)
        {
            if (c.ValueToInt() > cardToDeleteValue)
            {
                cardToDelete = c;
                cardToDeleteValue = cardToDelete.ValueToInt();
            }
        }
        if (cardToDelete.position == Card.Position.Player2Choice) // if IA choose to discard the chooseCard or an unknown one
        {
            bool cardSelection = false;
            foreach (Card card in GameManager.Instance.cardsJ2)
            {
                if (card != null && !knownCards.Contains(card))
                {
                    cardSelection = true;
                    Debug.Log("IA discard an unknown card: " + card);
                    //  discard an unknown card and keep the new one
                    Card.Position pos = card.position;

                    card.MoveTo(Card.Position.Discard);
                    knownCards.Remove(card);
                    chooseCard.MoveTo(pos);
                    knownCards.Add(chooseCard);
                    if (card.value == "Q") StartCoroutine(UseQueenPower());
                    else if (card.value == "J") StartCoroutine(UseJackPower());
                    break;
                }
            }
            if (!cardSelection) // if the IA already know all his cards and they are betters.
            {
                Debug.Log("IA discard the drawn card because of a better hand" + chooseCard);
                chooseCard.MoveTo(Card.Position.Discard);
            }
        }
        else // if the IA choose to discard one of his cards
        {
            Debug.Log("IA discard a lesser card of his hand" + cardToDelete);
            Card.Position p = cardToDelete.position;
            knownCards.Remove(cardToDelete);
            cardToDelete.MoveTo(Card.Position.Discard);
            chooseCard.MoveTo(p);
            knownCards.Add(chooseCard);
        }
        GameManager.Instance.gameLogic.SetTrigger("DiscardComplete");
    }


    private bool CheckBetterCard(Card c)
    {
        foreach(Card card in knownCards)
        {
            if(c.ValueToInt() < card.ValueToInt())
            {
                return true;
            }
        }
        return false;
    }
    private void LookCard(int slot)
    {
        knownCards.Add(GameManager.Instance.LookCard(slot));
    }

    private Card LookDiscard()
    {
        return Discard.Instance.stack[0].GetComponent<Card>();
    }

    private bool HasUnknownCards()
    {
        int knowCardsCount = knownCards.Count;
        int totalCardsCount = 0 ;
        foreach (Card c in GameManager.Instance.cardsJ2) if (c != null) totalCardsCount += 1;
        if (knowCardsCount == totalCardsCount) return false;
        return true;
    }

    private IEnumerator UseJackPower()
    {
        Card largestIACard = knownCards[0];
        Card smallestOpponentCard;
        if (opponentKnownCards.Count > 0) 

        foreach (Card c in knownCards)
        {
            if (c.ValueToInt() > largestIACard.ValueToInt()) largestIACard = c;
        }


        // si  je connais qui soient plus petit je prend
        // sinon si ma plus grosse >= 9 j'echange avec une inconnue
        // sinon j'echange 2 inconnues pour gener

            yield return null;
    }
    private IEnumerator UseQueenPower()
    {
        foreach(Card c in GameManager.Instance.cardsJ2)
        {
            if(c != null && !knownCards.Contains(c))
            {
                int slot = 0;
                switch(c.position)
                {
                    case Card.Position.Player2_Slot1:
                        slot = 0;
                        break;
                    case Card.Position.Player2_Slot2:
                        slot = 1;
                        break;
                    case Card.Position.Player2_Slot3:
                        slot = 2;
                        break;
                    case Card.Position.Player2_Slot4:
                        slot = 3;
                        break;
                    case Card.Position.Player2_Slot5:
                        slot = 4;
                        break;
                    case Card.Position.Player2_Slot6:
                        slot = 5;
                        break;
                }
                LookCard(slot);
                Debug.Log("IA used Queen power to look the card " + knownCards[knownCards.Count - 1] + "at slot " + slot);
            }
        }
        yield return null;
    }
}
