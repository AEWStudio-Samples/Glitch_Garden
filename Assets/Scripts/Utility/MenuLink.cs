using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLink : MonoBehaviour
{
    // state vars //
    GUIControll guiCon;
    HUDLink hudLink;
    PriceManager priceManager;

    [SerializeField]
    MenuType menuType = MenuType.Main;

    enum MenuType { Main, Sub, HUD }

    void Awake()
    {
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();

        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }

        if (menuType == MenuType.HUD)
        {
            PriceManager[] priceList = FindObjectsOfType<PriceManager>();

            // Sanity Check //
            foreach (PriceManager priceTest in priceList)
            {
                if (priceTest.CompareTag("GUI")) priceManager = priceTest;
            }

            hudLink = GetComponent<HUDLink>();

            LinkHUD();
        }
    }

    private void LinkHUD()
    {
        // Link HUD elements to GUIControll //
        guiCon.ninjaCounter = hudLink.ninjaCounter;
        guiCon.zombieCounter = hudLink.zombieCounter;
        guiCon.roundCounter = hudLink.roundCounter;
        guiCon.coinCounter = hudLink.coinCounter;

        // Link HUD elements to PriceManager //
        priceManager.ninjaPrice = hudLink.ninjaPrice;
        priceManager.wallPrice = hudLink.wallPrice;
        priceManager.pitPrice = hudLink.pitPrice;
        priceManager.minePrice = hudLink.minePrice;
    }

    public void StartGame(bool chk)
    {
        guiCon.StartGame(chk);
    }

    public void StartWithTut(bool chk)
    {
        guiCon.StartWithTut(chk);
    }
}
