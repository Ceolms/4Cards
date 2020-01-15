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
    public string deckData;

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
        Debug.Log("UpdatePosition");
        if (stack.Count > 0)
        {
            stack[0].GetComponent<Card>().SetFront(true);

            for (int i = 1; i < stack.Count; i++)
            {
                stack[i].GetComponent<Card>().SetFront(false);
            }
        }
    }

    public Card Draw()
    {
        GameObject obj = RemoveAndGet<GameObject>(this.stack, 0);
        UpdatePosition();
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
        card.SetFront(true);
        card.MoveTo(Card.Position.Discard);
        yield return new WaitUntil(() => card.isMoving == false);
        stack[0].GetComponent<Card>().SetFront(true);
        GameManager.Instance.gameLogic.SetTrigger("DistributeComplete"); // Draw Phase Complete, proceed to next phase
    }


    public void InitRound() // put all cards in deck, shuffle and distribute
    {
        List<GameObject> cards = GameObject.FindGameObjectsWithTag("Card").ToList<GameObject>();
        foreach (GameObject go in cards)
        {
            Card c = go.GetComponent<Card>();
            c.MoveTo(Card.Position.Deck);
        }
        UpdatePosition();
        if (GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player1)
        {
            Debug.Log("Gamemode : multi player 1");
            Shuffle();
            SendDeck();

            StartCoroutine(Distribute());
        }
        else if (GameManager.Instance.multiplayer && receivedDeckEvent) // player 2
        {
            Debug.Log("Gamemode : multi player 2");
            string[] dataSplit = deckData.Split(',');
            Deck.Instance.stack = new List<GameObject>();
            for (int i = 0; i < dataSplit.Length - 1; i++)
            {
                GameObject o = GameObject.Find(dataSplit[i]);
                if (o != null)
                    Deck.Instance.stack.Add(o);
            }
            StartCoroutine(Distribute());
        }
        else if (!GameManager.Instance.multiplayer)  // vs IA
        {
            Shuffle();
            Debug.Log("Gamemode : IA");
            StartCoroutine(Distribute());
        }
    }

    private void Shuffle()
    {
        Debug.Log("Shuffle");
        if(stack.Count == 0 ) Debug.LogError("Stack empty");
        List<GameObject> cards = stack.ToList<GameObject>();
        stack.Clear();

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<Card>().isHidden = true;
            GameObject temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<Card>().isHidden = true;
            GameObject temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
        
        for (int i = 0; i < cards.Count; i++)
        {
            stack.Add(cards[i]);
            cards[i].GetComponent<Card>().SetFront(false);
        }
        stack[0].GetComponent<Card>().SetFront(true);
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
        Debug.Log("Sending deck");
        MultiPlayerController.LocalPlayerInstance.photonView.RPC("DeckShuffleData", PhotonTargets.Others, myStringBuilder.ToString());
    }
}
