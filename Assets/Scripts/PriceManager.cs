using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PriceManager : MonoBehaviour
{
    // con fig vars
    [SerializeField, Space(10)]
    GUIControll guiControll = null;

    [Header("Price Tags")]
    [SerializeField]
    TextMeshProUGUI ninjaPrice = null;
    [SerializeField]
    TextMeshProUGUI wallPrice = null;
    [SerializeField]
    TextMeshProUGUI pitPrice = null;
    [SerializeField]
    TextMeshProUGUI minePrice = null;

    [Header("Base Price")]
    public int ninjaBasePrice = 75;
    public int wallBasePrice = 50;
    public int pitBasePrice = 25;
    public int mineBasePrice = 100;

    // state vars
    [Header("DO NOT TOUTCH")]
    public int curNinjaCost;
    public int curWallCost;
    public int curPitCost;
    public int curMineCost;

    public bool BuyNinja(int curCoinCount)
    {

        // run some checks to see if the ninja can be bought
        if (curCoinCount < curNinjaCost) { return false; }

        // update GUI elements
        curCoinCount -= curNinjaCost;
        curNinjaCost += ninjaBasePrice;
        guiControll.UpdateMaxNinjas();
        guiControll.UpdateCoinCount(curCoinCount);
        UpdateNinjaPrice(curNinjaCost);

        return true;
    }

    public void ResetPrice()
    {
        curNinjaCost = ninjaBasePrice;
        curWallCost = wallBasePrice;
        curPitCost = pitBasePrice;
        curMineCost = mineBasePrice;
        UpdateNinjaPrice(curNinjaCost);
        UpdateWallPrice(curWallCost);
        UpdatePitPrice(curPitCost);
        UpdateMinePrice(curMineCost);
    }

    public void UpdateNinjaPrice(int cost)
    {
        ninjaPrice.text = cost.ToString();
    }

    private void UpdateWallPrice(int cost)
    {
        wallPrice.text = cost.ToString();
    }

    private void UpdatePitPrice(int cost)
    {
        pitPrice.text = cost.ToString();
    }

    private void UpdateMinePrice(int cost)
    {
        minePrice.text = cost.ToString();
    }
}
