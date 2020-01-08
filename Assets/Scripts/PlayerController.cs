using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    public static PlayerController LocalPlayerInstance;
    public PhotonView photonView;
    public Card.Owner playerID;
    public string namePlayer;
    // Start is called before the first frame update
    void Start()
    {
        string gamemode = PlayerPrefs.GetString("gamemode");
        if (gamemode.Equals("IA"))
        {
            playerID = Card.Owner.Player1;
            namePlayer = "Player";
            GameManager.Instance.namePlayer1 = "Player";
            GameManager.Instance.namePlayer2 = "IA";
            GameManager.Instance.UpdateScoreText();
        }
        else
        {
            photonView = this.GetComponent<PhotonView>();
            
            if (photonView.isMine)
            {
                LocalPlayerInstance = this;
                string id = PlayerPrefs.GetString("playerID");
                if (id.Equals("player1")) playerID = Card.Owner.Player1;
                else playerID = Card.Owner.Player2;
                namePlayer = PlayerPrefs.GetString("PlayerName");
                Debug.Log("I am "+  id);
            }     
        }
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    GameManager.Instance.CheckTouch(ray, playerID);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                GameManager.Instance.CheckTouch(ray, playerID);
            }
        }
    }

    [PunRPC]
    void SetPlayerNameText(string name, Card.Owner id )
    {
        if (id == Card.Owner.Player1)
        {
            GameManager.Instance.namePlayer1 = name;
        }
        else
        {
            GameManager.Instance.namePlayer2 = name;
        }
        GameManager.Instance.UpdateScoreText();
    }

    [PunRPC]
    void DeckShuffleData(string data)
    {
        Debug.Log("I received the deck : " + data);

        string[] dataSplit = data.Split(',');

        Deck.Instance.stack = new List<GameObject>();
        for (int i = 0; i < dataSplit.Length-1; i++)
        {
            GameObject o = GameObject.Find(dataSplit[i]);
            if(o != null)
            Deck.Instance.stack.Add(o);
        }
        Deck.Instance.receivedDeckEvent = true;
    }

}
