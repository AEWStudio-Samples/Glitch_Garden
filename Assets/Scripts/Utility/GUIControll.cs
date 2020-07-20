using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// This script is used to control the GUI.
/// </summary>
public class GUIControll : MonoBehaviour
{
    #region Vars

    #region con fig vars

    public bool debugging;

    public LayerMask enemieLayers;

    [Space(10)]
    public ContentTracker conTrack = null;

    public PriceManager priceCon = null;
    public TutControll tutCon = null;
    public DamageHandler dmgHand = null;
    public SoundManager soundManager = null;
    public OptionsManager opManager = null;

    [Header("Round Information")]
    public TextMeshProUGUI coinCounter = null;

    public TextMeshProUGUI ninjaCounter = null;
    public TextMeshProUGUI roundCounter = null;
    public TextMeshProUGUI mobCounter = null;

    #endregion con fig vars

    #region state vars

    private MasterSpawner spawner;

    [Header("Exposed for other scripts to acess")]
    public bool loading = false;

    public bool roundClear = false;

    private bool newGame = true;
    private bool runTut = false;
    private bool roundStart = false;

    private string mobCntrText;

    #region vars for tracking current scene

    private string curSceneName;

    private AtScene curScene = AtScene.Title;

    private enum AtScene
    { MainGame, Start, Title }

    #endregion vars for tracking current scene

    #endregion state vars

    #endregion Vars

    #region General Functions

