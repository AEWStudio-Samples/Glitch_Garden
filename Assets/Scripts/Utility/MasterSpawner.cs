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
    int spawnCount = 30;
    [SerializeField, Tooltip("Delay between spawns in seconds: min add = x; max add = y; z = start delay; w = min time for a round")]
    Vector3 spawnDelay = new Vector3 { x = 2.0f , y = 10f, z = 60f};
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

    public void StartRound(int cnt)
    {
        spawnCount = cnt;
        SetSpawnList();
        StartCoroutine(SpawnMobs());
    }

    private void SetSpawnList()
    {
        SetSpawnChance();

        for (int i = 0; i < spawnCount; i++)
        {
            spawnMobList.Add(CheckMobSpawn(Random.Range(0, mobs.Length)));
            spawnPointList.Add(Random.Range(0, spawnPoints.Length));
        }
    }

    private void SetSpawnChance()
    {
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
    }

    private int CheckMobSpawn(int type)
    {
        if (Random.Range(1, 101) < spawnChance[type]) { return type; }
        return 0;
    }

    IEnumerator SpawnMobs()
    {
        int i = 0;
        float stMaxAdd = spawnDelay.x;          // spawn time max add //
        float sd = spawnDelay.y;                // delay before start //
        float st = spawnDelay.z / spawnCount;   // time between spawn //

        yield return new WaitForSeconds(sd);

        while (i < spawnCount)
        {
            GameObject mob = mobs[spawnMobList[i]];
            GameObject spm = spawnPoints[spawnPointList[i]];
            Vector3 sp = new Vector3(spm.transform.position.x, spm.transform.position.y, spm.transform.position.z);
            Instantiate<GameObject>(mob, sp, Quaternion.identity);
            i++;
            yield return new WaitForSeconds(Random.Range(st, st + stMaxAdd));
        }
    }
}
