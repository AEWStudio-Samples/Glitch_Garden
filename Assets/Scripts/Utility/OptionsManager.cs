using ChrisTutorials.Persistent;
using System.Collections;
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

    public Slider masterScrollbar;
    public TextMeshProUGUI masterValueText;

    [Header("Sound Volume")]
    public AudioManager.AudioChannel soundChannel;

    public Slider soundScrollbar;
    public TextMeshProUGUI soundValueText;

    [Header("Music Volume")]
    public AudioManager.AudioChannel musicChannel;

    public Slider musicScrollbar;
    public TextMeshProUGUI musicValueText;

    // state vars //

    public bool initializing = true;
    public bool settingChanged = false;
    private int masterVolume;
    private int soundVolume;
    private int musicVolume;

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

    public void RevertSetings()
    {
        initializing = true;

        InitializeSettings();
    }

    #region Audio

    public void GetAudioValues()
    {
        masterVolume = PlayerPrefs.GetInt("MasterVolume", 100);
        soundVolume = PlayerPrefs.GetInt("SoundVolume", 100);
        musicVolume = PlayerPrefs.GetInt("MusicVolume", 100);
    }

    private void InitializeAudio()
    {
        GetAudioValues();

        AudioManager.Instance.SetVolume(masterChannel, masterVolume);
        AudioManager.Instance.SetVolume(soundChannel, soundVolume);
        AudioManager.Instance.SetVolume(musicChannel, musicVolume);
    }

    public void OptMenuInitAudioSettings()
    {
        GetAudioValues();

        masterScrollbar.value = (float)masterVolume / 100;
        soundScrollbar.value = (float)soundVolume / 100;
        musicScrollbar.value = (float)musicVolume / 100;

        masterValueText.text = masterVolume + " / 100";
        soundValueText.text = soundVolume + " / 100";
        musicValueText.text = musicVolume + " / 100";
    }

    public IEnumerator EndInitialization()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        settingChanged = false;
        initializing = false;
    }

    #endregion Audio

    #endregion Initialization

    #region Change Settings

    public void UpdateSoundLevels(AudioManager.AudioChannel channel)
    {
        Slider scrollbar = masterScrollbar;
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

    #endregion Change Settings

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

    #endregion Save Settings
}