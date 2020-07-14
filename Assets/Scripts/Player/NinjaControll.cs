using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This is the script that controls the ninja.
/// </summary>
public class NinjaControll : MonoBehaviour
{
    // TODO add code for crit upgrade //
    // con fig vars //
    [SerializeField, Space(10)]
    int baseHP = 6;

    [SerializeField, Space(10)]
    public int damage = 2;
    
    [SerializeField, Space(10)]
    GameObject[] ninjas = { };

    [SerializeField, Space(10)]
    GameObject attackPoint = null;

    [SerializeField, Space(10)]
    GameObject rankInsignia = null;

    [SerializeField, Space(10)]
    GameObject heart = null;

    [SerializeField, Space(10)]
    GameObject kunai = null;

    [SerializeField, Space(10)]
    LayerMask enemyLayer = 9;

    // state vars //
    GameObject activeNinja;
    GUIControll guiCon;
    Material myMaterial;
    Material heartMaterial;

    int hitPoints;
    bool spawning = true;
    int rank;

    Collider2D target;

    // state vars for the animator //
    Animator anim;
    int rankHash = Animator.StringToHash("Rank");
    int hitHash = Animator.StringToHash("Hit");
    int attackHash = Animator.StringToHash("Attack");
    int meleeHash = Animator.StringToHash("Melee");
    int deathHash = Animator.StringToHash("Dead");

    private void Awake()
    {
        // Get GUI //
        LinkGUI();

        // Initialize For Spawning //
        foreach (GameObject ninja in ninjas)
        {
            ninja.SetActive(false);
        }
        attackPoint.SetActive(false);
        rankInsignia.SetActive(false);
        heart.SetActive(false);

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
        UpdateNinjas(true);
    }

    public void UpdateNinjas(bool spawn)
    {
        if (guiCon)
        {
            if (spawn)
            {
                guiCon.conTrack.ninjaCount++;

                // Run some fun checks to see if a ninja can be spawned //
                if (!guiCon.BuyNinja() && !guiCon.debugging)
                {
                    Destroy(gameObject); guiCon.conTrack.ninjaCount--;
                    return;
                }

                SpawnNinja();
            }
            else if (!spawn)
            {
                // Update everything to account for the ninjas death //
                rankInsignia.SetActive(false);
                heart.SetActive(false);

                guiCon.conTrack.ninjaCount--;
                int newPrice = guiCon.conTrack.curNinjaCost -= guiCon.conTrack.ninjaBasePrice;

                guiCon.UpdateMaxNinjas();
                guiCon.priceCon.UpdateNinjaPrice(newPrice);
            }
        }
    }

    void SpawnNinja()
    {
        // Set Active Ninja //
        activeNinja = ninjas[RandInt(1)];

        // Get My Material //
        myMaterial = activeNinja.GetComponent<SpriteRenderer>().material;

        // Set For Spawning //
        myMaterial.SetColor("_EdgeColor", myMaterial.GetColor("_SpawnColor"));
        myMaterial.SetFloat("_Fade", 1f);
        myMaterial.SetFloat("_UpgradeVFX", 0f);

        // Get Active Animator //
        anim = activeNinja.GetComponent<Animator>();

        // Activate Ninja //
        attackPoint.SetActive(true);
        activeNinja.SetActive(true);
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
        activeNinja.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        rankInsignia.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }

    public void FinishSpawn()
    {
        rankInsignia.SetActive(true);
        heart.SetActive(true);
        heartMaterial = heart.GetComponent<SpriteRenderer>().material;
        SetStats(0);
    }

    public void SetStats(int rank)
    {
        this.rank = rank;
        int roundHP = 0;
        int roundDMG = 0;
        int addHP = baseHP * rank;
        int addDMG = damage * rank;

        // Set Upgrade Cost //
        var upgradeCost = guiCon.conTrack.ninjaUpPrice;
        GetComponent<UpgradeManager>().SetUpgradeCost(upgradeCost, rank);

        // Trigger the upgrade VFX if this method is called after spawning is done //
        if (!spawning) { StartCoroutine(UpgradeVFX()); }

        // Set the ninja's health and damage //
        if (guiCon.conTrack.curRound > 4)
        {
            roundHP = baseHP * (guiCon.conTrack.curRound - 4);
            roundDMG = (damage / 2) * (guiCon.conTrack.curRound - 4);
         }
        addHP += roundHP;
        addDMG += roundDMG;
        hitPoints = baseHP + addHP;
        damage += addDMG;

        // Update Heart //
        UpdateHeart(true);

        // Update the animator and rank insignia //
        if (rank > 3) { rank = 3; }
        anim.SetInteger(rankHash, rank);
        rankInsignia.GetComponent<RankManager>().SetInsignia(rank);
    }

