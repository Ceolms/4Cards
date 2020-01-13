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
        Debug.Log("I have to shake the card : " + name);
        GameObject.Find(name).GetComponent<Card>().Shake();
    }
    [PunRPC]

    void MoveTo(string name,Card.Position pos)
    {
        GameObject.Find("name").GetComponent<Card>().MoveTo(pos);
    }

}
