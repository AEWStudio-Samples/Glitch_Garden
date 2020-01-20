using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NinjaControll : MonoBehaviour
{
    // con fig vars
    [SerializeField, Space(10)]
    GameObject[] ninjas = { };
    [SerializeField, Space(10)]
    GameObject attackPoint = null;
    [SerializeField, Space(10)]
    GameObject rankInsignia = null;
    [SerializeField, Space(10)]
    GameObject kunai = null;
    [SerializeField, Space(10)]
    LayerMask enemyLayers = 9;

    // state vars
    GameObject activeNinja;
    GUIControll guiControll;
    PriceManager priceManager;

    // state vars for the animator
    Animator anim;
    int rankHash = Animator.StringToHash("Rank");
    int rankUpHash = Animator.StringToHash("Rank Up");
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
                priceManager = FindObjectOfType<PriceManager>();

                guiControll.ninjaCount--;

                priceManager.curNinjaCost -= priceManager.ninjaBasePrice;
            }
        }
    }

    void SpawnNinja()
    {
        activeNinja = ninjas[RandInt(1)];
        anim = activeNinja.GetComponent<Animator>();
        attackPoint.SetActive(true);
        activeNinja.SetActive(true);
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

    void Update()
    {
        float distX = 9 - attackPoint.transform.position.x;

        RaycastHit2D hit = Physics2D.Raycast(attackPoint.transform.position, Vector2.right, distX, enemyLayers);

        if (hit)
        {
            if (hit.distance <= 1)
            {
                Debug.DrawRay(attackPoint.transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.red);
                anim.SetBool(meleeHash, true);
                anim.SetTrigger(attackHash);
            }
            else
            {
                Debug.DrawRay(attackPoint.transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow);
                anim.SetBool(meleeHash, false);
                anim.SetTrigger(attackHash);
            }
        }
        else
        {
            Debug.DrawRay(attackPoint.transform.position, transform.TransformDirection(Vector3.right) * distX, Color.white);
        }
    }

    public void FinishSpawn()
    {
        rankInsignia.SetActive(true);
    }

    public void ThrowKunai()
    {
        if (kunai)
        {
            GameObject newKunai = Instantiate(kunai, attackPoint.transform.position, Quaternion.identity);
        }
    }
}
