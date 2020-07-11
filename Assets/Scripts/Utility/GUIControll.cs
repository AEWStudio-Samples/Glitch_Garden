﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This script is used to control the GUI.
/// </summary>
public class GUIControll : MonoBehaviour
{
    // con fig vars //
    public bool debugging;

    [Header("Ninja Limit")]
    public int maxNinjasBase = 2;
    public int curMaxNinjas;
    public int ninjaCount;

    [Header("Round Information")]
    public TextMeshProUGUI ninjaCounter = null;
    public TextMeshProUGUI zombieCounter = null;
    public TextMeshProUGUI roundCounter = null;
    public int curRound;

    [Header("Counters")]
    public TextMeshProUGUI coinCounter = null;
    public int curCoinCount;
    public int startingCoinCount = 100;
    public PriceManager priceManager = null;

    // state vars //
    TutControll tutCon;
    MasterSpawner spawner;
    public bool loading = false;
    public bool runTut = false;
    bool startGame = false;
    bool newGame = true;
    bool roundClear = false;
    bool quitChk = false;
    bool paused = false;

    // state vars tracking the active scene //
    string curSceneName;

    AtScene curScene = AtScene.Title;

    enum AtScene { MainGame, Start, Title }

    private void CheckInput()
    {
        // Increase the current round by 1 while playing in the editor //
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateCurRound(curRound + 1);
        }
#endif

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (startGame) { StartGame(false); return; }

