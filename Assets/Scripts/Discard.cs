using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discard : MonoBehaviour
{
    public List<GameObject> stack;
    public static System.Random rnd = new System.Random();
    public static Discard Instance;
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        ps = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        ShowParticles(false);
        stack = new List<GameObject>();
    }

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
    }

    Card Draw()
    {
        GameObject obj = RemoveAndGet<GameObject>(this.stack, 0);
        return obj.GetComponent<Card>();
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
