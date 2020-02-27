﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the script that controls the zombie.
/// </summary>
public class ZombieControll : MonoBehaviour
{
    // con fig vars //
    [SerializeField, Space(10)]
    int hitPoints = 6;

    [SerializeField, Space(10)]
    int damage = 2;

    [SerializeField, Space(10)]
    GameObject[] zombies = { };

    [SerializeField, Space(10)]
    GameObject attackPoint = null;

    [SerializeField, Space(10)]
    GameObject rankInsignia = null;

    [SerializeField, Space(10)]
    LayerMask playerLayer = 8;

    [SerializeField, Space(10)]
    LayerMask enemyLayer = 9;

    // state vars //
    Rigidbody2D myBody;
    GameObject activeZomb;
    GUIControll guiCon;
    Material myMaterial;
    bool spawning = true;
    float speed = .5f;
    int rank;

    Collider2D target;

    // state vars for the animator //
    Animator anim;
    int speedHash = Animator.StringToHash("Speed");
    int blockHash = Animator.StringToHash("Blocked");
    int frezeHash = Animator.StringToHash("Freeze");
    int frozenHash = Animator.StringToHash("Frozen");
    int killHash = Animator.StringToHash("Kill");
    int deathHash = Animator.StringToHash("Dead");

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Find out what we walked in to //
        switch(collision.tag)
        {
            // Hit by a kunai //
            case "Kunai":
                KunaiControll kunai = collision.GetComponent<KunaiControll>();
                HandleHit(kunai.damage);
                Destroy(kunai.gameObject);
                break;
            // Walked in to a pit //
            case "Pit":
                PitControll pit = collision.GetComponent<PitControll>();
                break;
            // Walked in to a mine //
            case "Mine":
                MineControll mine = collision.GetComponent<MineControll>();
                mine.Detonate(gameObject);
                break;
        }
    }

    private void Awake()
    {
        // Get GUI //
        LinkGUI();

        // Initialize For Spawning //
        myBody = GetComponent<Rigidbody2D>();
        foreach (GameObject zombie in zombies)
        {
            zombie.SetActive(false);
        }
        attackPoint.SetActive(false);
        rankInsignia.SetActive(false);
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
        UpdateZombies(true);
    }

    public void UpdateZombies(bool spawn)
    {
        if (guiCon)
        {
            if (spawn)
            {
                // Run some fun checks to see if a zombie can be spawned //

                SpawnZombie();
            }
            else if (!spawn)
            {
                // Update everything to account for the zombies death //
            }
        }
    }

    void SpawnZombie()
    {
        // Set Active Zombie //
        activeZomb = zombies[RandInt(1)];

        // Get My Material //
        myMaterial = activeZomb.GetComponent<SpriteRenderer>().material;

        // Set For Spawning //
        myMaterial.SetColor("_EdgeColor", myMaterial.GetColor("_SpawnColor"));
        myMaterial.SetFloat("_Fade", 1f);
        myMaterial.SetFloat("_UpgradeVFX", 0f);
        myMaterial.SetInt("_Upgradeable", 0);

        // Get Active Animator //
        anim = activeZomb.GetComponent<Animator>();

        // Activate Zombie //
        attackPoint.SetActive(true);
        activeZomb.SetActive(true);
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
        activeZomb.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        rankInsignia.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }

    public void FinishSpawn()
    {
        rankInsignia.SetActive(true);
        SetStats();
        anim.SetFloat(speedHash, speed);
    }

    private void SetStats()
    {
        if (guiCon.curRound > 4)
        {
            rank = RandInt(3) + guiCon.curRound - 4;
        }
        else
        {
            rank = RandInt(guiCon.curRound - 1);
        }

        // Set the zombie's health //
        hitPoints += hitPoints * rank;
        damage += (damage / 2) * rank;

        // Update the rank insignia //
        rankInsignia.GetComponent<RankManager>().SetInsignia(rank);
    }

    void Update()
    {
        // Kills all zombies while playing in the editor //
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            anim.SetBool(killHash, true);
            anim.SetBool(deathHash, true);
        }
        #endif
    }

    // Checks the lane for a valid target //
    public void CheckLane()
    {
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.transform.position, Vector2.left, 0.25f, playerLayer);
        if (hit)
        {
            // Attack ninjas and walls //
            anim.SetBool(blockHash, true);
            target = hit.collider;
        }
        else
        {
            anim.SetBool(blockHash, false);
        }

        RaycastHit2D freeze = Physics2D.Raycast(attackPoint.transform.position, Vector2.left, 0.25f, enemyLayer);
        if (freeze)
        {
            if (!anim.GetBool(frozenHash))
            {
                anim.SetTrigger(frezeHash);
            }
            anim.SetBool(frozenHash, true);
        }
        else
        {
            anim.SetBool(frozenHash, false);
        }

        if (anim.GetBool(blockHash) || anim.GetBool(frozenHash))
        {
            myBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            myBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // Applies damage to target from a melee attack //
    public void Attack()
    {
        switch (target.tag)
        {
            case "Ninja":
                target.GetComponent<NinjaControll>().HandleHit(damage);
                break;
            case "Wall":
                target.GetComponent<WallControll>().HandleHit(damage);
                break;
        }
    }

    // Makes the zombie move //
    public void Walk()
    {
        myBody.velocity = Vector2.left * anim.GetFloat(speedHash);
    }

    // Applies damage when zombie gets hit //
    public void HandleHit(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            anim.SetTrigger(killHash);
            anim.SetBool(deathHash, true);
        }
    }

    // The zombie has died //
    public void HandleDeath()
    {
        GetComponent<CoinSpawner>().StartCoinSpawn(rank);
        myBody.velocity = Vector2.left * 0;
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
}
