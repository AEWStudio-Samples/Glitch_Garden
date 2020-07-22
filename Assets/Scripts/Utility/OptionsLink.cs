using ChrisTutorials.Persistent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsLink : MonoBehaviour
{
    // con fig vars //
    [Header("Sound Settings", order = 0)]
    [Space(5, order = 1)]
    [Header("Master Volume", order = 2)]
    public Slider masterScrollbar;

    public TextMeshProUGUI masterValueText;

    [Header("Sound Volume")]
    public Slider soundScrollbar;

    public TextMeshProUGUI soundValueText;

    [Header("Music Volume")]
    public Slider musicScrollbar;

    public TextMeshProUGUI musicValueText;

    // state vars //
    private GUIControll guiCon;

    public enum TestEnum { Master, Sound, Music }

    #region Initialization

    private void Awake()
    {
        // Sanity Check //
        foreach (GUIControll guiTest in FindObjectsOfType<GUIControll>())
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
    }

    private void PlayButtonSnd()
    {
        guiCon.soundManager.PlayButtonSnd();
    }

    private void Start()
    {
        LinkSettings();
    }

    private void LinkSettings()
    {
        guiCon.opManager.initializing = true;

        LinkAudio();

        guiCon.opManager.StartCoroutine(guiCon.opManager.EndInitialization());
    }

    #region Audio

    private void LinkAudio()
    {
        guiCon.opManager.masterScrollbar = masterScrollbar;
        guiCon.opManager.soundScrollbar = soundScrollbar;
        guiCon.opManager.musicScrollbar = musicScrollbar;

        guiCon.opManager.masterValueText = masterValueText;
        guiCon.opManager.soundValueText = soundValueText;
        guiCon.opManager.musicValueText = musicValueText;

        guiCon.opManager.OptMenuInitAudioSettings();
    }

    #endregion Audio

    #endregion Initialization

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

    #endregion Change Setting

    #region Use Final Button

    public void ApplySettings()
    {
        PlayButtonSnd();
        guiCon.opManager.SaveSettings();
    }

    public void LoadDefaultSettings()
    {
        PlayButtonSnd();
        masterScrollbar.value = 1;
        soundScrollbar.value = 1;
        musicScrollbar.value = 1;
    }

    public void ExitOptions()
    {
        PlayButtonSnd();
        if (guiCon.opManager.settingChanged)
        {
            guiCon.SettingChanged(true);
        }
        else
        {
            guiCon.OptionsMenu(false);
        }
    }

    #endregion Use Final Button
}