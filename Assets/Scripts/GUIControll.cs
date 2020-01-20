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

public class GUIControll : MonoBehaviour
{
    // con fig vars
    [Header("Ninja Limit")]
    [SerializeField]
    int maxNinjasBase = 2;
    public int maxNinjas;
    public int ninjaCount;

    [Header("Resource Collectors")]
    [SerializeField]
    Transform coinCollector = null;
    [SerializeField]
    Transform heartCollector = null;

    [Header("Resource Counters")]
    [SerializeField]
    TextMeshProUGUI ninjaCounter = null;
    [SerializeField]
    TextMeshProUGUI coinCounter = null;
    [SerializeField]
    int startingCoinCount = 100;
    [SerializeField]
    TextMeshProUGUI heartCounter = null;
    [SerializeField]
    PriceManager priceManager = null;

    // state vars for tracking game progress
    TutControll tutCon;
    MasterSpawner spawner;
    int curRound;
    int curCoinCount;
    int curHeartCount;

    // state vars for the animator
    Animator anim;
    int showMainMenuHash = Animator.StringToHash("Show Main Menu");
    int startGameHash = Animator.StringToHash("Start Game");
    int roundOneHash = Animator.StringToHash("Round One");
    int quitGameHash = Animator.StringToHash("Quit Game");
    int exitHash = Animator.StringToHash("EXIT");
    int showPauseScreenHash = Animator.StringToHash("Show Pause Screen");
    int hidePauseScreenHash = Animator.StringToHash("Hide Pause Screen");
    int startPlayHash = Animator.StringToHash("Start Play");
    int pauseGameHash = Animator.StringToHash("Pause Game");
    int waveClearHash = Animator.StringToHash("Wave Clear");
    int mainMenuHash = Animator.StringToHash("Main Menu");
    int mainGameHash = Animator.StringToHash("Main Game");
    int tutHash = Animator.StringToHash("Tutorial");
    int pauseHash = Animator.StringToHash("Pause");
    int quitChkHash = Animator.StringToHash("Quit Chk");
    int newWaveHash = Animator.StringToHash("New Wave");

    // state vars tracking the active scene
    string curSceneName;

    AtScene curScene = AtScene.Title;

    enum AtScene { MainGame, MainMenu, Title}

    void Awake()
    {
        int numGUIControlls = FindObjectsOfType<GUIControll>().Length;

        if (numGUIControlls > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        anim = GetComponent<Animator>();
        tutCon = GetComponent<TutControll>();
        ResetCounters();
        priceManager.ResetPrice();

        StartCoroutine(CheckScene());
    }

    private void ResetCounters()
    {
        curRound = 1;
        ninjaCount = 0;
        curHeartCount = 0;
        curCoinCount = startingCoinCount;

        UpdateMaxNinjas();
        UpdateCoinCount(curCoinCount);
        UpdateHeartCount(curHeartCount);
    }

    void Update()
    {
        CheckInput();
        UpdateButtons();
    }

    private void CheckInput()
    {
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

    private void UpdateButtons()
    {
        if (anim.GetBool(tutHash)) { return; }

        if (anim.GetBool(waveClearHash)) { tutCon.DisableAllButtons(); return; }

        tutCon.ToggleNinja(priceManager.curNinjaCost <= curCoinCount && ninjaCount < maxNinjas);
        tutCon.ToggleWall(priceManager.curWallCost <= curCoinCount);
        tutCon.TogglePit(priceManager.curPitCost <= curCoinCount);
        tutCon.ToggleMine(priceManager.curMineCost <= curCoinCount);
        tutCon.TogglePhantom(priceManager.curPhantomCost <= curHeartCount);
    }

    private IEnumerator CheckScene()
    {
        yield return SceneManager.GetActiveScene().isLoaded;

        curSceneName = SceneManager.GetActiveScene().name;

        anim.SetBool(mainMenuHash, false);
        anim.SetBool(mainGameHash, false);
        anim.SetBool(pauseHash, false);
        anim.SetBool(quitChkHash, false);
        anim.SetBool(newWaveHash, false);

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

    public void GoToScene(string name)
    {
        ResetCounters();
        priceManager.ResetPrice();
        SceneManager.LoadScene(name);
        StartCoroutine(CheckScene());
    }

    public void ShowMainMenu()
    {
        anim.SetTrigger(showMainMenuHash);
        if (curScene == AtScene.MainMenu) { anim.SetBool(mainMenuHash, true); }
    }

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

    public void StartWithTut(bool chk)
    {
        anim.SetBool(tutHash, chk);
        StartGame(true);
    }

    public void StartGame(bool start)
    {
        if (!start) { anim.SetTrigger(startGameHash); }
        else if (start) { anim.SetTrigger(roundOneHash); }
    }

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
            anim.SetBool(pauseHash, chk);
            anim.SetTrigger(pauseGameHash);
        }
    }

    public void QuitGame(bool chk)
    {
        anim.SetBool(quitChkHash, chk);
        anim.SetTrigger(quitGameHash);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void StartRound()
    {
        if (!spawner) { return; }

        if (anim.GetBool(tutHash)) { tutCon.StartRound();}
        else { spawner.StartRound(); }
    }

    public void ClearRound()
    {
        anim.SetBool(newWaveHash, !anim.GetBool(newWaveHash));
        anim.SetTrigger(waveClearHash);
        if (anim.GetBool(newWaveHash)) { curRound++; }
    }

    public Transform GetCoinCollector() { return coinCollector; }

    public Transform GetHeartCollector() { return heartCollector; }

    public void UpdateMaxNinjas()
    {
        if (maxNinjasBase + curRound < 15) { maxNinjas = maxNinjasBase + curRound; }
        else { maxNinjas = 15; }

        ninjaCounter.text = $"{ninjaCount}/{maxNinjas.ToString()}";
    }

    public void UpdateCoinCount(int count)
    {
        if (count > 999990) { count = 999990; }

        curCoinCount = count;

        coinCounter.text = count.ToString("000000");
    }

    public void UpdateHeartCount(int count)
    {
        if (count > 30) { count = 30; }

        heartCounter.text = $"{count.ToString()}/30";
    }

    public void AddCoins(int count)
    {
        curCoinCount += count;
        UpdateCoinCount(curCoinCount);
    }

    public void AddHeart(int count)
    {
        count /= 10;
        curHeartCount += count;
        UpdateHeartCount(curHeartCount);
    }

    public bool BuyNinja()
    {
        // run some checks to see if the ninja can be bought
        if (ninjaCount > maxNinjas) { return false; }

        if (!priceManager.BuyNinja(curCoinCount)) { return false; }

        return true;
    }
}
