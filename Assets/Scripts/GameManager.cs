using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Animator gameLogic;
    private float animatorSpeed;
    public GameObject powerPanel; // panel with Yes/No question to use power

    public int firstToPlay = 1;
    public int endTurn = 0; // 0 false , 1 for P1 and 2 for P2
    [HideInInspector]
    public CustomStateMachine state;

    public Card selectedCard;
    public Card selectedOpponentCard;

    //special power values
    private bool powerPanelVisible;
    public char powerChar;

    private List<Card> cardsList = new List<Card>();
    public List<Card> cardsJ1 = new List<Card>();
    public List<Card> cardsJ2 = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        gameLogic = GetComponent<Animator>();
        powerPanel = GameObject.Find("PowerPanel");
        powerPanel.SetActive(false);

        List<GameObject> listObj = new List<GameObject>();
        listObj.AddRange(GameObject.FindGameObjectsWithTag("Card"));
        foreach (GameObject o in listObj)
        {
            cardsList.Add(o.GetComponent<Card>());
        }
        for (int i = 0; i <= 5; i++)
        {
            cardsJ1.Add(null);
            cardsJ2.Add(null);
        }
        animatorSpeed = gameLogic.speed;
        powerChar = 'N';
    }

    public void InitRound()
    {
        Deck.Instance.InitRound();
        Discard.Instance.stack = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    CheckTouch(ray);
                }
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                CheckTouch(ray);
            }
        }
        if (powerChar == 'J' && selectedCard != null && selectedOpponentCard != null)
        {
            Card.Position p1 = selectedCard.position;
            Card.Position p2 = selectedOpponentCard.position;
            selectedCard.MoveTo(p2);
            selectedOpponentCard.MoveTo(p1);
            selectedCard.SetParticles(false);
            selectedOpponentCard.SetParticles(false);
            selectedCard = null;
            selectedOpponentCard = null;
            powerChar = 'N';
            gameLogic.speed = animatorSpeed;
        }
    }

    private void CheckTouch(Ray ray)
    {
        RaycastHit hit;
        if (powerPanelVisible)
        {
            CheckPower();
        }
        else if (Physics.Raycast(ray, out hit))
        {
            GameObject cardHit = hit.collider.gameObject;
            Card c = cardHit.GetComponent<Card>();
            Debug.Log("Card Clicked : " + c.ToString());


            if (powerChar == 'N')
            {
                if (selectedCard != null && selectedCard != c)
                {
                    //Destroy(selectedCard.GetComponent<DoubleClick>());
                    selectedCard = c;
                    if (c.gameObject.GetComponent<DoubleClick>() == null)
                    {
                        DoubleClick dc = this.gameObject.AddComponent<DoubleClick>();
                        dc.card = c;
                        dc.CheckDoubleClick();
                    }
                }
                else if (selectedCard == null)
                {
                    selectedCard = c;
                    if (c.gameObject.GetComponent<DoubleClick>() == null)
                    {
                        DoubleClick dc = this.gameObject.AddComponent<DoubleClick>();
                        dc.card = c;
                        dc.CheckDoubleClick();
                    }
                }
                else
                {
                    DoubleClick dc = this.gameObject.GetComponent<DoubleClick>();
                    if (dc != null) dc.count += 1;
                }
            }
            else if (powerChar == 'Q')
            {
                if (c.owner == Card.Owner.Player)
                {
                    c.SetHidden(false);
                    //Destroy(c.GetComponent<DoubleClick>());
                    foreach (Card card in cardsJ1)
                    {
                        if (card != null) card.SetParticles(false);
                    }
                    powerChar = 'N';
                    gameLogic.speed = animatorSpeed;
                    c.SetHidden(true);
                }
            }
            else if (powerChar == 'J')
            {
                if (selectedCard == null || selectedOpponentCard == null)
                {
                    if (c.owner == Card.Owner.Player)
                    {
                        selectedCard = c;
                        // Destroy(c.GetComponent<DoubleClick>());
                        foreach (Card card in cardsJ1)
                        {
                            if (card != null && card != c) card.SetParticles(false);
                        }
                    }
                    else if (c.owner == Card.Owner.Player2)
                    {
                        selectedOpponentCard = c;
                        //  Destroy(c.GetComponent<DoubleClick>());
                        foreach (Card card in cardsJ2)
                        {
                            if (card != null && card != c) card.SetParticles(false);
                        }
                    }
                }
            }
        }
    }
    private string CheckTouchUI(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject buttonHit = hit.collider.gameObject;
            if (buttonHit.tag == "Button")
            {
                return buttonHit.name;
            }
        }
        return null;
    }

    public void TryDeleteCard(Card cardSelected)
    {
        if (cardSelected.owner == Card.Owner.Player)
        {
            if (Discard.Instance.stack.Count > 0 && cardSelected.value == Discard.Instance.stack[0].GetComponent<Card>().value)
            {
                cardSelected.owner = Card.Owner.Discard;
                cardSelected.MoveTo(Card.Position.Discard);
                Debug.Log("Card deleted:" + cardSelected);
                int i = cardsJ1.IndexOf(cardSelected);
                cardsJ1[i] = null;

                if (cardSelected.value == "Q") UsePower('Q');
                if (cardSelected.value == "J") UsePower('J');
            }
            else
            {
                Debug.Log("wrong card deleted!");
                for (int i = 0; i <= 5; i++)
                {
                    if (cardsJ1[i] == null)
                    {
                        Card c = Deck.Instance.Draw();
                        cardsJ1[i] = c;
                        // berk !
                        if (i == 0) c.MoveTo(Card.Position.Player_Slot1);
                        if (i == 1) c.MoveTo(Card.Position.Player_Slot2);
                        if (i == 2) c.MoveTo(Card.Position.Player_Slot3);
                        if (i == 3) c.MoveTo(Card.Position.Player_Slot4);
                        if (i == 4) c.MoveTo(Card.Position.Player_Slot5);
                        if (i == 5) c.MoveTo(Card.Position.Player_Slot6);
                        break;
                    }
                }
            }
        }
    }

    public Card FindByPosition(Card.Position pos)
    {
        foreach (Card c in cardsList)
        {
            if (c.position == pos)
                return c;
        }
        return null;
    }

    public void CheckPower()
    {

        if (!gameLogic.GetCurrentAnimatorStateInfo(0).IsName("NewRound") && !gameLogic.GetCurrentAnimatorStateInfo(0).IsName("LookPhase") && !gameLogic.GetCurrentAnimatorStateInfo(0).IsName("EndPhase"))
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0 && Input.touchCount <= 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                        string s = CheckTouchUI(ray);
                        if (s == "YesButton")
                        {
                            powerPanel.SetActive(false);
                            powerPanelVisible = false;
                            foreach (Card card in cardsJ1)
                            {
                                if (card != null) card.SetParticles(true);
                            }
                            if (powerChar == 'J')
                            {
                                foreach (Card card in cardsJ2)
                                {
                                    if (card != null) card.SetParticles(true);
                                }
                            }
                        }
                        else if (s == "NoButton")
                        {
                            powerPanel.SetActive(false);
                            powerPanelVisible = false;
                            gameLogic.speed = animatorSpeed;
                            powerChar = 'N';
                        }
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                //Debug.Log("WindowsApplication");
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    string s = CheckTouchUI(ray);
                    if (s == "YesButton")
                    {
                        powerPanel.SetActive(false);
                        powerPanelVisible = false;
                        foreach (Card card in cardsJ1)
                        {
                            if (card != null) card.SetParticles(true);
                        }
                        if (powerChar == 'J')
                        {
                            foreach (Card card in cardsJ2)
                            {
                                if (card != null) card.SetParticles(true);
                            }
                        }
                    }
                    else if (s == "NoButton")
                    {
                        powerPanel.SetActive(false);
                        powerPanelVisible = false;
                        gameLogic.speed = animatorSpeed;
                        powerChar = 'N';
                    }
                }
            }
        }
    }

    public void UsePower(char p)
    {
        Debug.Log("Power activated : " + p);

        gameLogic.speed = 0;
        powerPanel.SetActive(true);
        powerPanelVisible = true;
        powerChar = p;
        if (p == 'Q') GameObject.Find("TextPower").GetComponent<UnityEngine.UI.Text>().text = "Queen : Look at one of your cards";
        else if (p == 'J') GameObject.Find("TextPower").GetComponent<UnityEngine.UI.Text>().text = "Jack : Exchange two cards";

    }
}
