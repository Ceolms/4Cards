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
    private float phaseChangeTime = 0f;
    private float phaseChangeTimeLong = 2f;

    public string gameType = "IA";
    public int scoreP1 = 0;
    public int scoreP2 = 0;


    [HideInInspector]
    public int firstToPlay = 1;
    [HideInInspector]
    public string endRoundPlayer = "null"; // 0 false , 1 for P1 and 2 for P2


    public CustomStateMachine state;
    [HideInInspector]
    public Card selectedCard;
    [HideInInspector]
    public Card selectedOpponentCard;

    //special power values
    private bool powerPanelVisible;
    [HideInInspector]
    public char powerChar;

    [HideInInspector]
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

    // Player functions
    private void CheckTouch(Ray ray)
    {
        RaycastHit hit;
        string s = CheckTouchUI(ray);
        if(s!= null && s.Equals("ActionButton") && state is P1Discard) 
        {
            Debug.Log("EndRound Clicked! ");
            SetEndTurn("Player1");
        }
        else if (s != null &&  s.Equals("ActionButton") && state is EndRound)
        {
            Debug.Log("New Round Clicked! ");
            gameLogic.SetTrigger("NewRoundStart");
        }
        else if (powerPanelVisible)
        {
            CheckPower();
        }
        else if (Physics.Raycast(ray, out hit) && s== null)
        {
            GameObject cardHit = hit.collider.gameObject;
            Card c = cardHit.GetComponent<Card>();
            //Debug.Log("Card Clicked : " + c.ToString());


            if (powerChar == 'N')
            {
                if (selectedCard != null && selectedCard != c)
                {
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
                        foreach (Card card in cardsJ1)
                        {
                            if (card != null && card != c) card.SetParticles(false);
                        }
                    }
                    else if (c.owner == Card.Owner.Player2)
                    {
                        selectedOpponentCard = c;
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
            if (Discard.Instance.stack.Count > 0 && cardSelected.value.Equals(Discard.Instance.stack[0].GetComponent<Card>().value))
            {
                cardSelected.owner = Card.Owner.Discard;
                cardSelected.MoveTo(Card.Position.Discard);
                cardsJ1.Remove(cardSelected);
                Debug.Log("Card deleted:" + cardSelected);

                if (cardSelected.value == "Q") UsePower('Q');
                if (cardSelected.value == "J") UsePower('J');
            }
            else
            {
                TextViewer.Instance.SetTextTemporary("Wrong card !", Color.red);
                int[] arrayPos = new int[] { 1, 2, 3, 4, 5,6 };
                if (cardsJ1.Count < 6)
                {
                    foreach (Card c in cardsJ1)
                    {
                        switch (c.position)
                        {
                            case Card.Position.Player_Slot1:
                                arrayPos[0] = -1;
                                break;
                            case Card.Position.Player_Slot2:
                                arrayPos[1] = -1;
                                break;
                            case Card.Position.Player_Slot3:
                                arrayPos[2] = -1;
                                break;
                            case Card.Position.Player_Slot4:
                                arrayPos[3] = -1;
                                break;
                            case Card.Position.Player_Slot5:
                                arrayPos[4] = -1;
                                break;
                            case Card.Position.Player_Slot6:
                                arrayPos[0] = -1;
                                break;
                        }
                    }

                    foreach (int i in arrayPos)
                    {
                        Card.Position p = Card.Position.Player_Slot6;
                        if (i != -1)
                        {

                            switch (i)
                            {
                                case (1):
                                    p = Card.Position.Player_Slot1;
                                    break;
                                case (2):
                                    p = Card.Position.Player_Slot2;
                                    break;
                                case (3):
                                    p = Card.Position.Player_Slot3;
                                    break;
                                case (4):
                                    p = Card.Position.Player_Slot4;
                                    break;
                                case (5):
                                    p = Card.Position.Player_Slot5;
                                    break;
                                case (6):
                                    p = Card.Position.Player_Slot6;
                                    break;
                            }
                            Card c = Deck.Instance.Draw();
                            c.MoveTo(p);
                            cardsJ1.Add(c);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void SetEndTurn(string s)
    {
        endRoundPlayer = s;
        TextViewer.Instance.SetTextTemporary("END ROUND", Color.red);
    }
    public void ChangePhase()
    {
        StartCoroutine(WaitAndChange(phaseChangeTime));
    }
    public void ChangePhaseLong()
    {
        StartCoroutine(WaitAndChange(phaseChangeTimeLong));
    }

    private IEnumerator WaitAndChange(float t)
    {
        yield return new WaitForSeconds(t);
        state.ChangePhase();
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
                                 card.SetParticles(true);
                            }
                            if (powerChar == 'J')
                            {
                                foreach (Card card in cardsJ2)
                                {
                                     card.SetParticles(true);
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
                            card.SetParticles(true);
                        }
                        if (powerChar == 'J')
                        {
                            foreach (Card card in cardsJ2)
                            {
                                card.SetParticles(true);
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
        gameLogic.speed = 0;
        powerPanel.SetActive(true);
        powerPanelVisible = true;
        powerChar = p;
        if (p == 'Q') GameObject.Find("TextPower").GetComponent<UnityEngine.UI.Text>().text = "Queen : Look at one of your cards";
        else if (p == 'J') GameObject.Find("TextPower").GetComponent<UnityEngine.UI.Text>().text = "Jack : Exchange two cards";

    }

    //IA Functions

 

    public Card  LookCard(int index)
    {
        return  cardsJ2[index];
    }
   // Multiplayer Functions
}
