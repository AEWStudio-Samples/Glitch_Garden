using DG.Tweening;
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
    public bool endless;
    [Header("Spawn Parameters")]
    [SerializeField]
    int spawnCount = 30;
    [SerializeField, Tooltip("Timing for spawns in seconds:" +
        "\n    X = Delay Add\n    Y = Start Delay\n    Z = Min Spawn Cycle = z")]
    Vector3 spawnTime = new Vector3 { x = 2.0f , y = 10f, z = 60f};
    [SerializeField]
    GameObject[] spawnPoints = { };
    [SerializeField]
    GameObject[] mobs = { };
    [SerializeField]
    int[] spawnChance = { 15, -5};
    [SerializeField]
    int[] spawnChanceInc = { 0, 5 };
    [SerializeField]
    int[] spawnCntUsed = { 1, 1 };

    // state vars //
    bool firstWave = true;
    int cntUsed = 0;
    List<int> spawnMobList = new List<int> { };
    List<int[]> spawnGroups = new List<int[]> { };
    int[] spawnGroup = new int[5];

    public void StartRound(int cnt)
    {
        spawnCount = cnt;

        cntUsed = 0;
        spawnMobList = new List<int> { };
        spawnGroups = new List<int[]> { };
        SetSpawnList();
        StartCoroutine(SpawnMobs());
    }

    private void SetSpawnList()
    {
        SetSpawnChance();

        int newSpawnCnt = 0;

        for (int i = 0;  cntUsed < spawnCount; i++)
        {
            spawnMobList.Add(CheckMobSpawn(Random.Range(0, mobs.Length)));
            newSpawnCnt++;
        }

        spawnCount = newSpawnCnt;

        SetSpawnPoints();
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
        if (type == 0 || Random.Range(1, 101) > spawnChance[type]
            || spawnCount - cntUsed < spawnCntUsed[type])
        {
            type = 0;
        }

        cntUsed += spawnCntUsed[type];
        return type;
    }

    private void SetSpawnPoints()
    {
        spawnGroups = new List<int[]> { };

        int st = 0;
        int sp = 0;
        int su = 0;

        for (int i = 0; i < spawnMobList.Count; i += 0)
        {
            spawnGroup = new int[5];

            while (sp < 5 && i < spawnMobList.Count)
            {
                int testRange = Random.Range(1, 101);

                if (testRange < spawnChance[0])
                {
                    spawnGroup[sp] = 1;
                    su++;
                    i++;
                }
                else
                {
                    spawnGroup[sp] = 0;
                }

                sp++;
            }

            if (st == 0 && sp == 5 && su == 0 && i < spawnMobList.Count)
            {
                int temp = Random.Range(0, 5);
                spawnGroup[temp] = 1;
                i++;
            }

            if (i < spawnMobList.Count)
            {
                st = 0;
                su = 0;
            }

            if (sp > 0 && sp < 5 && i == spawnMobList.Count)
            {
                while (sp < 5)
                {
                    spawnGroup[sp] = 0;
                    sp++;
                }
            }

            if (sp == 5)
            {
                spawnGroups.Add(spawnGroup);
                sp = 0;
            }
        }

    }

    IEnumerator SpawnMobs()
    {
        int i = 0;
        int m = 0;
        int[] sg;
        int sp = 0;

        float stMaxAdd = spawnTime.x;          // spawn time max add //
        float sd = spawnTime.y;                // delay before start //
        float st = spawnTime.z / spawnCount;   // time between spawn //

        GameObject mob;
        GameObject spm;
        Vector3 spl;

        if (firstWave) { yield return new WaitForSeconds(sd); }

        while (i < spawnGroups.Count && m < spawnCount)
        {
            sg = spawnGroups[i];

            foreach (int t in sg)
            {
                if (t == 1)
                {
                    mob = mobs[spawnMobList[m]];
                    spm = spawnPoints[sp];
                    spl = spm.transform.position;

                    GameObject smob = Instantiate(mob, spl,
                        Quaternion.identity, spm.transform);
                    m++;
                }

                sp++;
            }

            sp = 0;
            i++;

            if (i < spawnGroups.Count)
            {
                yield return new WaitForSeconds(Random.Range(st, st + stMaxAdd));
            }
        }

        if (endless)
        {
            firstWave = false;
        }
    }
}
