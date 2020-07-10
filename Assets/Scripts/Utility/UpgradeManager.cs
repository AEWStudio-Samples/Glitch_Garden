﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    // con fig vars //
    [SerializeField]
    GameObject upgradePlate = null;

    [SerializeField]
    TextMeshPro upgradeText = null;

    // state vars //
    internal int upgradeCost;
    internal bool upgradable;
    GUIControll guiCon;

    private void Awake()
    {
        // Get GUI //
        // Sanity Check //
        foreach (GUIControll guiTest in FindObjectsOfType<GUIControll>())
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }

        // Initialize Plate //
        TogglePlate(false);
    }

    public void TogglePlate(bool togl)
    {
        upgradePlate.SetActive(togl);
    }

    public void SetUpgradeCost(int cost, int rank)
    {
        upgradeCost = cost + (cost * rank);
        upgradeText.text = upgradeCost.ToString();
    }

    public bool CheckCoinCount()
    {
        if (guiCon.curCoinCount >= upgradeCost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}