using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> stack;
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
        stack = new List<Card>();
    }
    private void Update()
    {
        if (receivedDeckEvent)
        {
            InitRound();
            receivedDeckEvent = false;
        }
    }
    public Card Draw()
    {
        Card obj = RemoveAndGet<Card>(this.stack, 0);
      //  UpdatePosition();
        return obj;
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
       // card.SetFront(true);
        card.MoveTo(Card.Position.Discard);
        yield return new WaitUntil(() => card.isMoving == false);
     //   stack[0].GetComponent<Card>().SetFront(true);
        GameManager.Instance.gameLogic.SetTrigger("DistributeComplete"); // Draw Phase Complete, proceed to next phase
    }


    public void InitRound() // put all cards in deck, shuffle and distribute
    {
        List<GameObject> cards = GameObject.FindGameObjectsWithTag("Card").ToList<GameObject>();
        foreach (GameObject go in cards)
        {
            Card c = go.GetComponent<Card>();
            if(c != null) c.MoveTo(Card.Position.Deck);

        }
      //  UpdatePosition();
        if (GameManager.Instance.multiplayer && MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player1)
        {
            Debug.Log("Gamemode : multi player 1");
            Shuffle();
            SendDeck();
            SoundPlayer.Instance.Play("Shuffle");
            StartCoroutine(Distribute());
        }
        else if (GameManager.Instance.multiplayer && receivedDeckEvent) // player 2
        {
            Debug.Log("Gamemode : multi player 2");
            string[] dataSplit = deckData.Split(',');
            Deck.Instance.stack = new List<Card>();
            for (int i = 0; i < dataSplit.Length - 1; i++)
            {
                Card o = GameObject.Find(dataSplit[i]).GetComponent<Card>();
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
       
        if(stack.Count == 0 ) Debug.LogError("Stack empty");
        List<Card> cards = stack.ToList<Card>();
        stack.Clear();

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<Card>().isHidden = true;
            Card temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<Card>().isHidden = true;
            Card temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
        
        for (int i = 0; i < cards.Count; i++)
        {
            stack.Add(cards[i]);
        }

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
        foreach (Card obj in stack)
        {
            myStringBuilder.Append(obj.name + ",");
            debugString.Append(obj.name + "\n");
        }
        MultiPlayerController.LocalPlayerInstance.photonView.RPC("DeckShuffleData", PhotonTargets.Others, myStringBuilder.ToString());
    }
}
