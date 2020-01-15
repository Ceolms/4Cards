using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiPlayerController : PlayerController
{
    public static MultiPlayerController LocalPlayerInstance;
    public PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        photonView = this.GetComponent<PhotonView>();

        if (photonView.isMine)
        {
            LocalPlayerInstance = this;
            string id = PlayerPrefs.GetString("playerID");
            if (id.Equals("player1")) playerID = Card.Owner.Player1;
            else playerID = Card.Owner.Player2;
            namePlayer = PlayerPrefs.GetString("PlayerName");
        }
    }


    void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer && photonView.isMine)
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
        else if(photonView.isMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                GameManager.Instance.CheckTouch(ray, playerID);
            }
        }
    }

    [PunRPC]
    void SetPlayerNameText(string name, Card.Owner id)
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
        Deck.Instance.deckData = data;
        Deck.Instance.receivedDeckEvent = true;
    }

    [PunRPC]
    void SetAnimatorBool(string s,bool b)
    {
        GameManager.Instance.gameLogic.SetBool(s, b);
    }

    [PunRPC]
    void SetAnimatorTrigger(string s)
    {
        GameManager.Instance.gameLogic.SetTrigger(s);
    }

    [PunRPC]
    void ShakeCard(string name)
    {
        GameObject.Find(name).GetComponent<Card>().Shake();
    }

    [PunRPC]
    void MoveCard(string name,Card.Position pos)
    {
        Card c = GameObject.Find("name").GetComponent<Card>();
       c.MoveTo(pos);

        if(LocalPlayerInstance.playerID == Card.Owner.Player1 && GameManager.Instance.state is P2Draw)
        {
           
        }
        else if (LocalPlayerInstance.playerID == Card.Owner.Player2 && GameManager.Instance.state is P1Draw)
        {

        }
    }

    [PunRPC]
    void Exit()
    {
        //TODO EXIT other player
    }

}
