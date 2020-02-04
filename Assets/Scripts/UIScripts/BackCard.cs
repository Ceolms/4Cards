using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackCard : MonoBehaviour
{
    private SpriteRenderer renderer;
    public Sprite hiddenSprite;
    public Sprite backSprite;
    public int scoreToUnlock;
    public int id;
    public Text text;
    [HideInInspector]
    public bool isUnlocked;
    [HideInInspector]
    public bool isSelected;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        GetComponent<Outline>().StartOutline();
        int wins = 0;
        wins = PlayerPrefs.GetInt("winCount");
        if (wins >= scoreToUnlock)
        {
            isUnlocked = true;
            renderer.sprite = backSprite;
            text.text = "Unlocked !";
            if (id == 1 ) text.text = "Default";
        }
        else renderer.sprite = hiddenSprite;

        if (this.id == 6 && PlayerPrefs.GetInt("ThorAchievement") == 1)
        {
            isUnlocked = true;
            renderer.sprite = backSprite;
            text.text = "Unlocked !";
        }
        else if (this.id == 6)
        {
            renderer.sprite = hiddenSprite;
        }
    }

}
