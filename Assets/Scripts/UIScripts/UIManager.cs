using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject mainPanel;
    public GameObject soloPanel;
    public GameObject multiPanel;
    public GameObject settingsPanel;
    public GameObject bonusPanel;
    public GameObject searchingPanel;
    //public NetworkManager manager;
    public GameObject buttonMatchPrefab;
    private bool isHost;
    private bool searching;
    private bool cooldownSearch;
    void Start()
    {
        Instance = this;
        mainPanel.GetComponent<UIMover>().Show();
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
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                CheckTouchUI(ray);
            }
        }

        if (searching && !cooldownSearch)
        {
            cooldownSearch = true;
            StartCoroutine(CoolDownResearch());
        }
    }

    private IEnumerator CoolDownResearch()
    {
        Search();
        yield return new WaitForSeconds(2);
        cooldownSearch = false;
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
                case ("ButtonBonus"):
                    Bonus(buttonHit);
                    break;
                case ("ButtonEasy"):
                    StartSolo("Easy");
                    break;
                case ("ButtonNormal"):
                    StartSolo("Normal");
                    break;
                case ("ButtonUnfair"):
                    StartSolo("Unfair");
                    break;
                case ("ButtonCreate"):
                    Create();
                    break;
                case ("ButtonJoin"):
                    Join();
                    break;
                case ("ButtonPlay"):
                    JoinRandom();
                    break;
                case ("ButtonAbort"):
                    Abort();
                    break;
            }
        }
    }

    private void Solo(GameObject button)
    {
        soloPanel.GetComponent<UIMover>().Show();
        mainPanel.GetComponent<UIMover>().Hide();

    }
    private void Multi(GameObject button)
    {

        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, "1.0");
        multiPanel.GetComponent<UIMover>().Show();
        mainPanel.GetComponent<UIMover>().Hide();
        GameObject.Find("InputField").GetComponent<InputField>().text = PlayerPrefs.GetString("PlayerName");
    }
    private void Settings(GameObject button)
    {
        settingsPanel.GetComponent<UIMover>().Show();
        mainPanel.GetComponent<UIMover>().Hide();
    }

    private void Bonus(GameObject button)
    {
        bonusPanel.GetComponent<UIMover>().Show();
        mainPanel.GetComponent<UIMover>().Hide();
    }
    private void Return(GameObject button)
    {
         if (PhotonNetwork.connected) PhotonNetwork.Disconnect();

        mainPanel.GetComponent<UIMover>().Show();
        soloPanel.GetComponent<UIMover>().Hide();
        multiPanel.GetComponent<UIMover>().Hide();
        settingsPanel.GetComponent<UIMover>().Hide();
        bonusPanel.GetComponent<UIMover>().Hide();

        if(searching)
        {
            searching = false;

            GameObject listPanel = GameObject.Find("ListMatchsPanel");
            foreach (Transform child in listPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            searchingPanel.SetActive(false);
        }
    }

    private void StartSolo(string difficulty)
    {
        PlayerPrefs.SetString("difficulty", difficulty);
        PlayerPrefs.SetString("gamemode", "IA");

        GameObject go = GameObject.Find("MultiplayerManager");
        Destroy(go);
        SceneManager.LoadScene("Game");
    }
    private void Abort()
    {
        searching = false;

        GameObject listPanel = GameObject.Find("ListMatchsPanel");
        foreach (Transform child in listPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        searchingPanel.SetActive(false);
    }

    // Create Button -Create a Match and wait for it to be created
    private void Create()
    {
        string nom = GameObject.Find("InputField").GetComponent<InputField>().text;
        Debug.Log("Creating room : '" + nom + "'");
        if (!string.IsNullOrEmpty(nom))
        {
            isHost = true;
            PhotonNetwork.CreateRoom(nom, new RoomOptions() { MaxPlayers = 2 }, null);
        }
    }
    //Join Button - List alls matches available
    private void Join()
    {
        searchingPanel.SetActive(true);
        string nom = GameObject.Find("InputField").GetComponent<InputField>().text;
        PlayerPrefs.SetString("PlayerName", nom);
        if (nom.Equals("Thor")) PlayerPrefs.SetInt("ThorAchievement", 1);
        searching = true;
    }

    public void JoinRandom()
    {
        isHost = false;
        string nom = GameObject.Find("InputField").GetComponent<InputField>().text;
        PlayerPrefs.SetString("PlayerName", nom);
        if (nom.Equals("Thor")) PlayerPrefs.SetInt("ThorAchievement", 1);
        PlayerPrefs.SetString("playerID", "player2");
        PhotonNetwork.JoinRandomRoom(null, 2);
    }
    private void Search()
    {
        float offsetX = -2.4f;
        float offsetY = 3.4f;

        GameObject listPanel = GameObject.Find("ListMatchsPanel");
        foreach (Transform child in listPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        List<RoomInfo> rooms = new List<RoomInfo>(PhotonNetwork.GetRoomList());
        foreach (RoomInfo room in rooms)
        {
            GameObject btnMatch = Instantiate(buttonMatchPrefab);
            btnMatch.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            btnMatch.transform.SetParent(listPanel.transform);
            btnMatch.transform.localPosition = new Vector3(offsetX, offsetY, btnMatch.transform.position.x);
            offsetY -= 1;
            btnMatch.GetComponent<OnClickMatchJoin>().SetButtonMatch(room.Name);
        }
    }

    public void JoinMatch(string nom)
    {
        Debug.Log("Joining : '" + nom +"'");
        PhotonNetwork.JoinRoom("GAME");
    }

    //call backs ---------

    private void OnConnectedToMaster()
    {
         PhotonNetwork.JoinLobby(TypedLobby.Default);
         Debug.Log("Connected to Master");
    }
    private void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined");
    }

    private void OnJoinedRoom()
    {  
        string nom = GameObject.Find("InputField").GetComponent<InputField>().text;
        Debug.Log("room name:'" + nom+"'");
        PlayerPrefs.SetString("PlayerName", nom);
        if (nom.Equals("Thor")) PlayerPrefs.SetInt("ThorAchievement", 1);
        PlayerPrefs.SetString("gamemode", "multiplayer");
        if (isHost) PlayerPrefs.SetString("playerID", "player1");
        else PlayerPrefs.SetString("playerID", "player2");
        PhotonNetwork.LoadLevel("Game");
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("No random room found");
        string nom = GameObject.Find("InputField").GetComponent<InputField>().text;
        PlayerPrefs.SetString("PlayerName", nom);
        if (nom.Equals("Thor")) PlayerPrefs.SetInt("ThorAchievement", 1);
        isHost = true;
        PlayerPrefs.SetString("playerID", "player1");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, null);
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon");
    }

    private void OnFailedToConnectToPhoton()
    {
        Debug.Log("Error connecting to Photon");

        //TODO UI error connexion
    }
}
