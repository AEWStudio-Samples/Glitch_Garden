using System.Collections;
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
    int upgradeCost;
    public bool upgradable;
    GUIControll guiCon;

    private void Awake()
    {
        // Get GUI //
        LinkGUI();

        // Initialize Plate //
        TogglePlate(false);
    }

    public void TogglePlate(bool togl)
    {
        upgradePlate.SetActive(togl);
    }

    private void LinkGUI()
    {
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();

        // Sanity Check //
        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
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