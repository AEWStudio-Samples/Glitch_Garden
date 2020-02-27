using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the script that controls the wall.
/// </summary>
public class WallControll : MonoBehaviour
{
    // con fig vars //
    [SerializeField, Space(10)]
    int baseHP = 10;

    [SerializeField, Space(10)]
    GameObject wall = null;

    [SerializeField, Space(10)]
    GameObject rankInsignia = null;

    [SerializeField, Space(10)]
    GameObject cracks = null;

    // state vars //
    GUIControll guiCon;
    Material myMaterial;
    Material crackMaterial;

    int hitPoints;
    bool spawning = true;
    int rank;

    private void Awake()
    {
        // Get GUI //
        LinkGUI();

        // Initialize For Spawning //
        wall.SetActive(false);
        rankInsignia.SetActive(false);
        cracks.SetActive(false);

        hitPoints = baseHP;
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
        UpdateWall(true);
    }

    public void UpdateWall(bool spawn)
    {
        if (guiCon)
        {
            if (spawn)
            {
                // Run some fun checks to see if a wall can be spawned //
                if (!guiCon.BuyWall() && !guiCon.debugging) { Destroy(gameObject); return; }

                SpawnWall();
            }
            else if (!spawn)
            {
                // Update everything to account for the walls death //
                rankInsignia.SetActive(false);

                var priceManager = guiCon.GetComponent<PriceManager>();

                int newPrice = priceManager.curWallCost -= priceManager.wallBasePrice;

                guiCon.UpdateMaxNinjas();
                priceManager.UpdateWallPrice(newPrice);
            }
        }
    }

    void SpawnWall()
    {
        // Get My Material //
        myMaterial = wall.GetComponent<SpriteRenderer>().material;

        // Set For Spawning //
        myMaterial.SetColor("_EdgeColor", myMaterial.GetColor("_SpawnColor"));
        myMaterial.SetFloat("_Fade", 1f);
        myMaterial.SetFloat("_UpgradeVFX", 0f);

        // Activate Wall //
        wall.SetActive(true);
        StartCoroutine(Dissolve(1));
        SetSortingOrder();
    }

    // Gets a random int between 0 and max //
    private static int RandInt(int max)
    {
        // Add 1 to max so that it is in the range of possible outputs //
        max++;
        return Random.Range(0, max);
    }

    private void SetSortingOrder()
    {
        // This makes it so objects layer properly //
        int sortOrder = 6 - Mathf.FloorToInt(transform.position.y);
        wall.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        rankInsignia.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }

    public void FinishSpawn()
    {
        rankInsignia.SetActive(true);
        cracks.SetActive(true);
        crackMaterial = cracks.GetComponent<SpriteRenderer>().material;
        SetStats(0);
    }

    public void SetStats(int rank)
    {
        this.rank = rank;
        int roundHP = 0;
        int addHP;

        // Set Upgrade Cost //
        var upgradeCost = guiCon.GetComponent<PriceManager>().wallUpPrice;
        GetComponent<UpgradeManager>().SetUpgradeCost(upgradeCost, rank);

        // Trigger the upgrade VFX if this function is called after spawning is done //
        if (!spawning) { StartCoroutine(UpgradeVFX()); }

        // Set the wall's health //
        if (guiCon.curRound > 4) { roundHP = baseHP * (guiCon.curRound - 4); }
        addHP = (baseHP * rank) + roundHP;
        hitPoints = baseHP + addHP;

        // Update Cracks //
        UpdateCracks(true);

        // Update the rank insignia //
        if (rank > 3) { rank = 3; }
        rankInsignia.GetComponent<RankManager>().SetInsignia(rank);
    }

    public int GetRank()
    {
        return rank;
    }

    private void Update()
    {
        // Kills all walls while playing in the editor //
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            myMaterial.SetInt("_Upgradeable", 0);
            DestroyWall();
        }
        #endif
    }

    // Applies damage when wall gets hit //
    public void HandleHit(int damage)
    {
        hitPoints -= damage;

        // Update Cracks //
        UpdateCracks(false);

        if (hitPoints <= 0)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            myMaterial.SetInt("_Upgradeable", 0);
            DestroyWall();
        }
    }

    // Update the heart sprite //
    private void UpdateCracks(bool refresh)
    {
        if (refresh)
        {
            crackMaterial.SetInt("_MaxHP", hitPoints);
        }

        crackMaterial.SetInt("_CurrentHP", hitPoints);
    }

    // The wall has died //
    public void DestroyWall()
    {
        GetComponent<TrackPlayerObjs>().HandleDeath();
        UpdateWall(false);
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
            // Destroy the wall //
            bool death = true;
            float crackFade;
            while (death)
            {
                fade += Time.deltaTime;
                crackFade = fade * crackMaterial.GetFloat("_MaxHP");

                if (fade >= 1)
                {
                    fade = 1;
                    death = false;
                }

                myMaterial.SetFloat("_Fade", fade);
                crackMaterial.SetFloat("_CurrentHP", crackFade);

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