    public int GetRank()
    {
        return rank;
    }

    private void Update()
    {
        // Kills all ninjas while playing in the editor //
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            myMaterial.SetInt("_Upgradeable", 0);
            anim.SetBool(deathHash, true);
        }
        #endif
    }

    // Checks the lane for a valid target //
    public void CheckLane()
    {
        float distX = 9 - attackPoint.transform.position.x;

        RaycastHit2D hit = Physics2D.Raycast(attackPoint.transform.position, Vector2.right, distX, enemyLayer);

        if (hit)
        {
            if (hit.distance <= 1)
            {
                anim.SetBool(meleeHash, true);
                anim.SetTrigger(attackHash);
                target = hit.collider;
            }
            else
            {
                anim.SetBool(meleeHash, false);
                anim.SetTrigger(attackHash);
            }
        }
    }

    // Applies damage to target from a melee attack //
    public void Attack()
    {
        switch (target.tag)
        {
            case "Zombie":
                target.GetComponent<ZombieControll>().HandleHit(damage);
                break;
            case "Phantom":
                target.GetComponent<PhantomControll>().HandleHit(damage);
                break;
        }
    }

    // Checks an int to see if it is even or odd //
    private bool EvenOdd(int cnt)
    {
        if (cnt % 2 == 0)
        {
            return true;
        }
        return false;
    }

    // Throws a kunai //
    public void ThrowKunai()
    {
        if (kunai)
        {
            Transform spawnPnt = attackPoint.transform;

            var kunaiCnt = 1 + (guiCon.conTrack.curRound / 5);
            if (kunaiCnt > 5) { kunaiCnt = 5; }
            var spawnOff = kunai.GetComponent<KunaiControll>().spnOffset;
            int subCnt = 0;
            float newY;
            Vector3 newSpn;

            // Figure out were each kunai gets spawned //
            if (EvenOdd(kunaiCnt))
            {
                // Spawn each kunai offset from the attack point //
                while (kunaiCnt > 0)
                {
                    subCnt++;
                    if (EvenOdd(subCnt))
                    {
                        newY = spawnPnt.position.y + spawnOff;
                        newSpn = new Vector3(spawnPnt.position.x, newY, spawnPnt.position.z);
                        Instantiate(kunai, newSpn, Quaternion.identity);
                    }
                    else
                    {
                        newY = spawnPnt.position.y - spawnOff;
                        newSpn = new Vector3(spawnPnt.position.x, newY, spawnPnt.position.z);
                        Instantiate(kunai, newSpn, Quaternion.identity);
                    }

                    kunaiCnt--;

                    if (subCnt == 2) { subCnt = 0; spawnOff += spawnOff; }
                }
            }
            else
            {
                // Spawns first kunai at the attack point if kunaiCnt is odd //
                Instantiate(kunai, spawnPnt.position, Quaternion.identity);
                kunaiCnt--;
                spawnOff += .02f;

                // Spawn the remaining kunai offset from the attack point //
                while (kunaiCnt > 0)
                {
                    subCnt++;
                    if (EvenOdd(subCnt))
                    {
                        newY = spawnPnt.position.y + spawnOff;
                        newSpn = new Vector3(spawnPnt.position.x, newY, spawnPnt.position.z);
                        Instantiate(kunai, newSpn, Quaternion.identity);
                    }
                    else
                    {
                        newY = spawnPnt.position.y - spawnOff;
                        newSpn = new Vector3(spawnPnt.position.x, newY, spawnPnt.position.z);
                        Instantiate(kunai, newSpn, Quaternion.identity);
                    }

                    kunaiCnt--;

                    if (subCnt == 2) { subCnt = 0; spawnOff += spawnOff; }
                }
            }
        }
    }

    // Applies damage when ninja gets hit //
    public void HandleHit(int damage)
    {
        anim.SetTrigger(hitHash);
        hitPoints -= damage;

        // Update Heart //
        UpdateHeart(false);

        if (hitPoints <= 0)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            myMaterial.SetInt("_Upgradeable", 0);
            anim.SetBool(deathHash, true);
        }
    }

    // Update the heart sprite //
    private void UpdateHeart(bool refresh)
    {
        if (refresh)
        {
            heartMaterial.SetInt("_MaxHP", hitPoints);
        }

        heartMaterial.SetInt("_CurrentHP", hitPoints);
    }

    // The ninja has died //
    public void HandleDeath()
    {
        GetComponent<TrackPlayerObjs>().HandleDeath();
        UpdateNinjas(false);
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
            // Destroy the corps //
            while (anim.GetBool(deathHash))
            {
                fade += Time.deltaTime;

                if (fade >= 1)
                {
                    fade = 1;
                    anim.SetBool(deathHash, false);
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
