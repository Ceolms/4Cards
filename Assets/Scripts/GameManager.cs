using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Animator gameLogic;
    private float animatorSpeed;
    [HideInInspector]
    public GameObject powerPanel; // panel with Yes/No question to use power
    private float phaseChangeTime = 0f;
    private float phaseChangeTimeLong = 2f;
    public GameObject prefabPlayer;
    public GameObject prefabPlayerSolo;
    [HideInInspector]
    public bool multiplayer = false;
    private DoubleClick db;
    public bool debugMode;
    [HideInInspector]
    public bool gameBegin;
    [HideInInspector]
    public int firstToPlay = 1;
    [HideInInspector]

    public Card.Owner endRoundPlayer; // 0 false , 1 for P1 and 2 for P2

    public string namePlayer1;
    public string namePlayer2;
    public int scoreP1 = 0;
    public int scoreP2 = 0;

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

        if (PlayerPrefs.GetString("gamemode").Equals("IA"))
        {
            gameBegin = true;
            GameObject pp = Instantiate(prefabPlayerSolo);
        }
        else if (PlayerPrefs.GetString("gamemode").Equals("multiplayer"))
        {
            multiplayer = true;
            PhotonNetwork.Instantiate(prefabPlayer.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            // manager = GameObject.Find("MultiplayerManager").GetComponent<NetworkManager>();
        }
        else Debug.LogError("Error, Gamemode not set");
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedCard != null)
        {
            CustomDebug.Instance.SetText(selectedCard.name);
            CustomDebug.Instance.Log(selectedCard.owner.ToString());
        }
        else CustomDebug.Instance.SetText("null");
        if (gameBegin && multiplayer && state is NewRound)
        {
            if (PhotonNetwork.playerList.Length >= 2)
            {
                gameBegin = false;
                StartCoroutine(PrepareMultiplayer());
            }
        }
        else if (gameBegin && !multiplayer && state is NewRound)
        {
            gameBegin = false;
            state.Execute(null);
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



    public void UpdateScoreText()
    {
        GameObject.Find("ScoreText").GetComponent<Text>().color = Color.white;
        GameObject.Find("ScoreText").GetComponent<Text>().text = "Scores: " + namePlayer1 + " " + GameManager.Instance.scoreP1 + " - " + namePlayer2 + " " + GameManager.Instance.scoreP2;
    }
    private void Exit()
    {
        if (!multiplayer)
        {
            SceneManager.LoadScene("Main Menu");
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    // Player functions
    public void CheckTouch(Ray ray, Card.Owner player)
    {
        RaycastHit hit;
        string s = CheckTouchUI(ray);
        if (s != null && s.Equals("ActionButton") && state is P1Discard)
        {
            Debug.Log("EndRound Clicked! ");
            SetEndTurn(player);
        }
        else if (s != null && s.Equals("ActionButton") && state is EndRound)
        {
            Debug.Log("New Round Clicked! ");
            gameLogic.SetTrigger("NewRoundStart");
        }
        else if (s != null && s.Equals("ExitButton"))
        {
            Exit();
        }
        else if (powerPanelVisible)
        {
            CheckPower();
        }
        else if (Physics.Raycast(ray, out hit) && s == null)
        {
            GameObject cardHit = hit.collider.gameObject;
            Card c = cardHit.GetComponent<Card>();

            if (c == null) return;
            if (powerChar == 'N' && c.owner == player )
            {
                if (db != null && db.card == c)
                {
                    db.Click(player);
                }
                else if(db != null && db.card != c)
                {
                    Destroy(db);
                    this.db  = this.gameObject.AddComponent(typeof(DoubleClick)) as DoubleClick;
                    this.db.card = c;
                    this.db.Click(player);
                }
                else
                {
                    this.db = this.gameObject.AddComponent(typeof(DoubleClick)) as DoubleClick;
                    this.db.card = c;
                    this.db.Click(player);
                }
            }
            else if (c.owner == Card.Owner.Deck || c.owner == Card.Owner.Discard)
            {
                if (db != null) Destroy(db);
                state.Execute(c);
            }
            else if (powerChar == 'Q')
            {
                if (c.owner == player)
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
                    if (c.owner == player)
                    {
                        selectedCard = c;
                        if (player == Card.Owner.Player1)
                        {
                            foreach (Card card in cardsJ1)
                            {
                                if (card != null && card != c) card.SetParticles(false);
                            }
                        }
                        else
                        {
                            foreach (Card card in cardsJ2)
                            {
                                if (card != null && card != c) card.SetParticles(false);
                            }
                        }
                        // change IA knowledge about the two cards 
                        if (!multiplayer)
                        {
                            IA.Instance.opponentKnownCards.Remove(selectedCard);
                        }

                    }
                    else if (c.owner != player)
                    {
                        selectedOpponentCard = c;
                        if (player == Card.Owner.Player1)
                        {
                            foreach (Card card in cardsJ2)
                            {
                                if (card != null && card != c) card.SetParticles(false);
                            }
                        }
                        else
                        {
                            foreach (Card card in cardsJ1)
                            {
                                if (card != null && card != c) card.SetParticles(false);
                            }
                        }
                        // change IA knowledge about the two cards 
                        if (!multiplayer)
                        {
                            IA.Instance.RemoveKnown(selectedOpponentCard);
                            IA.Instance.opponentKnownCards.Add(selectedOpponentCard);
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
    public void TryDeleteCard(Card cardSelected, Card.Owner player)
    {
        if (cardSelected.owner == player && cardSelected.position != Card.Position.Discard && cardSelected.position != Card.Position.Deck)
        {
            if (Discard.Instance.stack.Count > 0 && cardSelected.value.Equals(Discard.Instance.stack[0].GetComponent<Card>().value))
            {
                cardSelected.MoveTo(Card.Position.Discard);

                if (cardSelected.value == "Q") UsePower('Q');
                if (cardSelected.value == "J") UsePower('J');

                if (player == Card.Owner.Player1 && cardsJ1.Count == 0)
                {
                    endRoundPlayer = player;
                    GameManager.Instance.gameLogic.SetTrigger("EndRound");
                }
                else if (player == Card.Owner.Player2 && cardsJ2.Count == 0)
                {
                    endRoundPlayer = player;
                    GameManager.Instance.gameLogic.SetTrigger("EndRound");
                }
            }
            else
            {
                TextViewer.Instance.SetTextTemporary("Wrong card !", Color.red);
                int[] arrayPos = new int[] { 1, 2, 3, 4, 5, 6 };
                if (player == Card.Owner.Player1)
                {
                    if (cardsJ1.Count < 6)
                    {
                        foreach (Card c in cardsJ1)
                        {
                            switch (c.position)
                            {
                                case Card.Position.Player1_Slot1:
                                    arrayPos[0] = -1;
                                    break;
                                case Card.Position.Player1_Slot2:
                                    arrayPos[1] = -1;
                                    break;
                                case Card.Position.Player1_Slot3:
                                    arrayPos[2] = -1;
                                    break;
                                case Card.Position.Player1_Slot4:
                                    arrayPos[3] = -1;
                                    break;
                                case Card.Position.Player1_Slot5:
                                    arrayPos[4] = -1;
                                    break;
                                case Card.Position.Player1_Slot6:
                                    arrayPos[5] = -1;
                                    break;
                            }
                        }

                        foreach (int i in arrayPos)
                        {
                            Card.Position p = Card.Position.Player1_Slot6;
                            if (i != -1)
                            {
                                switch (i)
                                {
                                    case (1):
                                        p = Card.Position.Player1_Slot1;
                                        break;
                                    case (2):
                                        p = Card.Position.Player1_Slot2;
                                        break;
                                    case (3):
                                        p = Card.Position.Player1_Slot3;
                                        break;
                                    case (4):
                                        p = Card.Position.Player1_Slot4;
                                        break;
                                    case (5):
                                        p = Card.Position.Player1_Slot5;
                                        break;
                                    case (6):
                                        p = Card.Position.Player1_Slot6;
                                        break;
                                }
                                Card c = Deck.Instance.Draw();
                                c.MoveTo(p);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (cardsJ2.Count < 6)
                    {
                        foreach (Card c in cardsJ2)
                        {
                            switch (c.position)
                            {
                                case Card.Position.Player2_Slot1:
                                    arrayPos[0] = -1;
                                    break;
                                case Card.Position.Player2_Slot2:
                                    arrayPos[1] = -1;
                                    break;
                                case Card.Position.Player2_Slot3:
                                    arrayPos[2] = -1;
                                    break;
                                case Card.Position.Player2_Slot4:
                                    arrayPos[3] = -1;
                                    break;
                                case Card.Position.Player2_Slot5:
                                    arrayPos[4] = -1;
                                    break;
                                case Card.Position.Player2_Slot6:
                                    arrayPos[5] = -1;
                                    break;
                            }
                        }

                        foreach (int i in arrayPos)
                        {
                            Card.Position p = Card.Position.Player2_Slot6;
                            if (i != -1)
                            {
                                switch (i)
                                {
                                    case (1):
                                        p = Card.Position.Player2_Slot1;
                                        break;
                                    case (2):
                                        p = Card.Position.Player2_Slot2;
                                        break;
                                    case (3):
                                        p = Card.Position.Player2_Slot3;
                                        break;
                                    case (4):
                                        p = Card.Position.Player2_Slot4;
                                        break;
                                    case (5):
                                        p = Card.Position.Player2_Slot5;
                                        break;
                                    case (6):
                                        p = Card.Position.Player2_Slot6;
                                        break;
                                }
                                Card c = Deck.Instance.Draw();
                                c.MoveTo(p);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetEndTurn(Card.Owner player)
    {
        endRoundPlayer = player;
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
            else
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

    public Card LookCard(int index)
    {
        if (index > cardsJ2.Count) Debug.LogError("Error LookCard index : " + index);
        return cardsJ2[index];
    }
    // Multi player Functions

    //swap the hands positions so the player 2 is still a the bottom of the screen
    public void SwapHandsPosition()
    {
        SwapPosition(GameObject.Find("DeckPosition"), GameObject.Find("DiscardPosition"));

        SwapPosition(GameObject.Find("PlayerHand_Slot1"), GameObject.Find("Player2Hand_Slot1"));
        SwapPosition(GameObject.Find("PlayerHand_Slot2"), GameObject.Find("Player2Hand_Slot2"));
        SwapPosition(GameObject.Find("PlayerHand_Slot3"), GameObject.Find("Player2Hand_Slot3"));
        SwapPosition(GameObject.Find("PlayerHand_Slot4"), GameObject.Find("Player2Hand_Slot4"));
        SwapPosition(GameObject.Find("PlayerHand_Slot5"), GameObject.Find("Player2Hand_Slot5"));
        SwapPosition(GameObject.Find("PlayerHand_Slot6"), GameObject.Find("Player2Hand_Slot6"));

        SwapPosition(GameObject.Find("PlayerChoicePosition"), GameObject.Find("Player2ChoicePosition"));
    }

    public void SwapPosition(GameObject obj1, GameObject obj2)
    {
        Vector3 tmp = obj1.transform.position;
        obj1.transform.position = obj2.transform.position;
        obj2.transform.position = tmp;
    }

    public IEnumerator PrepareMultiplayer()
    {
        yield return new WaitForSeconds(0.5f);

        if (MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player1) GameManager.Instance.namePlayer1 = MultiPlayerController.LocalPlayerInstance.namePlayer;
        else GameManager.Instance.namePlayer2 = MultiPlayerController.LocalPlayerInstance.namePlayer;
        GameManager.Instance.UpdateScoreText();

        MultiPlayerController.LocalPlayerInstance.photonView.RPC("SetPlayerNameText", PhotonTargets.Others, MultiPlayerController.LocalPlayerInstance.namePlayer, MultiPlayerController.LocalPlayerInstance.playerID);
        if (MultiPlayerController.LocalPlayerInstance.playerID == Card.Owner.Player2)
        {
            SwapHandsPosition();
        }
        state.Execute(null);
    }
}
