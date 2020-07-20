using ChrisTutorials.Persistent;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsLink : MonoBehaviour
{
    // con fig vars //
    [Header("Sound Settings", order = 0)]
    [Space(5, order = 1)]
    [Header("Master Volume", order = 2)]
    public Scrollbar masterScrollbar;
    public TextMeshProUGUI masterValueText;
    public int masterValueDefault = 100;

    [Header("Sound Volume")]
    public Scrollbar soundScrollbar;
    public TextMeshProUGUI soundValueText;
    public int soundValueDefault = 90;

    [Header("Music Volume")]
    public Scrollbar musicScrollbar;
    public TextMeshProUGUI musicValueText;
    public int musicValueDefault = 90;

    // state vars //
    GUIControll guiCon;

    public enum TestEnum { Master, Sound, Music }

    #region Initialization

    void Awake()
    {
        // Sanity Check //
        foreach (GUIControll guiTest in FindObjectsOfType<GUIControll>())
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
    }

    private void Start()
    {
        LinkSettings();
    }

    private void LinkSettings()
    {
        LinkAudio();
    }

    #region Audio

    private void LinkAudio()
    {
        guiCon.opManager.initializing = true;

        guiCon.opManager.masterScrollbar = masterScrollbar;
        guiCon.opManager.soundScrollbar = soundScrollbar;
        guiCon.opManager.musicScrollbar = musicScrollbar;

        guiCon.opManager.masterValueText = masterValueText;
        guiCon.opManager.soundValueText = soundValueText;
        guiCon.opManager.musicValueText = musicValueText;

        guiCon.opManager.SetAudioOptions();

        guiCon.opManager.StartCoroutine(guiCon.opManager.EndInitialization());
    }

    #endregion

    #endregion

    #region Change Setting

    public void UpdateSoundLevels(string _switch)
    {
        if (guiCon.opManager.initializing) { return; }

        int sliderValue = 0;
        AudioManager.AudioChannel channel = AudioManager.AudioChannel.Master;
        switch (_switch)
        {
            case "Master":
                sliderValue = (int)(masterScrollbar.value * 100);
                masterValueText.text = sliderValue + " / 100";
                channel = AudioManager.AudioChannel.Master;
                break;
            case "Sound":
                sliderValue = (int)(soundScrollbar.value * 100);
                soundValueText.text = sliderValue + " / 100";
                channel = AudioManager.AudioChannel.Sound;
                break;
            case "Music":
                sliderValue = (int)(musicScrollbar.value * 100);
                musicValueText.text = sliderValue + " / 100";
                channel = AudioManager.AudioChannel.Music;
                break;
        }

        AudioManager.Instance.SetVolume(channel, sliderValue);

        guiCon.opManager.settingChanged = true;
    }

    #endregion

    #region Use Final Button

    public void ApplySettings()
    {
        guiCon.opManager.SaveSettings();
    }

    public void LoadDefaultSettings()
    {
        masterScrollbar.value = (float)masterValueDefault / 100;
        soundScrollbar.value = (float)soundValueDefault / 100;
        musicScrollbar.value = (float)musicValueDefault / 100;
    }

    public void ExitOptinos()
    {

    }

    #endregion
}
