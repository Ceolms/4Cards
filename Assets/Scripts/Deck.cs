using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<GameObject> stack;
    public static System.Random rnd = new System.Random();
    public static Deck Instance;
    public bool receivedDeckEvent = false;
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        ps = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        ShowParticles(false);
        stack = new List<GameObject>();
    }
    private void Update()
    {
        if (receivedDeckEvent)
        {
            InitRound();
            receivedDeckEvent = false;
        }
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

    public Card Draw()
    {
        GameObject obj = RemoveAndGet<GameObject>(this.stack, 0);
        stack[0].GetComponent<Card>().SetFront(true);
        return obj.GetComponent<Card>();
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

    private IEnumerator Distribute()
    {
        yield return new WaitForSeconds(0.5f);
        if (stack.Count < 48) Debug.LogError("Error deck size :" + stack.Count);
        stack[0].GetComponent<Card>().SetFront(true);
        for (int i = 1; i < stack.Count; i++) // show only first card of deck and with the hidden face
        {
            Card c = stack[i].GetComponent<Card>();
            stack[i].GetComponent<Card>().SetFront(false);
        }

        CustomDebug.Instance.Log("Deck before distribution : ");
        foreach (GameObject go in stack)
        {
            CustomDebug.Instance.Log(go.name);
        }
        Card card = Draw();
        card.MoveTo(Card.Position.Player1_Slot1);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.MoveTo(Card.Position.Player2_Slot1);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.MoveTo(Card.Position.Player1_Slot2);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.MoveTo(Card.Position.Player2_Slot2);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.MoveTo(Card.Position.Player1_Slot3);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.MoveTo(Card.Position.Player2_Slot3);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.MoveTo(Card.Position.Player1_Slot4);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.MoveTo(Card.Position.Player2_Slot4);
        yield return new WaitUntil(() => card.isMoving == false);
        card = Draw();
        card.MoveTo(Card.Position.Discard);
        yield return new WaitUntil(() => card.isMoving == false);
        GameManager.Instance.gameLogic.SetTrigger("DistributeComplete"); // Draw Phase Complete, proceed to next phase
    }


    public void InitRound() // put all cards in deck, shuffle and distribute
    {
        List<GameObject> cards = GameObject.FindGameObjectsWithTag("Card").ToList<GameObject>();

        foreach(GameObject go in cards)
        {
            Card c = go.GetComponent<Card>();
            c.MoveTo(Card.Position.Deck);
        }
        if (GameManager.Instance.gamemode.Equals("multiplayer") && PlayerController.LocalPlayerInstance.playerID == Card.Owner.Player1)
        {
            Shuffle();
            SendDeck();

            StartCoroutine(Distribute());
        }
        else if (GameManager.Instance.gamemode.Equals("multiplayer") && receivedDeckEvent) // player 2
        {
            StartCoroutine(Distribute());
        }
        else if (GameManager.Instance.gamemode.Equals("IA"))  // vs IA
        {
            Debug.Log("Gamemode : IA");
            Shuffle(); // for the cards to move to the Deck
            StartCoroutine(Distribute());
        }
    }

    private void Shuffle()
    {
        Debug.Log("Shuffling Start");

        if(stack.Count == 0 ) Debug.LogError("Stack empty");
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
            c[i].GetComponent<Card>().SetFront(true);
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
    public void SendDeck()
    {
        StringBuilder myStringBuilder = new StringBuilder("");
        StringBuilder debugString = new StringBuilder("");
        foreach (GameObject obj in stack)
        {
            myStringBuilder.Append(obj.name + ",");
            debugString.Append(obj.name + "\n");
        }
        PlayerController.LocalPlayerInstance.photonView.RPC("DeckShuffleData", PhotonTargets.Others, myStringBuilder.ToString());
    }
}
