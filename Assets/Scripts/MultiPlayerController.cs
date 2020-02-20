using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        else if (photonView.isMine)
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
    void SetAnimatorBool(string s, bool b)
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
    void DrawCard(Card.Position pos, Card.Owner player)
    {
        if (player == Card.Owner.Player1)
        {
            if (pos == Card.Position.Deck)
            {
                Card c = Deck.Instance.Draw();
                c.MoveTo(Card.Position.Player1Choice);

            }
            else
            {
                Card c = Discard.Instance.Draw();
                c.SetHidden(true);
                c.MoveTo(Card.Position.Player1Choice);
            }

        }
        else
        {
            if (pos == Card.Position.Deck)
            {
                Card c = Deck.Instance.Draw();
                c.MoveTo(Card.Position.Player2Choice);

            }
            else
            {
                Card c = Discard.Instance.Draw();
                c.SetHidden(true);
                c.MoveTo(Card.Position.Player2Choice);
            }
        }
        GameManager.Instance.ChangePhase();
    }


    [PunRPC]
    void DiscardCard(Card.Position pos, Card.Owner player)
    {
        Card c = GameManager.Instance.FindByPosition(pos);

        c.MoveTo(Card.Position.Discard);
        c.SetHidden(false);
        if (player == Card.Owner.Player1)
        {
            GameManager.Instance.cardsJ1.Remove(c);
        }
        else
        {
            GameManager.Instance.cardsJ2.Remove(c);
        }
        GameManager.Instance.ChangePhaseLong();
    }

    [PunRPC]
    void WrongCard(Card.Position pos, Card.Owner player)
    {
        Card c = Deck.Instance.Draw();
        c.MoveTo(pos);
    }


    [PunRPC]
    void DeleteCard(Card.Position pos, Card.Owner player)
    {
        Card c = GameManager.Instance.FindByPosition(pos);

        c.MoveTo(Card.Position.Discard);
        c.SetHidden(false);
        if (player == Card.Owner.Player1)
        {
            GameManager.Instance.cardsJ1.Remove(c);
        }
        else
        {
            GameManager.Instance.cardsJ2.Remove(c);
        }
    }


    [PunRPC]
    void MoveCardToHand(Card.Position pos, Card.Owner player)
    {
        if(player == Card.Owner.Player1)
        {
            Card c = GameManager.Instance.FindByPosition(Card.Position.Player1Choice);
            c.MoveTo(pos);
        }
        else
        {
            Card c = GameManager.Instance.FindByPosition(Card.Position.Player2Choice);
            c.MoveTo(pos);
        }
    }


    [PunRPC]
    void ExchangeCards(Card.Position positionP1,Card.Position positionP2)
    {
        Card cardP1 = GameManager.Instance.FindByPosition(positionP1);
        Card cardP2 = GameManager.Instance.FindByPosition(positionP2);

        GameManager.Instance.cardsJ1.Remove(cardP1);
        GameManager.Instance.cardsJ2.Remove(cardP2);

        cardP1.MoveTo(positionP2);
        cardP2.MoveTo(positionP1);
        GameManager.Instance.ResumeGame();
    }



    [PunRPC]
    void EndRound(Card.Owner player)
    {
        GameManager.Instance.endRoundPlayer = player;
        TextViewer.Instance.SetTextTemporary("END ROUND", Color.red, 1.8f);
    }

    [PunRPC]
    void NewRound()
    {
        GameManager.Instance.gameLogic.SetTrigger("NewRoundStart");
    }

    [PunRPC]
    void PauseAnimator()
    {
        GameManager.Instance.PauseGame();
        TextViewer.Instance.SetText("Opponent is thinking");
    }

    [PunRPC]
    void ResumeAnimator()
    {
        GameManager.Instance.ResumeGame();
    }


    [PunRPC]
    void Exit()
    {
        PhotonNetwork.LeaveRoom();
    }
    void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }
}
