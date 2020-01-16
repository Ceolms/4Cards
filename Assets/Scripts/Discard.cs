using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discard : MonoBehaviour
{
    public List<Card> stack;
    public static System.Random rnd = new System.Random();
    public static Discard Instance;
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        ps = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        ShowParticles(false);
        stack = new List<Card>();
    }
    /*
    public void UpdatePosition()
    {
        if (stack.Count > 0)
        {
            stack[0].GetComponent<Card>().SetFront(true);

            for (int i = 1; i < stack.Count; i++)
            {
                stack[i].GetComponent<Card>().SetFront(false);
            }
        }
    }*/

    public Card Draw()
    {
        Card obj = RemoveAndGet<Card>(this.stack, 0);
      //  UpdatePosition();
        return obj;
    }

    private static T RemoveAndGet<T>(IList<T> list, int index)
    {
        lock (list)
        {
            T value = list[index];
            list.RemoveAt(index);
            return value;
        }
    }

    public void ShowParticles(bool b)
    {
        var main = ps.main;
        if (b)
        {
            ps.Simulate(4f);
            ps.Play();
        }
        else
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
