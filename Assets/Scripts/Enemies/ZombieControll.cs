using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieControll : MonoBehaviour
{
    // con fig vars
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

    // state vars
    Rigidbody2D myBody;
    GameObject activeZomb;
    GUIControll guiControll;
    Material myMaterial;
    bool spawning = true;
    float speed = .5f;
    int rank;

    Collider2D target;

    // state vars for the animator
    Animator anim;
    int speedHash = Animator.StringToHash("Speed");
    int rankHash = Animator.StringToHash("Rank");
    int blockHash = Animator.StringToHash("Blocked");
    int frezeHash = Animator.StringToHash("Freeze");
    int frozenHash = Animator.StringToHash("Frozen");
    int killHash = Animator.StringToHash("Kill");
    int deathHash = Animator.StringToHash("Dead");

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();

        LinkGUI();

        foreach (GameObject zombie in zombies)
        {
            zombie.SetActive(false);
        }
        attackPoint.SetActive(false);
        rankInsignia.SetActive(false);
    }

    void Start()
    {
        UpdateZombies(true);
    }

    private void LinkGUI()
    {
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();

        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiControll = guiTest;
        }
    }

    public void UpdateZombies(bool spawn)
    {
        if (guiControll)
        {
            if (spawn)
            {
                //guiControll.ninjaCount++;

                SpawnZombie();
            }
            else if (!spawn)
            {
                //guiControll.ninjaCount--;
            }
        }
    }

    void SpawnZombie()
    {
        activeZomb = zombies[RandInt(1)];
        myMaterial = activeZomb.GetComponent<SpriteRenderer>().material;
        myMaterial.SetColor("_EdgeColor", myMaterial.GetColor("_SpawnColor"));
        myMaterial.SetFloat("_Fade", 1f);
        myMaterial.SetFloat("_UpgradeVFX", 0f);
        myMaterial.SetInt("_Upgradeable", 0);
        anim = activeZomb.GetComponent<Animator>();
        attackPoint.SetActive(true);
        activeZomb.SetActive(true);
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
        activeZomb.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        rankInsignia.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            anim.SetBool(killHash, true);
            anim.SetBool(deathHash, true);
        }
    }

    public void FinishSpawn()
    {
        rankInsignia.SetActive(true);
        SetStats();
        anim.SetFloat(speedHash, speed);
    }

    private void SetStats()
    {
        if (guiControll.curRound > 4)
        {
            rank = RandInt(3) + guiControll.curRound - 4;
        }
        else
        {
            rank = RandInt(guiControll.curRound - 1);
        }

        hitPoints += hitPoints * rank;
        damage += (damage / 2) * rank;

        anim.SetInteger(rankHash, rank);

        rankInsignia.GetComponent<RankManager>().SetInsignia(rank);
    }

    public void CheckLane()
    {
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.transform.position, Vector2.left, 0.25f, playerLayer);
        if (hit)
        {
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

    public void Walk()
    {
        myBody.velocity = Vector2.left * anim.GetFloat(speedHash);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Kunai")
        {
            KunaiControll kunai = collision.collider.GetComponent<KunaiControll>();
            HandleHit(kunai.damage);
            Destroy(kunai.gameObject);
        }
    }

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

    public void HandleDeath()
    {
        GetComponent<CoinSpawner>().StartCoinSpawn(rank);
        myBody.velocity = Vector2.left * 0;
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
}
