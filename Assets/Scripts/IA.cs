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
        //Debug.Log("IA Draw phase, cards infos : " + cardsString);
        yield return new WaitForSeconds(1f); // to simulate the IA "thinking" and wait for the discarded card to finish moving
        Card discard = LookDiscard();


        if (discard.ValueToInt() <= 5 && HasUnknownCards()) // take the discard first card
        {
            //Debug.Log("IA take discard card because under 5 " + discard);
            Card c = Discard.Instance.Draw();
            c.MoveTo(Card.Position.Player2Choice);
            GameManager.Instance.ChangePhase();
            c.SetHidden(true);
        }
        else if (discard.ValueToInt() <= 8 && CheckBetterCard(discard))
        {
            //Debug.Log("IA take discard card because better than hand" + discard);
            Card c = Discard.Instance.Draw();
            c.MoveTo(Card.Position.Player2Choice);
            GameManager.Instance.ChangePhase();
            c.SetHidden(true);
        }
        else // take the deck first card
        {
            //Debug.Log("IA take deck card " + Deck.Instance.stack[0].GetComponent<Card>());
            Card c = Deck.Instance.Draw();
            c.MoveTo(Card.Position.Player2Choice);
            GameManager.Instance.ChangePhase();
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
                if ( !knownCards.Contains(card))
                {
                    cardSelection = true;
                    Debug.Log("IA discard an unknown card: " + card);
                    //  discard an unknown card and keep the new one
                    Card.Position pos = card.position;

                    card.MoveTo(Card.Position.Discard);
                    knownCards.Remove(card);
                    GameManager.Instance.cardsJ2.Remove(card);
                    chooseCard.MoveTo(pos);
                    knownCards.Add(chooseCard);
                    if (card.value == "Q") StartCoroutine(UseQueenPower());
                    else if (card.value == "J") StartCoroutine(UseJackPower());
                    break;
                }
            }
            if (!cardSelection) // if the IA already know all his cards and they are betters.
            {
                Debug.Log("IA discard the drawn card because of a better hand: " + chooseCard);
                chooseCard.MoveTo(Card.Position.Discard);
                if (chooseCard.value == "Q") StartCoroutine(UseQueenPower());
                else if (chooseCard.value == "J") StartCoroutine(UseJackPower());
            }
        }
        else // if the IA choose to discard one of his cards
        {
            Debug.Log("IA discard a lesser card of his hand: " + cardToDelete);
            knownCards.Add(chooseCard);
            knownCards.Remove(cardToDelete);
            Card.Position p = cardToDelete.position;
            GameManager.Instance.cardsJ2.Remove(cardToDelete);
            cardToDelete.MoveTo(Card.Position.Discard);
            GameManager.Instance.cardsJ2.Remove(cardToDelete);
            chooseCard.MoveTo(p);          
            if (cardToDelete.value == "Q") StartCoroutine(UseQueenPower());
            else if (cardToDelete.value == "J") StartCoroutine(UseJackPower());
        }
        if(knownCards.Count >= 4)
        {
            int score = 0;
            foreach(Card c in knownCards)
            {
                score += c.ValueToInt();

            }
            if (score < 6) GameManager.Instance.SetEndTurn("Opponent");
        }
        GameManager.Instance.ChangePhaseLong();  
    }

    public void CheckDeleteCard()
    {

        Card cardDiscard = Discard.Instance.stack[0].GetComponent<Card>();
        if(cardDiscard != null)
        {
            foreach(Card c in knownCards)
            {
                if(c.value.Equals(cardDiscard.value))
                {
                    c.MoveTo(Card.Position.Discard);
                    knownCards.Remove(c);
                }
            }
        }
        
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
        Card c = GameManager.Instance.LookCard(slot);
        knownCards.Add(c);
        c.Shake();
    }

    private Card LookDiscard()
    {
        return Discard.Instance.stack[0].GetComponent<Card>();
    }

    private bool HasUnknownCards()
    {
        int knowCardsCount = knownCards.Count;
        int totalCardsCount = 0 ;
        foreach (Card c in GameManager.Instance.cardsJ2) totalCardsCount += 1;
        if (knowCardsCount == totalCardsCount) return false;
        return true;
    }

    private IEnumerator UseJackPower()
    {
        yield return  new WaitForSeconds(1f);
        Card largestIACard = null;
        if (knownCards.Count > 0) largestIACard = knownCards[0];
        if (largestIACard != null)
        {
            foreach (Card c in knownCards)
            {
                if (c.ValueToInt() >= 9 && c.ValueToInt() > largestIACard.ValueToInt()) largestIACard = c ;
            }
        }

        Card smallestOpponentCard = null;
        if(opponentKnownCards.Count > 0 ) smallestOpponentCard = opponentKnownCards[0];
        if (smallestOpponentCard != null)
        {
            foreach (Card c in opponentKnownCards)
            {
                if (c.ValueToInt() <= 5 && c.ValueToInt() > smallestOpponentCard.ValueToInt()) smallestOpponentCard = c;
            }
        }

        if (largestIACard == null && smallestOpponentCard == null) // if the IA known nothing
        {
            Card cardIA = null;
            foreach(Card c in GameManager.Instance.cardsJ2)
            {
                if (c != null && !knownCards.Contains(c)) { cardIA = c; break; }
            }
            Card playerCard = null;
            foreach (Card c in GameManager.Instance.cardsJ1)
            {
                if (c != null && !opponentKnownCards.Contains(c)) {cardIA = c; break;}
            }
            Exchange(cardIA, playerCard);
        }
        if(largestIACard == null)
        {
            Card cardIA = null;
            foreach (Card c in GameManager.Instance.cardsJ2)
            {
                if (c != null && !knownCards.Contains(c)) { cardIA = c; break; }
            }
            Exchange(cardIA, smallestOpponentCard);
        }
        if (smallestOpponentCard == null)
        {
            Card playerCard = null;
            foreach (Card c in GameManager.Instance.cardsJ1)
            {
                if (c != null && !opponentKnownCards.Contains(c)) { playerCard = c; break; }
            }
            Exchange(largestIACard, playerCard);
        }
        else
        {
            if (largestIACard.ValueToInt() > smallestOpponentCard.ValueToInt()) Exchange(largestIACard, smallestOpponentCard);
        }
        
    }
    private void Exchange(Card cardIA , Card cardPlayer)
    {
        Card.Position posIA = cardIA.position;
        Card.Position posPlayer = cardPlayer.position;

        cardIA.MoveTo(posPlayer);
        cardPlayer.MoveTo(posIA);

        knownCards.Remove(cardIA);
        knownCards.Add(cardPlayer);

        opponentKnownCards.Remove(cardPlayer);
        opponentKnownCards.Add(cardIA);
    }

    private IEnumerator UseQueenPower()
    {
        foreach(Card c in GameManager.Instance.cardsJ2)
        {
            if(!knownCards.Contains(c))
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
                Debug.Log("IA used Queen power to look the card " + knownCards[knownCards.Count - 1] + " at slot " + slot);
                break;
            }
        }
        yield return null;
    }
}
