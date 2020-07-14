using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for spawning coins.
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    // con fig vars //
    [SerializeField]
    GameObject coin = null;

    public void StartCoinSpawn(int coinMax)
    {
        StartCoroutine(SpawnNewCoin(coinMax));
    }

    // gets a random int between 1 and max //
    private static int RandInt(int max)
    {
        max++;
        return Random.Range(1, max);
    }

    private IEnumerator SpawnNewCoin(int max)
    {
        int coinCount = RandInt(max);
        while (coinCount > 0)
        {
            GameObject newCoin = Instantiate(coin);
            newCoin.transform.position = RandSpawnLocation();
            coinCount--;
            yield return null;
        }
    }

    Vector3 RandSpawnLocation()
    {
        float randX = Random.Range(-.5f, .5f);
        float randY = Random.Range(-.5f, .5f);

        Vector3 spawnOffSet = new Vector3(randX, randY, 0);

        return transform.position - spawnOffSet;
    }
}
