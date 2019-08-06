using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextViewer : MonoBehaviour
{
    [SerializeField]
    public static TextViewer Instance;
    private UnityEngine.UI.Text textLine;
    private Color defaultColor = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        textLine = GameObject.Find("TextLine").GetComponent<UnityEngine.UI.Text>();
        textLine.color = defaultColor;
    }

    public void SetText(string s)
    {
        textLine.enabled = true;
        textLine.text = s;
        textLine.color = defaultColor;
    }

    public void  SetText(string s,Color c)
    {
        textLine.enabled = true;
        textLine.text = s;
        textLine.color = c;
    }

    public void HideText()
    {
        textLine.enabled = false;
    }
}
