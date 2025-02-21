using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Загрузка сохранённых настроек
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0);
    }

    // Метод для изменения громкости музыки
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    // Метод для изменения громкости звуковых эффектов
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