            if (curScene == AtScene.Start)
            {
                QuitGame(!quitChk);
            }
            else if (curScene == AtScene.MainGame)
            {
                if (paused && quitChk)
                {
                    QuitGame(false);
                    return;
                }

                PauseGame(!paused);
            }
        }
    }

    // Toggles the active GUI //
    IEnumerator ToggleMenu(string menu, bool load)
    {
        if (load)
        {
            yield return SceneManager.LoadSceneAsync(menu, LoadSceneMode.Additive).isDone;
        }
        else
        {
            yield return SceneManager.UnloadSceneAsync(menu).isDone;
        }
    }

    void Awake()
    {
#if UNITY_EDITOR
#else
        debugging = false;
#endif
        loading = true;
        // Make it so there is only one //
        int numGUIControlls = FindObjectsOfType<GUIControll>().Length;

        if (numGUIControlls > 1)
        {
            Destroy(this.gameObject);
        }

        // One to rule them all //
        DontDestroyOnLoad(this.gameObject);

        // Get key components //
        tutCon = GetComponent<TutControll>();

        // What scene are we on //
        StartCoroutine(CheckScene());
    }

    // Check what scene we are on so that we know what we are doing //
    private IEnumerator CheckScene()
    {
        yield return SceneManager.GetActiveScene().isLoaded;

        curSceneName = SceneManager.GetActiveScene().name;

        if (curSceneName == "StartScene")
        {
            curScene = AtScene.Start;
            loading = false;
        }
        else if (curSceneName == "MainGame")
        {
            curScene = AtScene.MainGame;
            spawner = FindObjectOfType<MasterSpawner>();
            StartRound();
            if (newGame)
            {
                ResetCounters();
                priceManager.ResetPrice();
            }
        }

        SetSceneGUI();
    }

    // Set the GUI up for the current scene //
    void SetSceneGUI()
    {
        switch (curScene)
        {
            case AtScene.Start:
                ShowMainMenu();
                break;
            case AtScene.MainGame:
                ShowGameMenu();
                break;
        }
    }

    // Goes to a scene with the given name //
    public void GoToScene(string name)
    {
        startGame = false;
        paused = false;
        quitChk = false;
        loading = true;
        ResetCounters();
        priceManager.ResetPrice();
        SceneManager.LoadScene(name);
        StartCoroutine(CheckScene());
    }

    void Update()
    {
        CheckInput();
        if (curScene == AtScene.MainGame && !loading)
        {
            UpdateButtons();
        }
    }

    // Show the main menu GUI //
    public void ShowMainMenu()
    {
        if (curScene == AtScene.Start)
        {
            StartCoroutine(ToggleMenu("MainMenu", true));
        }
    }

    // Show the GUI for playing the game //
    public void ShowGameMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(ToggleMenu("HUD", true));
    }

    // Begin Code that is triggered by MenuLink.cs //
    // Start the actual game //
    public void StartGame(bool start)
    {
        if (!start && !startGame)
        {
            startGame = true;

            if (curScene == AtScene.Start)
            {
                StartCoroutine(ToggleMenu("MainMenu", false));
            }
            else if (curScene == AtScene.MainGame)
            {
                StartCoroutine(ToggleMenu("PauseMenu", false));
            }

            StartCoroutine(ToggleMenu("StartMenu", true));
        }
        else if (!start && startGame)
        {
            StartCoroutine(ToggleMenu("StartMenu", false));

            if (curScene == AtScene.Start)
            {
                StartCoroutine(ToggleMenu("MainMenu", true));
            }
            else if (curScene == AtScene.MainGame)
            {
                StartCoroutine(ToggleMenu("PauseMenu", true));
            }

            startGame = false;
        }
        else if (start && startGame)
        {
            StartCoroutine(ToggleMenu("StartMenu", false));
            GoToScene("MainGame");
        }
    }

    // Set things up to run the tutorial //
    public void StartWithTut(bool chk)
    {
        runTut = chk;
        StartGame(true);
    }

    // Pause the game to give the player a break, restart, or quit to the main menu //
    public void PauseGame(bool chk)
    {
        if (chk)
        {
            Time.timeScale = 0;
            paused = chk;
            StartCoroutine(ToggleMenu("HUD", false));
            StartCoroutine(ToggleMenu("PauseMenu", true));
        }
        else if (!chk)
        {
            StartCoroutine(ToggleMenu("PauseMenu", false));
            StartCoroutine(ToggleMenu("HUD", true));
            paused = chk;
            Time.timeScale = 1;
        }
    }

    // Start the process of quiting the game //
    public void QuitGame(bool chk)
    {
        quitChk = chk;

        if (quitChk)
        {
            if (curScene == AtScene.Start)
            {
                StartCoroutine(ToggleMenu("MainMenu", false));
            }
            else if (curScene == AtScene.MainGame)
            {
                StartCoroutine(ToggleMenu("PauseMenu", false));
            }

            StartCoroutine(ToggleMenu("QuitMenu", true));
        }
        else if (!quitChk)
        {
            StartCoroutine(ToggleMenu("QuitMenu", false));

            if (curScene == AtScene.Start)
            {
                StartCoroutine(ToggleMenu("MainMenu", true));
            }
            else if (curScene == AtScene.MainGame)
            {
                StartCoroutine(ToggleMenu("PauseMenu", true));
            }
        }
    }

    // Close the game down completely //
    public void ExitGame()
    {
        // If in the editor stop playing otherwise quit the application when at the main menu //
        if (curScene == AtScene.Start)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        else if (curScene == AtScene.MainGame)
        {
            GoToScene("StartScene");
        }
    }
    // End Code that is triggered by MenuLink.cs //

    // Begin Code for managing the actual game play //
    // Start a round of game play //
    public void StartRound()
    {
        if (!spawner) { return; }

        if (runTut) { tutCon.StartRound(); }
        else { spawner.StartRound(5); }
    }

    // Show the round clear menu when the last attacker is killed //
    public void ClearRound(bool chk)
    {
        if (chk)
        {
            StartCoroutine(ToggleMenu("ClearMenu", true));
        }
        else if (!chk)
        {
            curRound++;
            StartCoroutine(ToggleMenu("ClearMenu", false));
            StartRound();
        }
    }

    // Add some coins to the coin counter //
    public void AddCoins(int count)
    {
        curCoinCount += count;
        UpdateCoinCount(curCoinCount);
    }

    // Disables the buttons if the player can't afford the object, the round is cleared, or the tutorial is active //
    public void UpdateButtons()
    {
        if (runTut) { return; }

        if (roundClear) { tutCon.DisableAllButtons(); return; }

        tutCon.ToggleNinja(priceManager.curNinjaCost <= curCoinCount && ninjaCount < curMaxNinjas);
        tutCon.ToggleWall(priceManager.curWallCost <= curCoinCount);
        tutCon.TogglePit(priceManager.curPitCost <= curCoinCount);
        tutCon.ToggleMine(priceManager.curMineCost <= curCoinCount);
    }

    // Set the counters to there default state for starting a new game //
    private void ResetCounters()
    {
        curRound = 1;
        ninjaCount = 0;
        curCoinCount = startingCoinCount;
    }

    // Update the counters //
    public void UpdateCounters()
    {
        UpdateMaxNinjas();
        UpdateCoinCount(curCoinCount);
        UpdateCurRound(curRound);
    }

    // Begin update functions //
    public void UpdateMaxNinjas()
    {
        if (maxNinjasBase + curRound < 15) { curMaxNinjas = maxNinjasBase + curRound; }
        else { curMaxNinjas = 15; }

        ninjaCounter.text = $"{ninjaCount}/{curMaxNinjas.ToString()}";
    }

    public void UpdateCoinCount(int count)
    {
        if (count > 999990) { count = 999990; }

        curCoinCount = count;

        coinCounter.text = count.ToString("000000");
    }

    public void UpdateCurRound(int round)
    {
        if (round > 99) { round = 99; }

        curRound = round;

        roundCounter.text = round.ToString("Round : 00");
    }
    // End update functions //

    // Begin unit buy functions //
    public bool BuyNinja()
    {
        // run some checks to see if the ninja can be bought //
        if (ninjaCount > curMaxNinjas) { return false; }

        return priceManager.BuyNinja(curCoinCount);
    }

    public bool BuyWall()
    {
        // run some checks to see if the wall can be bought //
        return priceManager.BuyWall(curCoinCount);
    }

    public bool BuyPit()
    {
        // run some checks to see if the wall can be bought //
        return priceManager.BuyPit(curCoinCount);
    }

    public bool BuyMine()
    {
        // run some checks to see if the wall can be bought //
        return priceManager.BuyMine(curCoinCount);
    }
    // End unit buy functions //
    // End Code for managing the actual game play //
}
