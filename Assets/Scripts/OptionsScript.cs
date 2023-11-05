using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class OptionsScript : MonoBehaviour {
    [Header("Options")]
    public bool fullscreen;
    public string resolution;
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    [Header("Audio Objects")]
    [SerializeField] private AudioSource[] musicSources;
    public List<AudioSource> sfxSources = new List<AudioSource>();
    [SerializeField] private AudioSource sfxTestAudioSource;

    [Header("UI Objects")]
    [SerializeField] private TMP_Text masterVolumeSliderText;
    public Slider masterVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeSliderText;
    public Slider musicVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeSliderText;
    public Slider sfxVolumeSlider;

    private void Start() {
        fullscreen = Screen.fullScreen;
        resolution = Screen.currentResolution.ToString();

        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 100);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 100);

        masterVolumeSliderText.text = masterVolume.ToString() + "%";
        masterVolumeSlider.value = masterVolume;
        musicVolumeSliderText.text = musicVolume.ToString() + "%";
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSliderText.text = sfxVolume.ToString() + "%";
        sfxVolumeSlider.value = sfxVolume;

        foreach (var musicAudio in musicSources) {
            musicAudio.volume = (musicVolume * (masterVolumeSlider.value / 100)) / 100;
        }

        foreach (var sfxAudio in sfxSources) {
            sfxAudio.volume = (sfxVolume * (masterVolumeSlider.value / 100)) / 100;
        }
    }

    public void SetFullScreen(bool toggleValue) {
        Screen.fullScreen = toggleValue;
        fullscreen = toggleValue;
    }

    public void SetResolution(int resolutionNumber) {
        switch (resolutionNumber) { 
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                resolution = "1920x1080";
                break;
            case 1:
                Screen.SetResolution(1366, 768, Screen.fullScreen);
                resolution = "1366x768";
                break;
            case 2:
                Screen.SetResolution(1536, 865, Screen.fullScreen);
                resolution = "1536x865";
                break;
            case 3:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                resolution = "1280x720";
                break;
            case 4:
                Screen.SetResolution(1440, 900, Screen.fullScreen);
                resolution = "1440x900";
                break;
            case 5:
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                resolution = "1600x900";
                break;
        }
    }

    public void MasterSliderChanged(float value) {
        masterVolumeSliderText.text = value.ToString() + "%";
        masterVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);

        foreach (var musicAudio in musicSources) {
            musicAudio.volume = (musicVolumeSlider.value * (masterVolumeSlider.value / 100)) / 100;
            musicVolume = musicAudio.volume * 100;
        }

        foreach (var sfxAudio in sfxSources) {
            sfxAudio.volume = (sfxVolumeSlider.value * (masterVolumeSlider.value / 100)) / 100;
            sfxVolume = sfxAudio.volume * 100;
        }
    }

    public void MusicSliderChanged(float value) {
        musicVolumeSliderText.text = value.ToString() + "%";
        musicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);

        foreach (var musicAudio in musicSources) {
            musicAudio.volume = (value * (masterVolumeSlider.value / 100)) / 100;
        }
    }

    public void SFXSliderChanged(float value) {
        sfxVolumeSliderText.text = value.ToString() + "%";
        sfxVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);

        foreach (var sfxAudio in sfxSources) {
            sfxAudio.volume = (value * (masterVolumeSlider.value / 100)) / 100;
        }

        if (sfxTestAudioSource.isPlaying == false && sfxVolumeSlider.isActiveAndEnabled == true) {
            sfxTestAudioSource.Play();
        }
    }
}