using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackCardSelector : MonoBehaviour
{
    int currentId;
    BackCard currentBackCard;
    public Text text;
    void Start()
    {
        currentId = PlayerPrefs.GetInt("spriteIndex");
        if (currentId == 0)
        {
            PlayerPrefs.SetInt("spriteIndex", 1);
            currentId = 1;
            currentBackCard = GameObject.Find("Back_" + 1).GetComponent<BackCard>();
        }
        else
        {
            currentBackCard = GameObject.Find("Back_" + currentId).GetComponent<BackCard>();
        }
        currentBackCard.GetComponent<Outline>().SetActive(true);
        int wons = PlayerPrefs.GetInt("winCount");
        text.text = "Number of games won : " + wons;
    }

    void Update()
    {
        RaycastHit hit;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    if (Physics.Raycast(ray, out hit) && hit.transform.tag.Equals("BackCard")) ClickCard(hit.transform.gameObject);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit) && hit.transform.tag.Equals("BackCard")) ClickCard(hit.transform.gameObject);

            }
        }
    }

    private void ClickCard(GameObject go)
    {
        BackCard bc = go.GetComponent<BackCard>();

        if (bc.isUnlocked)
        {
            Debug.Log(bc.name + " , unlocked");
            currentId = bc.id;
            PlayerPrefs.SetInt("spriteIndex", bc.id);
            currentBackCard.GetComponent<Outline>().SetActive(false);
            currentBackCard = GameObject.Find("Back_" + currentId).GetComponent<BackCard>();
            currentBackCard.GetComponent<Outline>().SetActive(true);
        }
        else Debug.Log(bc.name + " , locked");
    }
}
