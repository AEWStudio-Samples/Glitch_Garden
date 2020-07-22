using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDLink : MonoBehaviour
{
    #region HUD Elements

    #region Link to GUIControll.cs

    [Header("GUIControll Elements")]
    public TextMeshProUGUI ninjaCounter = null;

    public TextMeshProUGUI spawnCounter = null;
    public TextMeshProUGUI roundCounter = null;
    public TextMeshProUGUI coinCounter = null;

    public Slider escapeTracker = null;

    #endregion Link to GUIControll.cs

    #region Link to PriceManager.cs

    [Header("PriceManager Elements")] public TextMeshProUGUI ninjaPrice = null;
    public TextMeshProUGUI wallPrice = null;
    public TextMeshProUGUI pitPrice = null;
    public TextMeshProUGUI minePrice = null;

    #endregion Link to PriceManager.cs

    #region Link to TutControll.cs

    [Header("TutControll Elements")] public Button ninjaButton = null;
    public Button wallButton = null;
    public Button pitButton = null;
    public Button mineButtton = null;
    public Button upgButton = null;
    public Button delButton = null;

    #endregion Link to TutControll.cs

    #endregion HUD Elements

    #region Get GUIControll

    // state vars //
    private GUIControll guiCon;

    private void Awake()
    {
        // Sanity Check //
        foreach (GUIControll guiTest in FindObjectsOfType<GUIControll>())
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
    }

    #endregion Get GUIControll

    #region Link HUD elements to there scripts

    private void Start()
    {
        LinkHUD();
    }

    private void LinkHUD()
    {
        // Link HUD elements to GUIControll //
        guiCon.ninjaCounter = ninjaCounter;
        guiCon.spawnCounter = spawnCounter;
        guiCon.roundCounter = roundCounter;
        guiCon.coinCounter = coinCounter;
        guiCon.escapeTracker = escapeTracker;

        // Link HUD elements to PriceManager //
        guiCon.priceCon.ninjaPrice = ninjaPrice;
        guiCon.priceCon.wallPrice = wallPrice;
        guiCon.priceCon.pitPrice = pitPrice;
        guiCon.priceCon.minePrice = minePrice;

        // Link HUD elements to TutControll //
        guiCon.tutCon.ninjaButton = ninjaButton;
        guiCon.tutCon.wallButton = wallButton;
        guiCon.tutCon.pitButton = pitButton;
        guiCon.tutCon.mineButton = mineButtton;
        guiCon.tutCon.upgButton = upgButton;
        guiCon.tutCon.delButton = delButton;

        guiCon.UpdateCounters();
        guiCon.priceCon.UpdatePrice();

        guiCon.loading = false;
    }

    #endregion Link HUD elements to there scripts
}