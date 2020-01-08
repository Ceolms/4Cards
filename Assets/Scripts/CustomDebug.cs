using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomDebug : MonoBehaviour
{
    public static CustomDebug Instance;
    public Text text;


    private void Start()
    {
        Instance = this;
    }

    public void Log(string s)
    {
        text.text += s + "\n";
    }
}
