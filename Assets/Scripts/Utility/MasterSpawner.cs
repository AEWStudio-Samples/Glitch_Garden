using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This is the script for the master spawner.
/// </summary>
public class MasterSpawner : MonoBehaviour
{
    // con fig vars //
    [Header("Spawn Parameters")]
    [SerializeField]
    float roundStartDelay = 10f;
    [SerializeField]
    int spawnCount = 30;
    [SerializeField, Tooltip("Delay between spawns in seconds: min = x; max = y")]
    Vector2 spawnDelay = new Vector2 { x = 1.0f, y = 3.0f };
    [SerializeField]
    GameObject[] spawnPoints = { };
    [SerializeField]
    GameObject[] mobs = { };
    [SerializeField]
    int[] spawnChance = { 100, -5};
    [SerializeField]
    int[] spawnChanceInc = { 0, 5 };

    // state vars //
    List<int> spawnMobList = new List<int> { };
    List<int> spawnPointList = new List<int> { };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRound()
    {
        StartCoroutine(SetSpawnList());
    }

    IEnumerator SetSpawnList()
    {
        yield return new WaitForSeconds(roundStartDelay);

        for (int i = 0; i < spawnChance.Length; i++)
        {
            if (spawnChance[i] < 100)
            {
                spawnChance[i] += spawnChanceInc[i];
                if (spawnChance[i] > 100)
                {
                    spawnChance[i] = 100;
                }
            }
        }

        for (int i = 0; i < spawnCount; i++)
        {
            spawnMobList.Add(CheckMobSpawn(Random.Range( 0, mobs.Length)));
            spawnPointList.Add(Random.Range(0, spawnPoints.Length));
            print(spawnPointList[i]);
        }
    }

    private int CheckMobSpawn(int type)
    {
        if (Random.Range(1, 101) < spawnChance[type]) { return type; }
        return 0;
    }
}
