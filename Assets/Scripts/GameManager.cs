using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Animator gameLogic;
    private float animatorSpeed;
    Card cardSelected;
    public GameObject powerPanel;
    public int firstToPlay = 1;
    public int endTurn = 0; // 0 false , 1 for P1 and 2 for P2
    private char powerChar;
    private List<Card> cardsList = new List<Card>();

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
    }

    public void InitRound()
    {
        Deck.Instance.InitRound();
        Discard.Instance.stack = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Pour jetter une carte correspondante au Discard , faire un double click
        if (!gameLogic.GetCurrentAnimatorStateInfo(0).IsName("NewRound") && !gameLogic.GetCurrentAnimatorStateInfo(0).IsName("LookPhase") && !gameLogic.GetCurrentAnimatorStateInfo(0).IsName("EndPhase"))
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //Debug.Log("MobileApplication");
                if (Input.touchCount > 0 && Input.touchCount <= 2)
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
                //Debug.Log("WindowsApplication");
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    CheckTouch(ray);
                }
            }
        }
        CheckPower();
    }

    private void CheckTouch(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject cardHit = hit.collider.gameObject;
            Card card = cardHit.GetComponent<Card>();
            if (cardSelected != null && card == cardSelected) TryDeleteCard();
            else
            {
                cardSelected = card;
                StartCoroutine(CountDown());
            }
        }
    }

    private string CheckTouchUI(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject buttonHit = hit.collider.gameObject;
            if(buttonHit.tag =="Button")
            {
                return buttonHit.name;
            }
        }
        return null;
    }
    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1f);
        cardSelected = null;
    }

    private void TryDeleteCard()
    {
        if(cardSelected.owner == Card.Owner.Player)
        {
            if (Discard.Instance.stack.Count > 0 && cardSelected.value == Discard.Instance.stack[0].GetComponent<Card>().value)
            {
                cardSelected.owner = Card.Owner.Discard;
                cardSelected.MoveTo(Card.Position.Discard);
                Debug.Log("Card deleted:" + cardSelected);

                if (cardSelected.value == "Q") UsePower('Q');
                if (cardSelected.value == "J") UsePower('J');
            }
            else
            {
                Debug.Log("wrong card deleted!");
                //TODO pioche
            }
        }
    }

    public Card FindByPosition(Card.Position pos)
    {
        foreach(Card c in cardsList)
        {
            if (c.position == pos)
                return c;
        }
        return null;
    }

    public void CheckPower()
    {   
        if (powerChar != 'N')
        {
            if (!gameLogic.GetCurrentAnimatorStateInfo(0).IsName("NewRound") && !gameLogic.GetCurrentAnimatorStateInfo(0).IsName("LookPhase") && !gameLogic.GetCurrentAnimatorStateInfo(0).IsName("EndPhase"))
            {
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    //Debug.Log("MobileApplication");
                    if (Input.touchCount > 0 && Input.touchCount <= 2)
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                            string s = CheckTouchUI(ray);
                            if (s == "YesButton")
                            {
                                powerPanel.SetActive(false);
                                if (powerChar == 'J') StartCoroutine(UsePowerExchange());
                                else if (powerChar == 'Q') StartCoroutine(UsePowerLook());
                                powerChar = 'N';
                            }
                            else if (s == "NoButton")
                            {
                                powerPanel.SetActive(false);
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
                            if (powerChar == 'J') StartCoroutine(UsePowerExchange());
                            else if (powerChar == 'Q') StartCoroutine(UsePowerLook());
                            powerChar = 'N';
                        }
                        else if (s == "NoButton")
                        {
                            powerPanel.SetActive(false);
                            gameLogic.speed = animatorSpeed;
                            powerChar = 'N';
                        }
                    }
                }
            }
        }
    }

    public void UsePower(char p)
    {
        Debug.Log("Power activated : " + p);
        /*
        gameLogic.speed = 0;
        powerPanel.SetActive(true);
        powerChar = p;
        if (p == 'Q') GameObject.Find("TextPower").GetComponent<UnityEngine.UI.Text>().text = "Queen : Look at one of your cards";
        else if (p == 'J') GameObject.Find("TextPower").GetComponent<UnityEngine.UI.Text>().text = "Jack : Exchange two cards";    
        */
    }


    private IEnumerator UsePowerLook()
    { 
        //GameManager.Instance.gameLogic.SetTrigger("Select one of your card to reveal");
        Debug.Log("LookPower Activated");
        gameLogic.speed = animatorSpeed;
        //TODO POWER
        return null;
    }
    private IEnumerator UsePowerExchange()
    {
        Debug.Log("Exchange Activated");
        gameLogic.speed = animatorSpeed;
        //TODO POWER
        return null;
    }
}
