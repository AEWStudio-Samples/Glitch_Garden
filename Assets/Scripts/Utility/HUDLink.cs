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
        LinkHUD();
    }

    private void LinkHUD()
    {
        // Link HUD elements to GUIControll //
        guiCon.ninjaCounter = ninjaCounter;
        guiCon.mobCounter = zombieCounter;
        guiCon.roundCounter = roundCounter;
        guiCon.coinCounter = coinCounter;

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

        if (guiCon.runTut)
        {
            guiCon.tutCon.DisableAllButtons();
        }

        guiCon.loading = false;
    }
}
