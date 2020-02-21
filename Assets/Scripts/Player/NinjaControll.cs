using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NinjaControll : MonoBehaviour
{
    // con fig vars
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
    GameObject kunai = null;

    [SerializeField, Space(10)]
    LayerMask enemyLayer = 9;

    // state vars
    public int hitPoints;
    GameObject activeNinja;
    GUIControll guiControll;
    Material myMaterial;
    bool spawning = true;
    int rank;
    Collider2D target;

    // state vars for the animator
    Animator anim;
    int rankHash = Animator.StringToHash("Rank");
    int hitHash = Animator.StringToHash("Hit");
    int attackHash = Animator.StringToHash("Attack");
    int meleeHash = Animator.StringToHash("Melee");
    int deathHash = Animator.StringToHash("Dead");

    private void Awake()
    {
        LinkGUI();

        foreach (GameObject ninja in ninjas)
        {
            ninja.SetActive(false);
        }
        attackPoint.SetActive(false);
        rankInsignia.SetActive(false);

        hitPoints = baseHP;
        rank = 0;
    }

    private void LinkGUI()
    {
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();

        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiControll = guiTest;
        }
    }

    void Start()
    {
        UpdateNinjas(true);
    }

    public void UpdateNinjas(bool spawn)
    {
        if (guiControll)
        {
            if (spawn)
            {
                guiControll.ninjaCount++;

                if (!guiControll.BuyNinja()) { Destroy(gameObject); guiControll.ninjaCount--; return; }

                SpawnNinja();
            }
            else if (!spawn)
            {
                var priceManager = guiControll.GetComponent<PriceManager>();
                var tutCon = guiControll.GetComponent<TutControll>();

                guiControll.ninjaCount--;
                int newPrice = priceManager.curNinjaCost -= priceManager.ninjaBasePrice;

                guiControll.UpdateMaxNinjas();
                priceManager.UpdateNinjaPrice(newPrice);
                tutCon.ToggleNinja(priceManager.curNinjaCost <= guiControll.curCoinCount && guiControll.ninjaCount < guiControll.maxNinjas);
            }
        }
    }

    void SpawnNinja()
    {
        activeNinja = ninjas[RandInt(1)];
        myMaterial = activeNinja.GetComponent<SpriteRenderer>().material;
        myMaterial.SetColor("_EdgeColor", myMaterial.GetColor("_SpawnColor"));
        myMaterial.SetFloat("_Fade", 1f);
        myMaterial.SetFloat("_UpgradeVFX", 0f);
        anim = activeNinja.GetComponent<Animator>();
        attackPoint.SetActive(true);
        activeNinja.SetActive(true);
        StartCoroutine(Dissolve(1));
        SetSortingOrder();
    }

    // gets a random int between 0 and max
    private static int RandInt(int max)
    {
        max += 1;
        return Random.Range(0, max);
    }

    private void SetSortingOrder()
    {
        int sortOrder = 6 - Mathf.FloorToInt(transform.position.y);
        activeNinja.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        rankInsignia.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }

    public void FinishSpawn()
    {
        rankInsignia.SetActive(true);
        SetStats(0);
    }

    public void SetStats(int rank)
    {
        if (!spawning) { StartCoroutine(UpgradeVFX()); }
        if (rank > 3) { rank = 3; }
        hitPoints = baseHP + (baseHP * rank);
        anim.SetInteger(rankHash, rank);
        rankInsignia.GetComponent<RankManager>().SetInsignia(rank);
    }

    public int GetRank()
    {
        return rank;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            anim.SetBool(deathHash, true);
        }
    }

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

    public void ThrowKunai()
    {
        if (kunai)
        {
            GameObject newKunai = Instantiate(kunai, attackPoint.transform.position, Quaternion.identity);
        }
    }

    public void HandleHit(int damage)
    {
        anim.SetTrigger(hitHash);
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            myMaterial.SetInt("_Upgradeable", 0);
            anim.SetBool(deathHash, true);
        }
    }

    public void HandleDeath()
    {
        GetComponent<TrackPlayerObjs>().HandleDeath();
        UpdateNinjas(false);
        StartCoroutine(Dissolve(0));
    }

    IEnumerator Dissolve(float fade)
    {
        yield return new WaitForSeconds(.3f);

        if (spawning)
        {
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
        }
        else
        {
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

    IEnumerator UpgradeVFX()
    {
        bool ug = true;
        float ugv = 0;
        float speed = myMaterial.GetFloat("_UpgradeSpeed");

        Debug.Log(speed);

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

        ug = true;

        while (ug)
        {
            ugv -= Time.deltaTime * speed;

            if (ugv <= 0)
            {
                ugv = 0;
                ug = false;
            }

            myMaterial.SetFloat("_UpgradeVFX", ugv);

            yield return null;
        }
    }
}
