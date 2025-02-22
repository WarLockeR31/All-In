using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        SetStartValues();
        gameObject.SetActive(false);
    }

    public void SetStartValues()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0);

        audioMixer.SetFloat("MusicVolume", musicSlider.value);
        audioMixer.SetFloat("SFXVolume", sfxSlider.value);
    }

    // Метод для изменения громкости музыки
    public void SetMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    // Метод для изменения громкости звуковых эффектов
    public void SetSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }
}