    private void Awake()
    {
        soundManager.SwitchGameUIMusic(soundManager.atMenu);
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

    private void Update()
    {
        if (curScene == AtScene.MainGame && !loading)
        {
            UpdateButtons();
        }

        if (conTrack.mobCounts.x == 0 && !TestForHostiles() && roundStart)
        {
            roundStart = false;
            StartCoroutine(CheckClear());
        }
    }

    private bool TestForPlayerUnits()
    {
        var units = FindObjectsOfType<GameObject>();
        var tags = new string[] { "Ninja", "Wall", "Pit", "Mine" };

        foreach (var unit in units)
        {
            foreach (var tag in tags)
            {
                if (unit.tag == tag) { return true; }
            }
        }

        return false;
    }

    // Disables the buttons if the player can't afford the object, the round is cleared, or the tutorial is active //
    public void UpdateButtons()
    {
        if (runTut) { return; }

        if (roundClear) { tutCon.DisableAllButtons(); return; }

        tutCon.ToggleNinja(conTrack.curNinjaCost <= conTrack.curCoinCount
            && conTrack.ninjaCount < conTrack.maxNinjas);
        tutCon.ToggleWall(conTrack.curWallCost <= conTrack.curCoinCount);
        tutCon.TogglePit(conTrack.curPitCost <= conTrack.curCoinCount);
        tutCon.ToggleMine(conTrack.curMineCost <= conTrack.curCoinCount);

        tutCon.ToggleUpgBtn(TestForPlayerUnits());
        tutCon.ToggleDelBtn(TestForPlayerUnits());
    }

    private bool TestForHostiles()
    {
        GameObject[] mobs = FindObjectsOfType<GameObject>();

        foreach (var mob in mobs)
        {
            if (mob.tag == "Zombie" || mob.tag == "Phantom")
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator CheckClear()
    {
        yield return new WaitForSeconds(0.5f);

        if (spawner.endless)
        {
            conTrack.curRound++;
            conTrack.mobCounts.x = conTrack.mobSpawnCountBase.x * conTrack.curRound;
            UpdateCounters();
            StartCoroutine(StartRound());
        }
        else
        {
            roundClear = true;
            ClearRound(true);
        }
    }

    #endregion General Functions

    #region GUI Navagation Functions

    // Goes to a scene with the given name //
    // Used by AnimationTriggerLink.cs //
    public void GoToScene(string name)
    {
        Time.timeScale = 1;
        loading = true;
        SceneManager.LoadScene(name);
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
            soundManager.StartGameMusic();
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
    private void SetSceneGUI()
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

    // Toggles the active GUI //
    private IEnumerator ToggleMenu(string menu, bool load)
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

    // Show the main menu GUI //
    public void ShowMainMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(ToggleMenu("MainMenu", true));
    }

    // Show the GUI for playing the game //
    public void ShowGameMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(ToggleMenu("HUD", true));
    }

    // Close the game down completely //
    public IEnumerator ExitGame()
    {
        if (curScene == AtScene.MainGame)
        {
            yield return null;
            GoToScene("StartScene");
        }
        // If in the editor stop playing otherwise quit the application when at the main menu //
        else if (curScene == AtScene.Start)
        {
            StartCoroutine(ToggleMenu("QuitMenu", false));
            yield return new WaitForSeconds(0.5f);
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    #endregion GUI Navagation Functions

    #region Code triggered by MenuLink.cs

    private void StartQuitOptionToggle(bool toggle)
    {
        switch (curScene)
        {
            case AtScene.Start:
                StartCoroutine(ToggleMenu("MainMenu", toggle));
                break;

            case AtScene.MainGame when !roundClear:
                StartCoroutine(ToggleMenu("PauseMenu", toggle));
                break;

            case AtScene.MainGame when roundClear:
                StartCoroutine(ToggleMenu("ClearMenu", toggle));
                break;
        }
    }

    // Start the actual game //
    public void StartMenu(bool chk)
    {
        StartQuitOptionToggle(!chk);
        StartCoroutine(ToggleMenu("StartMenu", chk));
    }

    // Set things up to run the tutorial //
    public void StartWithTut(bool chk)
    {
        StartCoroutine(ToggleMenu("StartMenu", false));
        runTut = chk;
        GoToScene("MainGame");
    }

    // Start the process of quiting the game //
    public void QuitGame(bool chk)
    {
        StartQuitOptionToggle(!chk);
        StartCoroutine(ToggleMenu("QuitMenu", chk));
    }

    // Pause the game to give the player a break, restart, or quit to the main menu //
    public void PauseGame(bool chk)
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        StartCoroutine(ToggleMenu("HUD", !chk));
        StartCoroutine(ToggleMenu("PauseMenu", chk));
    }

    #endregion Code triggered by MenuLink.cs

    #region Code triggered by OptionsLink.cs

    public void OptionsMenu(bool chk)
    {
        StartQuitOptionToggle(!chk);
        StartCoroutine(ToggleMenu("OptionsMenu", chk));
    }

    public void SettingChanged(bool chk)
    {
        StartCoroutine(ToggleMenu("SettingChanged", chk));
    }

    #endregion Code triggered by OptionsLink.cs

    #region Code for managing game progression

    #region Round Management

    // Start a round of game play //
    public IEnumerator StartRound()
    {
        roundClear = false;

        yield return new WaitForSeconds(1);

        roundStart = true;

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

    #endregion Round Management

    #region GUI Updates

    // Set the counters to there default state for starting a new game //
    private void ResetCounters()
    {
        var tpobjs = FindObjectOfType<TrackPlayerObjs>();

        if (tpobjs) { tpobjs.ResetTracker(); }

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

    // Add some coins to the coin counter //
    public void AddCoins(int count)
    {
        conTrack.curCoinCount += count;
        UpdateCoinCount(conTrack.curCoinCount);
    }

    public void UpdateMaxNinjas()
    {
        if (conTrack.curRound == 1)
        {
            conTrack.maxNinjas = conTrack.maxNinjasBase;
        }
        else if (conTrack.curRound < 15)
        {
            conTrack.maxNinjas = conTrack.curRound + conTrack.maxNinjasBase - 1;
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

        if (spawner.endless)
        {
            roundCounter.text = "Endless";
        }
    }

    public void UpdateMobCnt(Vector2Int cnt)
    {
        conTrack.mobCounts = cnt;

        mobCntrText = "SPAWNING\n" + cnt.x;

        mobCounter.text = mobCntrText;
    }

    #endregion GUI Updates

    #region Buy Units

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

    #endregion Buy Units

    #endregion Code for managing game progression
}