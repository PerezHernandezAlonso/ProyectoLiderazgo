using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider efectosSlider;
    [SerializeField] Slider musicaSlider;
    // Start is called before the first frame update
    const string MIXER_MASTER = "Master_escena1";
    const string MIXER_EFECTOS = "Efectos_escena1";
    const string MIXER_MUSICA = "Musica_escena1";
    void Awake()
    {
        masterSlider.onValueChanged.AddListener(setMasterVolume);
        efectosSlider.onValueChanged.AddListener(setEfectosVolume);
        musicaSlider.onValueChanged.AddListener(setMusicaVolume);
    }

    void setMasterVolume(float value)
    {
        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
    }

    void setEfectosVolume(float value)
    {
        mixer.SetFloat(MIXER_EFECTOS, Mathf.Log10(value) * 20);
    }

    void setMusicaVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSICA, Mathf.Log10(value) * 20);
    }
}
