using System.Collections;
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
    [SerializeField]
    int maxNinjasBase = 2;

    public int maxNinjas;
    public int ninjaCount;

    [Header("Round Information")]
    [SerializeField]
    TextMeshProUGUI ninjaCounter = null;

    [SerializeField]
    TextMeshProUGUI roundCounter = null;

    [SerializeField]
    TextMeshProUGUI zombieCounter = null;

    [Header("Resource Counters")]
    [SerializeField]
    TextMeshProUGUI coinCounter = null;
    [SerializeField]
    int startingCoinCount = 100;
    [SerializeField]
    PriceManager priceManager = null;

    // state vars //
    TutControll tutCon;
    MasterSpawner spawner;
    public int curRound;
    public int curCoinCount;

    // state vars for the animator //
    Animator anim;
    int showMainMenuHash = Animator.StringToHash("Show Main Menu");
    int startGameHash = Animator.StringToHash("Start Game");
    int roundOneHash = Animator.StringToHash("Round One");
    int quitGameHash = Animator.StringToHash("Quit Game");
    int startPlayHash = Animator.StringToHash("Start Play");
    int pauseGameHash = Animator.StringToHash("Pause Game");
    int newWaveHash = Animator.StringToHash("New Wave");
    int mainMenuHash = Animator.StringToHash("Main Menu");
    int mainGameHash = Animator.StringToHash("Main Game");
    int tutHash = Animator.StringToHash("Tutorial");
    int pauseHash = Animator.StringToHash("Pause");
    int quitChkHash = Animator.StringToHash("Quit Chk");
    int roundClearHash = Animator.StringToHash("Round Clear");

    // state vars tracking the active scene //
    string curSceneName;

    AtScene curScene = AtScene.Title;

    enum AtScene { MainGame, MainMenu, Title}

    void Awake()
    {
        #if UNITY_EDITOR
        #else
        debugging = false;
        #endif
        // Make it so there is only one //
        int numGUIControlls = FindObjectsOfType<GUIControll>().Length;

        if (numGUIControlls > 1)
        {
            Destroy(this.gameObject);
        }

        // One to rule them all //
        DontDestroyOnLoad(this.gameObject);

        // Get key components //
        anim = GetComponent<Animator>();
        tutCon = GetComponent<TutControll>();

        // Just setting things up for first play //
        ResetCounters();
        priceManager.ResetPrice();

        // What scene are we on //
        StartCoroutine(CheckScene());
    }

    // Set the counters to there default state for starting a new game //
    private void ResetCounters()
    {
        curRound = 1;
        ninjaCount = 0;
        curCoinCount = startingCoinCount;

        UpdateMaxNinjas();
        UpdateCoinCount(curCoinCount);
        UpdateCurRound(curRound);
    }

    void Update()
    {
        CheckInput();
        UpdateButtons();
    }

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
            if (curScene == AtScene.MainMenu)
            {
                QuitGame(!anim.GetBool(quitChkHash));
            }
            else if (curScene == AtScene.MainGame)
            {
                if (anim.GetBool(pauseHash) && anim.GetBool(quitChkHash))
                {
                    QuitGame(false);
                    return;
                }

                PauseGame(!anim.GetBool(pauseHash));
            }
        }
    }

    // Disable the buttons if the player can't afford the object, the round is cleared, or the tutorial is active //
    public void UpdateButtons()
    {
        if (anim.GetBool(tutHash)) { return; }

        if (anim.GetBool(roundClearHash)) { tutCon.DisableAllButtons(); return; }

        tutCon.ToggleNinja(priceManager.curNinjaCost <= curCoinCount && ninjaCount < maxNinjas);
        tutCon.ToggleWall(priceManager.curWallCost <= curCoinCount);
        tutCon.TogglePit(priceManager.curPitCost <= curCoinCount);
        tutCon.ToggleMine(priceManager.curMineCost <= curCoinCount);
    }

    // Check what scene we are on so that we know what we are doing //
    private IEnumerator CheckScene()
    {
        yield return SceneManager.GetActiveScene().isLoaded;

        curSceneName = SceneManager.GetActiveScene().name;

        anim.SetBool(mainMenuHash, false);
        anim.SetBool(mainGameHash, false);
        anim.SetBool(pauseHash, false);
        anim.SetBool(quitChkHash, false);
        anim.SetBool(roundClearHash, false);

        if (curSceneName == "MainMenu")
        {
            curScene = AtScene.MainMenu;
        }
        else if (curSceneName == "MainGame")
        {
            curScene = AtScene.MainGame;
            spawner = FindObjectOfType<MasterSpawner>();
        }

        SetSceneGUI();
    }

    // Set the GUI up for the current scene //
    void SetSceneGUI()
    {
        switch(curScene)
        {
            case AtScene.MainMenu:
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
        ResetCounters();
        priceManager.ResetPrice();
        SceneManager.LoadScene(name);
        StartCoroutine(CheckScene());
    }

    // Show the main menu GUI //
    public void ShowMainMenu()
    {
        anim.SetTrigger(showMainMenuHash);
        if (curScene == AtScene.MainMenu) { anim.SetBool(mainMenuHash, true); }
    }

    // Set things up to run the tutorial //
    public void StartWithTut(bool chk)
    {
        anim.SetBool(tutHash, chk);
        StartGame(true);
    }

    // Show the GUI for playing the game //
    public void ShowGameMenu()
    {
        Time.timeScale = 1;
        anim.SetTrigger(startPlayHash);
        anim.SetBool(mainGameHash, true);
        if (anim.GetBool(tutHash)) 
        {
            tutCon.DisableAllButtons();
        }
    }

    // Start the actual game //
    public void StartGame(bool start)
    {
        if (!start) { anim.SetTrigger(startGameHash); }
        else if (start) { anim.SetTrigger(roundOneHash); }
    }

    // Pause the game to give the player a break, restart, or quit to the main menu //
    public void PauseGame(bool chk)
    {
        if (chk)
        {
            Time.timeScale = 0;
            anim.SetBool(pauseHash, chk);
            anim.SetTrigger(pauseGameHash);
        }
        else if (!chk)
        {
            Time.timeScale = 1;
            anim.SetBool(pauseHash, chk);
            anim.SetTrigger(pauseGameHash);
        }
    }

    // Start the process of quiting the game //
    public void QuitGame(bool chk)
    {
        anim.SetBool(quitChkHash, chk);
        anim.SetTrigger(quitGameHash);
    }

    // Close the game down completely //
    public void ExitGame()
    {
        // If in the editor stop playing otherwise quit the application //
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // Start a round of game play //
    public void StartRound()
    {
        if (!spawner) { return; }

        if (anim.GetBool(tutHash)) { tutCon.StartRound();}
        else { spawner.StartRound(); }
    }

    // Show the round clear menu when the last attacker is killed //
    public void ClearRound()
    {
        anim.SetBool(roundClearHash, !anim.GetBool(roundClearHash));
        anim.SetTrigger(newWaveHash);
        if (anim.GetBool(roundClearHash)) { curRound++; }
    }

    // Add some coins to the coin counter //
    public void AddCoins(int count)
    {
        curCoinCount += count;
        UpdateCoinCount(curCoinCount);
    }

    // Begin update functions //
    public void UpdateCurRound(int round)
    {
        if (round > 99) { round = 99; }

        curRound = round;

        roundCounter.text = round.ToString("Round : 00");
    }

    public void UpdateCoinCount(int count)
    {
        if (count > 999990) { count = 999990; }

        curCoinCount = count;

        coinCounter.text = count.ToString("000000");
    }

    public void UpdateMaxNinjas()
    {
        if (maxNinjasBase + curRound < 15) { maxNinjas = maxNinjasBase + curRound; }
        else { maxNinjas = 15; }

        ninjaCounter.text = $"{ninjaCount}/{maxNinjas.ToString()}";
    }
    // End update functions //

    // Begin unit buy functions //
    public bool BuyNinja()
    {
        // run some checks to see if the ninja can be bought //
        if (ninjaCount > maxNinjas) { return false; }

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
}
