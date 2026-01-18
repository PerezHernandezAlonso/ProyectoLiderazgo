using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MySounds
{
    public string name;
    public AudioClip clip;
    public bool randomPitch = false;
    [Range(0.1f, 3f)]
    public float pitch;
    [Range(0f, 1f)]
    public float volume;

    public bool loop;
}
