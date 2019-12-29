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
    LayerMask enemies;

    // state vars
    GameObject activeNinja;

    private void Awake()
    {
        foreach (GameObject ninja in ninjas)
        {
            ninja.SetActive(false);
        }
        attackPoint.SetActive(false);
        rankInsignia.SetActive(false);
    }

    void Start()
    {
        SpawnNinja();
    }

    void SpawnNinja()
    {
        activeNinja = ninjas[RandInt(1)];
        attackPoint.SetActive(true);
        activeNinja.SetActive(true);
        SetSortingOrder();
    }

    private void SetSortingOrder()
    {
        int sortOrder = 6 - Mathf.FloorToInt(transform.position.y);
        activeNinja.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        rankInsignia.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }

    void Update()
    {

    }

    // gets a random int between 0 and max
    private static int RandInt(int max)
    {
        max += 1;
        return Random.Range(0, max);
    }

    public void FinishSpawn()
    {
        rankInsignia.SetActive(true);
    }
}
