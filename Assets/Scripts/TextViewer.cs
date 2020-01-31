using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextViewer : MonoBehaviour
{
    [SerializeField]
    public static TextViewer Instance;
    private string oldLine;
    private UnityEngine.UI.Text textLine;
    private Color defaultColor = Color.yellow;

    public Button btn;
    public Sprite imageEndTurn;
    public Sprite imageEndTurn_Push;
    public Sprite imageNewRound;
    public Sprite imageNewRound_Push;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        textLine = GameObject.Find("TextLine").GetComponent<UnityEngine.UI.Text>();
        textLine.color = defaultColor;

    }

    public void SetNewRound()
    {
        btn.GetComponent<Image>().sprite = imageNewRound;
    }
    public void SetEndTurn()
    {
        btn.GetComponent<Image>().sprite = imageEndTurn;
    }

    public void SetText(string s)
    {
        oldLine = s;
        oldLine = textLine.text;
        textLine.enabled = true;
        textLine.text = s;
        textLine.color = defaultColor;
    }

    public void HideText()
    {
        textLine.enabled = false;
    }

    public void SetTextTemporary(string s,float t)
    {
        oldLine = textLine.text;
        textLine.enabled = true;
        textLine.text = s;
        textLine.color = defaultColor;
        StartCoroutine(ResetOldText(t));
    }

    public void SetTextTemporary(string s , Color c,float t)
    {
        oldLine = textLine.text;
        textLine.enabled = true;
        textLine.text = s;
        textLine.color = c;
        StartCoroutine(ResetOldText(t));
    }

    private IEnumerator ResetOldText(float t)
    {
        yield return new WaitForSeconds(t);
        textLine.text = oldLine;
        textLine.color = defaultColor;
    }
}
