using ChrisTutorials.Persistent;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    // con fig vars //
    [Header("Sound Settings", order = 0)]
    [Space(5, order = 1)]
    [Header("Master Volume", order = 2)]
    public AudioManager.AudioChannel masterChannel;
    public Scrollbar masterScrollbar;
    public TextMeshProUGUI masterValueText;

    [Header("Sound Volume")]
    public AudioManager.AudioChannel soundChannel;
    public Scrollbar soundScrollbar;
    public TextMeshProUGUI soundValueText;

    [Header("Music Volume")]
    public AudioManager.AudioChannel musicChannel;
    public Scrollbar musicScrollbar;
    public TextMeshProUGUI musicValueText;

    // state vars //
    public bool initializing = true;
    public bool settingChanged = false;
    int masterVolume;
    int soundVolume;
    int musicVolume;

    private void Start()
    {
        InitializeSettings();
    }

    #region Initialization

    private void InitializeSettings()
    {
        InitializeAudio();
        StartCoroutine(EndInitialization());
    }

    #region Audio

    private void InitializeAudio()
    {
        masterVolume = PlayerPrefs.GetInt("MasterVolume", 100);
        soundVolume = PlayerPrefs.GetInt("SoundVolume", 100);
        musicVolume = PlayerPrefs.GetInt("MusiVolume", 100);
        
    }

    public void SetAudioOptions()
    {
        masterScrollbar.value = (float)masterVolume / 100;
        soundScrollbar.value = (float)soundVolume / 100;
        musicScrollbar.value = (float)musicVolume / 100;

        masterValueText.text = masterVolume + " / 100";
        soundValueText.text = masterVolume + " / 100";
        musicValueText.text = masterVolume + " / 100";
    }

    public IEnumerator EndInitialization()
    {
        yield return new WaitForSeconds(0.5f);
        initializing = false;
    }

    #endregion

    #endregion

    #region Change Settings

    public void UpdateSoundLevels(AudioManager.AudioChannel channel)
    {
        Scrollbar scrollbar = masterScrollbar;
        TextMeshProUGUI valueText = masterValueText;

        switch (channel)
        {
            case AudioManager.AudioChannel.Master:
                scrollbar = masterScrollbar;
                valueText = masterValueText;
                break;
            case AudioManager.AudioChannel.Sound:
                scrollbar = soundScrollbar;
                valueText = soundValueText;
                break;
            case AudioManager.AudioChannel.Music:
                scrollbar = musicScrollbar;
                valueText = musicValueText;
                break;
        }

        int sliderValue = (int)(scrollbar.value * 100);

        AudioManager.Instance.SetVolume(channel, sliderValue);

        valueText.text = sliderValue + " / 100";
    }

    #endregion

    #region Save Settings

    public void SaveSettings()
    {
        SaveAudioSettings();
        settingChanged = false;
    }

    private void SaveAudioSettings()
    {
        PlayerPrefs.SetInt("MasterVolume", (int)(masterScrollbar.value * 100));
        PlayerPrefs.SetInt("SoundVolume", (int)(soundScrollbar.value * 100));
        PlayerPrefs.SetInt("MusicVolume", (int)(musicScrollbar.value * 100));
    }

    #endregion
}