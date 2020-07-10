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
    [Header("Price Tags")]
    public TextMeshProUGUI ninjaPrice = null;
    public TextMeshProUGUI wallPrice = null;
    public TextMeshProUGUI pitPrice = null;
    public TextMeshProUGUI minePrice = null;

    [Header("Base Price")]
    public int ninjaBasePrice = 70;
    public int wallBasePrice = 50;
    public int pitBasePrice = 25;
    public int mineBasePrice = 100;

    [Header("Upgrade Price")]
    public int ninjaUpPrice = 50;
    public int wallUpPrice = 25;
    public int pitUpPrice = 10;
    public int mineUpPrice = 80;

    // state vars //
    [Header("DO NOT TOUTCH")]
    public int curNinjaCost;
    public int curWallCost;
    public int curPitCost;
    public int curMineCost;

    GUIControll guiCon = null;

    private void Awake()
    {
        guiCon = GetComponent<GUIControll>();
    }

    public void ResetPrice()
    {
        // Reset Ninja Price //
        curNinjaCost = ninjaBasePrice;

        // Reset Wall Price //
        curWallCost = wallBasePrice;

        // Reset Pit Price //
        curPitCost = pitBasePrice;

        // Reset Mine Price //
        curMineCost = mineBasePrice;
    }

    public void UpdatePrice()
    {
        UpdateNinjaPrice(curNinjaCost);
        UpdateWallPrice(curWallCost);
        UpdatePitPrice(curPitCost);
        UpdateMinePrice(curMineCost);
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
        if (curCoinCount < curNinjaCost) { return false; }

        // update GUI elements
        curCoinCount -= curNinjaCost;
        curNinjaCost += ninjaBasePrice;
        guiCon.UpdateMaxNinjas();
        guiCon.UpdateCoinCount(curCoinCount);
        UpdateNinjaPrice(curNinjaCost);

        return true;
    }

    public bool BuyWall(int curCoinCount)
    {
        // Run some checks to see if the ninja can be bought //
        if (curCoinCount < curWallCost) { return false; }

        // update GUI elements
        curCoinCount -= curWallCost;
        curWallCost += wallBasePrice;
        guiCon.UpdateCoinCount(curCoinCount);
        UpdateWallPrice(curWallCost);

        return true;
    }

    public bool BuyPit(int curCoinCount)
    {
        // Run some checks to see if the ninja can be bought //
        if (curCoinCount < curPitCost) { return false; }

        // update GUI elements
        curCoinCount -= curPitCost;
        curPitCost += pitBasePrice;
        guiCon.UpdateCoinCount(curCoinCount);
        UpdatePitPrice(curPitCost);

        return true;
    }

    public bool BuyMine(int curCoinCount)
    {
        // Run some checks to see if the ninja can be bought //
        if (curCoinCount < curMineCost) { return false; }

        // update GUI elements
        curCoinCount -= curMineCost;
        curMineCost += mineBasePrice;
        guiCon.UpdateCoinCount(curCoinCount);
        UpdateMinePrice(curMineCost);

        return true;
    }
    // End unit buy functions //
}
