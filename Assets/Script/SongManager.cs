using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    [SerializeField] private Slider mainSlide;
    [SerializeField] private Slider musicSlide;
    [SerializeField] private Slider sfxSlide;

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    { 
        audioMixer.GetFloat("MasterVolume",out float mainVolume);
        mainSlide.value = Mathf.Exp(mainVolume/20f) ;
        audioMixer.GetFloat("MusicVolume",out float musicVolume);
        musicSlide.value = Mathf.Exp(musicVolume/20f);
        audioMixer.GetFloat("SFXVolume",out float sfxVolume);
        sfxSlide.value = Mathf.Exp(sfxVolume/20f);
    }

    public void OnMainVolumeChanged()
    {
        audioMixer.SetFloat("MasterVolume",Mathf.Log(mainSlide.value) * 20f );
    }

    public void OnMusicVolumeChanged()
    {
        audioMixer.SetFloat("MusicVolume",Mathf.Log( musicSlide.value) * 20f);
    }

    public void OnSFXVolumeChanged()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log(sfxSlide.value) * 20f );
    }
}