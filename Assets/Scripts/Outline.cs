using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    Transform outlineObjectTransform;
    void Start()
    {
        foreach (Transform tr in this.transform)
        {
            if (tr.tag == "OutlineObject")
            {
                outlineObjectTransform = tr;
            }
        }
    }

    public void SetActive(bool b)
    {
            foreach (Transform tr in outlineObjectTransform)
            {
                if (tr.tag == "OutlineMesh")
                {
                    tr.GetComponent<MeshRenderer>().enabled = b;
                }
            }
        
    }
}
