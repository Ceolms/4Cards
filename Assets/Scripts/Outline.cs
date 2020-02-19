using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    public Transform outlineObjectTransform;
    public void StartOutline()
    {
        outlineObjectTransform = this.transform.GetChild(0);
        SetActive(false);
    }

    public void SetActive(bool b)
    {
        foreach (Transform tr in outlineObjectTransform)
        {
            if (tr.tag.Equals("OutlineMesh"))
            {
                tr.gameObject.SetActive(b);
            }
        }
    }
}
