using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField]
    AudioMixer master_mixer;

    [SerializeField]
    MySounds[] sounds;

    public void PlaySound(string name)
    {
        Debug.Log("Estás aquí");
        foreach (MySounds s in sounds)
        {
            if (s.name == name)
            {
                GetComponent<AudioSource>().clip = s.clip;
                GetComponent<AudioSource>().volume = s.volume;
                GetComponent<AudioSource>().loop = s.loop;

                if (s.randomPitch)
                {
                    GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.1f, 3.0f);
                }
                else
                {
                    GetComponent<AudioSource>().pitch = s.pitch;
                }
                break;

            }
        }
        MySounds sfx = Array.Find(sounds, sound => sound.name == name);
        GetComponent<AudioSource>().Play();
    }
}
