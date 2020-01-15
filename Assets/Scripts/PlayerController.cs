using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    public Card.Owner playerID;
    public string namePlayer;
    void Start()
    {
        playerID = Card.Owner.Player1;
        namePlayer = "Player";
        GameManager.Instance.namePlayer1 = "Player";
        GameManager.Instance.namePlayer2 = "IA";
        GameManager.Instance.UpdateScoreText();
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
                Debug.Log("MouseDown");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                GameManager.Instance.CheckTouch(ray, playerID);
            }
        }
    }
}
