using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDLink : MonoBehaviour
{
    // HUD elements that link to GUIControll //
    [Header("GUIControll Elements")]
    public TextMeshProUGUI ninjaCounter = null;
    public TextMeshProUGUI zombieCounter = null;
    public TextMeshProUGUI roundCounter = null;
    public TextMeshProUGUI coinCounter = null;

    // HUD elements that link to PriceManager //
    [Header("PriceManager Elements")]
    public TextMeshProUGUI ninjaPrice = null;
    public TextMeshProUGUI wallPrice = null;
    public TextMeshProUGUI pitPrice = null;
    public TextMeshProUGUI minePrice = null;

    // HUD elements that link to TutControll //
    [Header("TutControll Elements")]
    public Button ninjaButton = null;
    public Button wallButton = null;
    public Button pitButton = null;
    public Button mineButtton = null;
    public Button upgButton = null;
    public Button delButton = null;

    // state vars //
    GUIControll guiCon;
    PriceManager priceManager;
    TutControll tutCon;

    void Awake()
    {
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();
        PriceManager[] priceList = FindObjectsOfType<PriceManager>();
        TutControll[] tutList = FindObjectsOfType<TutControll>();

        // Sanity Check //
        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }

        foreach (PriceManager priceTest in priceList)
        {
            if (priceTest.CompareTag("GUI")) priceManager = priceTest;
        }

        foreach (TutControll tutTest in tutList)
        {
            if (tutTest.CompareTag("GUI")) tutCon = tutTest;
        }
    }

    private void Start()
    {
        LinkHUD();
    }

    private void LinkHUD()
    {
        // Link HUD elements to GUIControll //
        guiCon.ninjaCounter = ninjaCounter;
        guiCon.zombieCounter = zombieCounter;
        guiCon.roundCounter = roundCounter;
        guiCon.coinCounter = coinCounter;

        // Link HUD elements to PriceManager //
        priceManager.ninjaPrice = ninjaPrice;
        priceManager.wallPrice = wallPrice;
        priceManager.pitPrice = pitPrice;
        priceManager.minePrice = minePrice;

        // Link HUD elements to TutControll //
        tutCon.ninjaButton = ninjaButton;
        tutCon.wallButton = wallButton;
        tutCon.pitButton = pitButton;
        tutCon.mineButton = mineButtton;
        tutCon.upgButton = upgButton;
        tutCon.delButton = delButton;

        guiCon.UpdateCounters();
        priceManager.UpdatePrice();

        if (guiCon.runTut)
        {
            tutCon.DisableAllButtons();
        }

        guiCon.loading = false;
    }
}
