using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : MonoBehaviour
{
    private const float speed = 30;
    private bool onScreen = false;
    public bool isMainScreen;
    private void Update()
    {

        if (!isMainScreen)
        {

            if (onScreen)
            {
                if (this.transform.localPosition.x < 0f) this.transform.localPosition = Vector3.zero;
                if (this.transform.localPosition.x > 0)
                {
                    //move left
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x - (speed * Time.deltaTime), 0, 0);
                }
            }
            else
            {
                if (this.transform.localPosition.x > 14f) this.transform.localPosition = new Vector3(14, 0, 0);
                if (this.transform.localPosition.x < 14f)
                {
                    //move right
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x + (speed * Time.deltaTime), 0, 0);
                }
            }
        }
        else
        {
            if (onScreen)
            {
                if (this.transform.localPosition.x > 0f) this.transform.localPosition = Vector3.zero;
                if (this.transform.localPosition.x < 0f)
                {
                    //move left
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x + (speed * Time.deltaTime), 0, 0);
                }
            }
            else
            {
                if (this.transform.localPosition.x < -14f) this.transform.localPosition = new Vector3(-14, 0, 0);
                if (this.transform.localPosition.x > -14f)
                {
                    //move right
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x - (speed * Time.deltaTime), 0, 0);
                }
            }
        }

    }

    public void Show()
    {
        onScreen = true;
    }

    public void Hide()
    {
        onScreen = false;
    }


}
