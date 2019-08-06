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
        PreWarm();
        stack = new List<GameObject>();
    }

    Card Draw()
    {
        GameObject obj = RemoveAndGet<GameObject>(this.stack, 0);
        return obj.GetComponent<Card>();
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
            stack.Add(obj);
            Card c = obj.GetComponent<Card>();
            c.owner = Card.Owner.Deck;

            c.isVisible = true;
            c.MoveTo(Card.Position.Deck);
            
        }
        

        Shuffle(); // for the cards to move to the Deck
        
        for (int i = 0; i < stack.Count; i++) // show only first card of deck and with the hidden face
        {
            Card c = stack[i].GetComponent<Card>();
            c.isHidden = true;
            //c.isVisible = false;
            //if (i <= 8) c.isVisible = true; // berk
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

    private static T RemoveAndGet<T>(IList<T> list, int index)
    {
        lock (list)
        {
            T value = list[index];
            list.RemoveAt(index);
            return value;
        }
    }

    public void ShowParticles(bool b)
    {
        var main = ps.main;
        if (b) ps.Play();
        else ps.Stop();
        Debug.Log("Particle : " + b + "(Deck)");
    }
    
    public void PreWarm()
    {
        var main = ps.main;
        main.prewarm = true;
    }
}
