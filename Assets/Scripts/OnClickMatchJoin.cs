using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class OnClickMatchJoin : MonoBehaviour
{
    private string nomMatch;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() => Join());
    }

    public void SetButtonMatch(string nomMatch)
    {
        this.nomMatch = nomMatch;
        this.GetComponentInChildren<Text>().text = nomMatch;
    }

    private void Join()
    {
        if(this.nomMatch != null) UIManager.Instance.JoinMatch(nomMatch);
    }
}
