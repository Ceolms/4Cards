using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    public static PlayerController LocalPlayerInstance;
    private PhotonView photonView;
    public Card.Owner playerID;

    // Start is called before the first frame update
    void Start()
    {
        string gamemode = PlayerPrefs.GetString("gamemode");
        if (gamemode.Equals("IA"))
        {
            playerID = Card.Owner.Player1;
        }
        else
        {
            photonView = this.GetComponent<PhotonView>();
            string id = PlayerPrefs.GetString("PlayerID");
            if (id.Equals("player1"))  playerID = Card.Owner.Player1;
            else playerID = Card.Owner.Player2;

            if (photonView.isMine)
            {
                LocalPlayerInstance = this;
            }
        }
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
            GameManager.Instance.namePlayer1 = name;
        else GameManager.Instance.namePlayer2 = name;
        GameManager.Instance.UpdateScoreText();
    }
}
