using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentTracker : MonoBehaviour
{
    // state vars //
    [Space(25, order = 0), Header("Mid Round Upgrades", order = 1)]
    [Space(-10, order = 2), Header("Round Information", order = 3)]
    public int curRound = 0;

    [Header("Starting Values")]
    public int startCoinCount = 200;
    public Vector2Int mobSpawnCountBase = new Vector2Int { x = 5, y = 0 };

    [Header("Counters")]
    public int curCoinCount = 0;
    public int maxNinjas = 0;
    public int maxNinjasBase = 3;
    public int ninjaCount = 0;
    [Tooltip("MobCounter:" +
        "\n    X = Spawning\n    Y = Escaped")]
    public Vector2Int mobCounts = new Vector2Int { x = 0, y = 0 };
    public int mobsThisRound = 0;

    [Header("Base Price")]
    public int ninjaBasePrice = 70;
    public int wallBasePrice = 20;
    public int pitBasePrice = 50;
    public int mineBasePrice = 100;

    [Header("Current Price")]
    public int curNinjaCost = 70;
    public int curWallCost = 20;
    public int curPitCost = 50;
    public int curMineCost = 100;

    [Header("Unit Upgrade Price")]
    public int ninjaUpPrice = 50;
    public int wallUpPrice = 10;
    public int pitUpPrice = 20;
    public int mineUpPrice = 80;

    [Space(25, order = 0), Header("End Round Upgrades", order = 1)]
    [Space(-10, order = 2), Header("Critical Hit Toggle", order = 3)]
    public bool meleeCrit = false;
    public bool kuniCrit = false;
    public bool wallCrit = false;
    public bool mineCrit = false;

    [Header("Current Crit Rates")]
    public int meleeCRCur = 20;
    public int kuniCRCur = 10;
    public int WallCRCur = 15;
    public int mineCRCur = 25;

    [Header("Crit Rate Base")]
    public int meleeCRBase = 20;
    public int kuniCRBase = 10;
    public int wallCRBase = 15;
    public int mineCRBase = 25;

    [Header("Crit Rate Boost Per Upgrade")]
    public int meleeCRUp = 2;
    public int kuniCRUp = 5;
    public int wallCRUp = 1;
    public int mineCRUp = 1;

    [Header("Max Crit Rate")]
    public int meleeCRMax = 95;
    public int kuniCRMax = 95;
    public int wallCRMax = 95;
    public int mineCRMax = 95;

    [Header("Current Crit Rate Upgrade Price")]
    public int meleeCRCurUpPrice = 100;
    public int kuniCRCurUpPrice = 100;
    public int wallCRCurUpPrice = 100;
    public int mineCRCurUpPrice = 100;

    [Header("Base Crit Rate Upgrade Price")]
    public int meleeCRBaseUpPrice = 100;
    public int kuniCRBaseUpPrice = 100;
    public int wallCRBaseUpPrice = 100;
    public int mineCRBaseUpPrice = 100;

    [Header("Current Crit Multiplier")]
    public int meleeCMPCur = 2;
    public int kuniCMPCur = 2;
    public int wallCMPCur = 2;
    public int mineCMPCur = 2;

    [Header("Base Crit Multiplier")]
    public int meleeCMPBase = 2;
    public int kuniCMPBase = 2;
    public int wallCMPBase = 2;
    public int mineCMPBase = 2;

    [Header("Max Crit Multiplier")]
    public int meleeCMPMax = 5;
    public int kuniCMPMax = 5;
    public int wallCMPMax = 5;
    public int mineCMPMax = 5;

    [Header("Current Crit Multiplier Upgrade Price")]
    public int meleeCMPCurUpPrice = 100;
    public int kuniCMPCurUpPrice = 100;
    public int wallCMPCurUpPrice = 100;
    public int mineCMPCurUpPrice = 100;

    [Header("Base Crit Multiplier Upgrade Price")]
    public int meleeCMPBaseUpPrice = 100;
    public int kuniCMPBaseUpPrice = 100;
    public int wallCMPBaseUpPrice = 100;
    public int mineCMPBaseUpPrice = 100;

    [Header("Kuni Upgrades")]
    public int kuniDMGBoost = 1;
    public int kuniDMGBUpBasePrice = 100;
    public int kuniDMGBUpCurPrice = 100;
    public int kuniPirceCnt = 0;
    public int kuniPCntUpPriceBase = 100;
    public int kuniPCntUpPriceCur = 100;

    [Header("Wall Upgrades")]
    public bool wallThorns = false;

    [Header("Pit Upgrades")]
    public bool pitCorpsDecay = false;
    public int pitCDRate = 1;
    public int pitCDAdd = 1;

    [Header("Mine Upgrades")]
    public int mineCMP = 2;
}
