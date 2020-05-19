﻿using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    public bool loop;

    public bool isMusic = false;

    [HideInInspector]
    public AudioSource source;
}