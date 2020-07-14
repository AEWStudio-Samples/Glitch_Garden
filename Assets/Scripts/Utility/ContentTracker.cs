using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentTracker : MonoBehaviour
{
    // con fig vars //
    public GUIControll guiCon = null;

    [Space(25, order = 0), Header("Mid Round Upgrades", order = 1)]
    [Space(-10, order = 2), Header("Round Information", order = 3)]
    public int curRound = 0;

    [Header("Starting Values")]
    public int startCoinCount = 200;
    public int maxNinjasBase = 3;
    public Vector3Int mobSpawnCountBase = new Vector3Int { x = 5, y = 0, z = 0 };

    [Header("Counters")]
    public int curCoinCount = 0;
    public int maxNinjas = 0;
    public int ninjaCount = 0;
    [Tooltip("MobCounter:" +
        "\n    X = Spawning\n    Y = Killed\n    Z = Escaped")]
    public Vector3Int mobCounts = new Vector3Int { x = 0, y = 0, z = 0 };
    public int mobsThisRound = 0;

    [Header("Base Price")]
    public int ninjaBasePrice = 70;
    public int wallBasePrice = 50;
    public int pitBasePrice = 20;
    public int mineBasePrice = 100;

    [Header("Current Price")]
    public int curNinjaCost = 70;
    public int curWallCost = 50;
    public int curPitCost = 20;
    public int curMineCost = 100;

    [Header("Unit Upgrade Price")]
    public int ninjaUpPrice = 50;
    public int wallUpPrice = 20;
    public int pitUpPrice = 10;
    public int mineUpPrice = 80;

    [Space(25, order = 0), Header("End Round Upgrades", order = 1)]
    [Space(-10, order = 2), Header("Melee Upgrades", order = 3)]
    public bool meleeCrit = false;
    public int meleeCRBase = 20;
    public int meleeCRUp = 2;
    public int meleeCRMax = 95;
    public int meleeCRCur = 10;
    public int meleeCRUpPriceBase = 100;
    public int meleeCRUpPriceCur = 100;

    [Header("Kuni Upgrades")]
    public bool kuniCrit = false;
    public int kuniCRBase = 10;
    public int kuniCRUp = 5;
    public int kuniCRMax = 95;
    public int kuniCRCur = 10;
    public int kuniCRUpPriceBase = 100;
    public int kuniCRUpCur = 100;

    [Header("Wall Upgrades")]
    public bool wallThorns = false;
    public bool thornsCrit = false;
    public int thornsCritRate = 15;

    [Header("Pit Upgrades")]
    public bool pitCorpsDecay = false;
    public int pitCDRate = 1;
    public int pitCDAdd = 1;

    [Header("Mine Upgrades")]
    public bool mineCrit = false;
    public int mineCritRate = 25;
    public int mineCRUp = 1;
    public int mineCRMax = 95;
}
