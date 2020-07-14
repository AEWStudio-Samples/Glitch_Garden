using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This script is used to manage the cost of buying and upgrading units.
/// </summary>
public class PriceManager : MonoBehaviour
{
    // con fig vars //
    public GUIControll guiCon = null;

    [Header("Price Tags")]
    public TextMeshProUGUI ninjaPrice = null;
    public TextMeshProUGUI wallPrice = null;
    public TextMeshProUGUI pitPrice = null;
    public TextMeshProUGUI minePrice = null;

    public void ResetPrice()
    {
        // Reset Ninja Price //
        guiCon.conTrack.curNinjaCost = guiCon.conTrack.ninjaBasePrice;

        // Reset Wall Price //
        guiCon.conTrack.curWallCost = guiCon.conTrack.wallBasePrice;

        // Reset Pit Price //
        guiCon.conTrack.curPitCost = guiCon.conTrack.pitBasePrice;

        // Reset Mine Price //
        guiCon.conTrack.curMineCost = guiCon.conTrack.mineBasePrice;
    }

    public void UpdatePrice()
    {
        UpdateNinjaPrice(guiCon.conTrack.curNinjaCost);
        UpdateWallPrice(guiCon.conTrack.curWallCost);
        UpdatePitPrice(guiCon.conTrack.curPitCost);
        UpdateMinePrice(guiCon.conTrack.curMineCost);
    }

    // Begin price update functions //
    public void UpdateNinjaPrice(int cost)
    {
        ninjaPrice.text = cost.ToString();
    }

    public void UpdateWallPrice(int cost)
    {
        wallPrice.text = cost.ToString();
    }

    public void UpdatePitPrice(int cost)
    {
        pitPrice.text = cost.ToString();
    }

    public void UpdateMinePrice(int cost)
    {
        minePrice.text = cost.ToString();
    }
    // End price update functions //

    // Begin unit buy functions //
    public bool BuyNinja(int curCoinCount)
    {
        // Run some checks to see if the ninja can be bought //
        if (curCoinCount < guiCon.conTrack.curNinjaCost) { return false; }

        // update GUI elements
        curCoinCount -= guiCon.conTrack.curNinjaCost;
        guiCon.conTrack.curNinjaCost += guiCon.conTrack.ninjaBasePrice;
        guiCon.UpdateMaxNinjas();
        guiCon.UpdateCoinCount(curCoinCount);
        UpdateNinjaPrice(guiCon.conTrack.curNinjaCost);

        return true;
    }

    public bool BuyWall(int curCoinCount)
    {
        // Run some checks to see if the ninja can be bought //
        if (curCoinCount < guiCon.conTrack.curWallCost) { return false; }

        // update GUI elements
        curCoinCount -= guiCon.conTrack.curWallCost;
        guiCon.conTrack.curWallCost += guiCon.conTrack.wallBasePrice;
        guiCon.UpdateCoinCount(curCoinCount);
        UpdateWallPrice(guiCon.conTrack.curWallCost);

        return true;
    }

    public bool BuyPit(int curCoinCount)
    {
        // Run some checks to see if the ninja can be bought //
        if (curCoinCount < guiCon.conTrack.curPitCost) { return false; }

        // update GUI elements
        curCoinCount -= guiCon.conTrack.curPitCost;
        guiCon.conTrack.curPitCost += guiCon.conTrack.pitBasePrice;
        guiCon.UpdateCoinCount(curCoinCount);
        UpdatePitPrice(guiCon.conTrack.curPitCost);

        return true;
    }

    public bool BuyMine(int curCoinCount)
    {
        // Run some checks to see if the ninja can be bought //
        if (curCoinCount < guiCon.conTrack.curMineCost) { return false; }

        // update GUI elements
        curCoinCount -= guiCon.conTrack.curMineCost;
        guiCon.conTrack.curMineCost += guiCon.conTrack.mineBasePrice;
        guiCon.UpdateCoinCount(curCoinCount);
        UpdateMinePrice(guiCon.conTrack.curMineCost);

        return true;
    }
    // End unit buy functions //
}
