using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieControll : MonoBehaviour
{
    // con fig vars
    [SerializeField, Space(10)]
    GameObject[] zombies = { };
    [SerializeField, Space(10)]
    GameObject attackPoint = null;
    [SerializeField, Space(10)]
    GameObject rankInsignia = null;
    [SerializeField, Space(10)]

    // state vars
    int playerLayer = 1 << 8;
    int enemyLayer = 1 << 9;
    Rigidbody2D myBody;
    GameObject activeZomb;
    GUIControll guiControll;

    float speed = .5f;

    // state vars for the animator
    Animator anim;
    int speedHash = Animator.StringToHash("Speed");
    int rankHash = Animator.StringToHash("Rank");
    int rankUpHash = Animator.StringToHash("Rank Up");
    int blockHash = Animator.StringToHash("Blocked");
    int frezeHash = Animator.StringToHash("Freeze");
    int frozenHash = Animator.StringToHash("Frozen");
    int killHash = Animator.StringToHash("Kill");

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
        anim = activeZomb.GetComponent<Animator>();
        attackPoint.SetActive(true);
        activeZomb.SetActive(true);
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
        float distX = 9 - attackPoint.transform.position.x;

        RaycastHit2D hit = Physics2D.Raycast(attackPoint.transform.position, Vector2.left, 0.25f, playerLayer);
        if (hit)
        {
            Debug.DrawRay(attackPoint.transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.red);
            anim.SetBool(blockHash, true);
        }
        else
        {
            Debug.DrawRay(attackPoint.transform.position, transform.TransformDirection(Vector3.left) * 0.25f, Color.white);
            anim.SetBool(blockHash, false);
        }

        RaycastHit2D freeze = Physics2D.Raycast(attackPoint.transform.position, Vector2.left, 0.25f, enemyLayer);
        if (freeze)
        {
            Debug.DrawRay(attackPoint.transform.position, transform.TransformDirection(Vector3.left) * freeze.distance, Color.green);
            if (!anim.GetBool(frozenHash))
            {
                anim.SetTrigger(frezeHash);
            }
            anim.SetBool(frozenHash, true);
        }
        else
        {
            Debug.DrawRay(attackPoint.transform.position, transform.TransformDirection(Vector3.left) * 0.25f, Color.white);
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

    public void FinishSpawn()
    {
        rankInsignia.SetActive(true);
        anim.SetFloat(speedHash, speed);
    }

    public void Walk()
    {
        myBody.velocity = Vector2.left * anim.GetFloat(speedHash);
    }
}
