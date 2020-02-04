using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomDebug : MonoBehaviour
{
    public static CustomDebug Instance;
    private Text text;


    private void Start()
    {
        Instance = this;
        text = GameObject.Find("DebugText").GetComponent<Text>();
    }

    public void Log(string s)
    {
        text.text += s + "\n";
    }

    public void SetText(string s)
    {
        text.text = s + "\n";
    }
}
