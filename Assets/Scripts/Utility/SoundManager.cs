using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // con fig vars //
    [Header("Audio Sources")]
    public AudioSource musicGame = null;
    public AudioSource musicUI = null;
    public AudioSource soundUI = null;

    [Header("Audio Snapshots")]
    public AudioMixerSnapshot inGame = null;
    public AudioMixerSnapshot atMenu = null;

    [Header("Sound Clips")]
    public AudioClip spawnUnit = null;
    public AudioClip upgradeUnit = null;
    public AudioClip clearRound = null;
    public AudioClip gameOver = null;
    public AudioClip[] zombieSpawn = null;
    public AudioClip[] zombieDeath = null;
    public AudioClip[] coinSound = null;

    public void PlayButtonSnd()
    {
        soundUI.Play();
    }

    public void StartGameMusic()
    {
        musicGame.Play();
        SwitchGameUIMusic(inGame);
    }

    public void SwitchGameUIMusic(AudioMixerSnapshot snapshot)
    {
        snapshot.TransitionTo(0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
