using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject mainPanel;
    public GameObject soloPanel;
    public GameObject multiPanel;
    public GameObject settingsPanel;
    public GameObject searchingPanel;
    public GameObject loadingCard;

    private bool searching;
    void Start()
    {
        Instance = this;
        soloPanel.SetActive(false);
        multiPanel.SetActive(false);
        settingsPanel.SetActive(false);
        searchingPanel.SetActive(false);
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
                    CheckTouchUI(ray);
                }
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (Input.GetMouseButtonDown(0))
            { 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                CheckTouchUI(ray);
            }
        }
        if(searchingPanel.activeSelf)
        {
            loadingCard.transform.Rotate(new Vector3(0, 10, 0));
        }

        if(searching)
        {

        }
    }


    private void CheckTouchUI(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            
            GameObject buttonHit = hit.collider.gameObject;
            switch (buttonHit.name)
            {
                case ("ButtonReturn"):
                    Return(buttonHit);
                    break;
                case ("ButtonSolo"):
                    Solo(buttonHit);
                    break;
                case ("ButtonMulti"):
                    Multi(buttonHit);
                    break;
                case ("ButtonSettings"):
                    Settings(buttonHit);
                    break;
                case ("ButtonEasy"):
                    StartSolo("Easy");
                    break;
                case ("ButtonMedium"):
                    StartSolo("Medium");
                    break;
                case ("ButtonOverkill"):
                    StartSolo("Overkill");
                    break;
                case ("ButtonSearch"):
                    Search();
                    break;
                case ("ButtonAbort"):
                    searching = false;
                    searchingPanel.SetActive(false);
                    break;
            }
        }
    }

    private void Solo(GameObject button)
    {
        soloPanel.SetActive(true);
        mainPanel.SetActive(false);
        multiPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    private void Multi(GameObject button)
    {
        multiPanel.SetActive(true);
        mainPanel.SetActive(false);
        soloPanel.SetActive(false); 
        settingsPanel.SetActive(false);
    }
    private void Settings(GameObject button)
    {
        settingsPanel.SetActive(true);
        mainPanel.SetActive(false);
        soloPanel.SetActive(false);
        multiPanel.SetActive(false); 
    }
    private void Return(GameObject button)
    {
        mainPanel.SetActive(true);
        soloPanel.SetActive(false);
        multiPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    private void StartSolo(string difficulty)
    {
        PlayerPrefs.SetString("difficulty", difficulty);
        SceneManager.LoadScene("Game");
    }

    private void Search()
    {
        searchingPanel.SetActive(true);
        searching = true;
    }
}
