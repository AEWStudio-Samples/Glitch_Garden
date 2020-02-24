using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This is the script that controls the mine.
/// </summary>
public class MineControll : MonoBehaviour
{
    // con fig vars //
    [SerializeField, Space(10)]
    int baseCharge = 5;

    [SerializeField, Space(10)]
    GameObject mine = null;

    [SerializeField, Space(10)]
    GameObject chargeCounter = null;

    [SerializeField, Space(10)]
    GameObject blast = null;

    // state vars //
    GUIControll guiCon;
    Material myMaterial;
    TextMeshPro counter;

    int maxCharge;
    int curCharge;
    bool spawning = true;
    int rank;

    private void Awake()
    {
        // Get GUI //
        LinkGUI();

        // Initialize For Spawning //
        mine.SetActive(false);
        chargeCounter.SetActive(false);
        counter = chargeCounter.GetComponent<TextMeshPro>();
        maxCharge = baseCharge;
        rank = 0;
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

    void Start()
    {
        UpdateMine(true);
    }

    public void UpdateMine(bool spawn)
    {
        if (guiCon)
        {
            if (spawn)
            {
                guiCon.ninjaCount++;

                // Run some fun checks to see if a mine can be spawned //
                if (!guiCon.BuyMine()) { Destroy(gameObject); return; }

                SpawnPit();
            }
            else if (!spawn)
            {
                // Update everything to account for the mines destruction //
                chargeCounter.SetActive(false);

                var priceManager = guiCon.GetComponent<PriceManager>();

                int newPrice = priceManager.curMineCost -= priceManager.mineBasePrice;

                priceManager.UpdateMinePrice(newPrice);
            }
        }
    }

    void SpawnPit()
    {
        // Get My Material //
        myMaterial = mine.GetComponent<SpriteRenderer>().material;

        // Set For Spawning //
        myMaterial.SetColor("_EdgeColor", myMaterial.GetColor("_SpawnColor"));
        myMaterial.SetFloat("_Fade", 1f);
        myMaterial.SetFloat("_UpgradeVFX", 0f);

        // Activate Mine //
        mine.SetActive(true);
        StartCoroutine(Dissolve(1));
        SetSortingOrder();
    }

    private void SetSortingOrder()
    {
        // This makes it so objects layer properly //
        int sortOrder = 6 - Mathf.FloorToInt(transform.position.y);
        mine.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }

    public void FinishSpawn()
    {
        chargeCounter.SetActive(true);
        SetStats(0);
    }

    public void SetStats(int rank)
    {
        this.rank = rank;
        int roundBonus = 0;
        int addCharge;

        // Set Upgrade Cost //
        var upgradeCost = guiCon.GetComponent<PriceManager>().mineUpPrice;
        GetComponent<UpgradeManager>().SetUpgradeCost(upgradeCost, rank);

        // Trigger the upgrade VFX if this method is called after spawning is done //
        if (!spawning) { StartCoroutine(UpgradeVFX()); }

        // Set the mine's capacity //
        if (guiCon.curRound > 4 && spawning) { roundBonus = baseCharge * (guiCon.curRound - 4); }
        addCharge = (baseCharge * rank) + roundBonus;
        maxCharge = baseCharge + addCharge;
        curCharge = baseCharge + addCharge;
        SetCounterText(curCharge);
    }

    private void SetCounterText(int count)
    {
        if (count == 0) { DestroyMine(); }
        counter.text = $"{count}/{maxCharge}";
    }

    public int GetRank()
    {
        return rank;
    }

    private void Update()
    {
        // Kills all pits while playing in the editor //
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            myMaterial.SetInt("_Upgradeable", 0);
        }
        #endif
    }

    // The pit has died //
    public void DestroyMine()
    {
        GetComponent<TrackPlayerObjs>().HandleDeath();
        UpdateMine(false);
        StartCoroutine(Dissolve(0));
    }

    // Applies VFX for spawning and death //
    IEnumerator Dissolve(float fade)
    {
        yield return new WaitForSeconds(.3f);

        if (spawning)
        {
            // Spawn into the scene //
            while (spawning)
            {
                fade -= Time.deltaTime;

                if (fade <= 0)
                {
                    fade = 0;
                    spawning = false;
                }

                myMaterial.SetFloat("_Fade", fade);

                yield return null;
            }

            myMaterial.SetColor("_EdgeColor", myMaterial.GetColor("_FadeColor"));
            FinishSpawn();
        }
        else
        {
            // Destroy the pit //
            bool death = true;
            while (death)
            {
                fade += Time.deltaTime;

                if (fade >= 1)
                {
                    fade = 1;
                    death = false;
                }

                myMaterial.SetFloat("_Fade", fade);

                yield return null;
            }

            Destroy(gameObject);
        }
    }

    // Applies VFX for being upgraded //
    IEnumerator UpgradeVFX()
    {
        bool ug = true;
        float ugv = 0;
        float speed = myMaterial.GetFloat("_UpgradeSpeed");

        while (ug)
        {
            ugv += Time.deltaTime * speed;

            if (ugv >= 1)
            {
                ugv = 1;
                ug = false;
            }

            myMaterial.SetFloat("_UpgradeVFX", ugv);

            yield return null;
        }

        // Reset for next upgrade //
        myMaterial.SetFloat("_UpgradeVFX", 0);
    }
}
