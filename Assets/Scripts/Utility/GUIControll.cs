using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityScript.Steps;
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

    [Space(10)]
    public ContentTracker conTrack = null;
    public PriceManager priceCon = null;
    public TutControll tutCon = null;

    [Header("Round Information")]
    public TextMeshProUGUI coinCounter = null;
    public TextMeshProUGUI ninjaCounter = null;
    public TextMeshProUGUI roundCounter = null;
    public TextMeshProUGUI mobCounter = null;

    // state vars //
    MasterSpawner spawner;
    [Header("Exposed for other scripts to acess")]
    public int mobCntrInc = 0;
    public bool loading = false;
    public bool runTut = false;

    bool startGame = false;
    bool newGame = true;
    bool roundClear = false;
    bool roundStart = false;
    bool quitChk = false;
    bool paused = false;

    string[] mobCntrText = new string[3];

    int curMobCnt;

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
            UpdateCurRound(conTrack.curRound + 1);
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
            StartCoroutine(StartRound());
            if (newGame)
            {
                ResetCounters();
                priceCon.ResetPrice();
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
        priceCon.ResetPrice();
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



        if (conTrack.mobsThisRound == 0 && roundStart)
        {
            roundStart = false;
            roundClear = true;
            ClearRound(true);
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
            else if (curScene == AtScene.MainGame && !roundClear)
            {
                StartCoroutine(ToggleMenu("PauseMenu", false));
            }
            else if (curScene == AtScene.MainGame && roundClear)
            {
                StartCoroutine(ToggleMenu("ClearMenu", false));
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
            else if (curScene == AtScene.MainGame && !roundClear)
            {
                StartCoroutine(ToggleMenu("PauseMenu", true));
            }
            else if (curScene == AtScene.MainGame && roundClear)
            {
                StartCoroutine(ToggleMenu("ClearMenu", true));
            }

            startGame = false;
        }
        else if (start && startGame)
        {
            startGame = false;
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
            else if (curScene == AtScene.MainGame && !roundClear)
            {
                StartCoroutine(ToggleMenu("PauseMenu", false));
            }
            else if (curScene == AtScene.MainGame && roundClear)
            {
                StartCoroutine(ToggleMenu("ClearMenu", false));
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
            else if (curScene == AtScene.MainGame && !roundClear)
            {
                StartCoroutine(ToggleMenu("PauseMenu", true));
            }
            else if (curScene == AtScene.MainGame && roundClear)
            {
                StartCoroutine(ToggleMenu("ClearMenu", true));
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
    public IEnumerator StartRound()
    {
        roundClear = false;

        yield return new WaitForSeconds(1);

        roundStart = true;
        StartCoroutine(MobCntrCycle());

        if (spawner)
        {
            if (runTut) { tutCon.StartRound(); }
            else { spawner.StartRound(conTrack.mobCounts.x); }
        }
    }

    // Show the round clear menu when the last attacker is killed //
    public void ClearRound(bool chk)
    {
        if (chk)
        {
            StartCoroutine(ToggleMenu("ClearMenu", true));
            conTrack.curRound++;
            conTrack.mobCounts.x = conTrack.mobSpawnCountBase.x * conTrack.curRound;
            UpdateCounters();
        }
        else if (!chk)
        {
            StartCoroutine(ToggleMenu("ClearMenu", false));
            StartCoroutine(StartRound());
        }
    }

    // Add some coins to the coin counter //
    public void AddCoins(int count)
    {
        conTrack.curCoinCount += count;
        UpdateCoinCount(conTrack.curCoinCount);
    }

    // Disables the buttons if the player can't afford the object, the round is cleared, or the tutorial is active //
    public void UpdateButtons()
    {
        if (runTut) { return; }

        if (roundClear) { tutCon.DisableAllButtons(); return; }
        else
        {
            tutCon.ToggleUpgBtn(true);
            tutCon.ToggleDelBtn(true);
        }

        tutCon.ToggleNinja(conTrack.curNinjaCost <= conTrack.curCoinCount 
            && conTrack.ninjaCount < conTrack.maxNinjas);
        tutCon.ToggleWall(conTrack.curWallCost <= conTrack.curCoinCount);
        tutCon.TogglePit(conTrack.curPitCost <= conTrack.curCoinCount);
        tutCon.ToggleMine(conTrack.curMineCost <= conTrack.curCoinCount);
    }

    // Set the counters to there default state for starting a new game //
    private void ResetCounters()
    {
        FindObjectOfType<TrackPlayerObjs>().ResetTracker();
        conTrack.curRound = 1;
        conTrack.ninjaCount = 0;
        conTrack.curCoinCount = conTrack.startCoinCount;
        conTrack.mobCounts = conTrack.mobSpawnCountBase;
    }

    // Update the counters //
    public void UpdateCounters()
    {
        UpdateMaxNinjas();
        UpdateCoinCount(conTrack.curCoinCount);
        UpdateCurRound(conTrack.curRound);
        UpdateMobCnt(conTrack.mobCounts);
        conTrack.mobsThisRound = conTrack.mobCounts.x;
    }

    // Begin update functions //
    public void UpdateMaxNinjas()
    {
        if (conTrack.curRound < 15)
        {
            conTrack.maxNinjas = conTrack.curRound;
        }
        else { conTrack.maxNinjas = 15; }

        ninjaCounter.text = $"{conTrack.ninjaCount}/{conTrack.maxNinjas}";
    }

    public void UpdateCoinCount(int count)
    {
        if (count > 999990) { count = 999990; }

        conTrack.curCoinCount = count;

        coinCounter.text = count.ToString("000000");
    }

    public void UpdateCurRound(int round)
    {
        if (round > 99) { round = 99; }

        conTrack.curRound = round;

        roundCounter.text = round.ToString("Round : 00");
    }

    public void UpdateMobCnt(Vector3Int cnt)
    {
        conTrack.mobCounts = cnt;

        mobCntrText[0] = "SPAWNING\n" + cnt.x;
        mobCntrText[1] = "KILLED\n" + cnt.y;
        mobCntrText[2] = "ESCAPED\n" + cnt.z;

        mobCounter.text = mobCntrText[mobCntrInc];
    }

    IEnumerator MobCntrCycle()
    {
        while (roundStart)
        {
            mobCounter.text = mobCntrText[mobCntrInc];
            mobCntrInc++;

            if (mobCntrInc == 3) { mobCntrInc = 0; }

            yield return new WaitForSeconds(3);
        }
        mobCounter.text = mobCntrText[0];
        mobCntrInc = 0;
    }
    // End update functions //

    // Begin unit buy functions //
    public bool BuyNinja()
    {
        // run some checks to see if the ninja can be bought //
        if (conTrack.ninjaCount > conTrack.maxNinjas) { return false; }

        return priceCon.BuyNinja(conTrack.curCoinCount);
    }

    public bool BuyWall()
    {
        // run some checks to see if the wall can be bought //
        return priceCon.BuyWall(conTrack.curCoinCount);
    }

    public bool BuyPit()
    {
        // run some checks to see if the wall can be bought //
        return priceCon.BuyPit(conTrack.curCoinCount);
    }

    public bool BuyMine()
    {
        // run some checks to see if the wall can be bought //
        return priceCon.BuyMine(conTrack.curCoinCount);
    }
    // End unit buy functions //
    // End Code for managing the actual game play //
}
