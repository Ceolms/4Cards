using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<GameObject> stack;
    public static System.Random rnd = new System.Random();
    public static Deck Instance;
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        ps = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        ShowParticles(false);
        stack = new List<GameObject>();
    }

    public void UpdatePosition()
    {
        if (stack.Count > 0)
        {
            stack[0].GetComponent<Card>().SetFront(true);

            for (int i = 1; i < stack.Count; i++)
            {
                stack[0].GetComponent<Card>().SetFront(false);
            }
        }
    }

    Card Draw()
    {
        GameObject obj = stack[0];
        stack.RemoveAt(0);
        Card c = obj.GetComponent<Card>();
        c.SetFront(true);
        return c;
    }

    private IEnumerator Distribute()
    {
        //berk !
        //Debug.Log("Distribute");
        Card card = Draw();
        card.owner = Card.Owner.Player;
        card.MoveTo(Card.Position.Player_Slot1);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.owner = Card.Owner.Player2;
        card.MoveTo(Card.Position.Player2_Slot1);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.owner = Card.Owner.Player;
        card.MoveTo(Card.Position.Player_Slot2);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.owner = Card.Owner.Player2;
        card.MoveTo(Card.Position.Player2_Slot2);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.owner = Card.Owner.Player;
        card.MoveTo(Card.Position.Player_Slot3);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.owner = Card.Owner.Player2;
        card.MoveTo(Card.Position.Player2_Slot3);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.owner = Card.Owner.Player;
        card.MoveTo(Card.Position.Player_Slot4);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.owner = Card.Owner.Player2;
        card.MoveTo(Card.Position.Player2_Slot4);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.owner = Card.Owner.Discard;
        card.MoveTo(Card.Position.Discard);
        yield return new WaitUntil(() => card.isMoving == false);
        GameManager.Instance.gameLogic.SetTrigger("DistributeComplete"); // Draw Phase Complete, proceed to next phase
    }


    public void InitRound() // put all cards in deck, suffle and distribute
    {
        
        //Debug.Log("Init Round");

        stack.Clear();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Card"))
        {
            Card c = obj.GetComponent<Card>();
            c.owner = Card.Owner.Deck;
            c.SetFront(true);
            c.MoveTo(Card.Position.Deck);           
        }

        Shuffle(); // for the cards to move to the Deck
        
        for (int i = 0; i < stack.Count; i++) // show only first card of deck and with the hidden face
        {
            Card c = stack[i].GetComponent<Card>();
            c.isHidden = true; // DEBUG view
        }
        
        StartCoroutine(Distribute());
        
    }

    private void Shuffle()
    {
        //Debug.Log("Shuffling Start");
       
        List<GameObject> c = stack.ToList<GameObject>();
        stack.Clear();

        for (int i = 0; i < c.Count; i++)
        {
            c[i].GetComponent<Card>().isHidden = true;
            GameObject temp = c[i];
            int randomIndex = Random.Range(i, c.Count);
            c[i] = c[randomIndex];
            c[randomIndex] = temp;
        }
        for (int i = 0; i < c.Count; i++)
        {
            c[i].GetComponent<Card>().isHidden = true;
            GameObject temp = c[i];
            int randomIndex = Random.Range(i, c.Count);
            c[i] = c[randomIndex];
            c[randomIndex] = temp;
        }
        for (int i = 0; i < c.Count; i++)
        {
            stack.Add(c[i]);
        }
        //Debug.Log("Shuffling Done");
        
    }
    
    public void ShowParticles(bool b)
    {
        var main = ps.main;
        if (b)
        {
            ps.Simulate(4f);
            ps.Play();
        }
        else
        {
            
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
