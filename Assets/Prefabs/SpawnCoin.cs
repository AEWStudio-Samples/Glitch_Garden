using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCoin : MonoBehaviour
{
    [SerializeField]
    GameObject coin = null;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnNewCoin());
    }

    private IEnumerator SpawnNewCoin()
    {
        yield return new WaitForSeconds(2);
        GameObject newCoin = Instantiate(coin);
        newCoin.transform.position = RandSpawnLocation();
        StartCoroutine(SpawnNewCoin());
    }

    // Update is called once per frame
    Vector3 RandSpawnLocation()
    {
        float randX = Random.Range(-.5f, .5f);
        float randY = Random.Range(-.5f, .5f);

        Vector3 spawnOffSet = new Vector3(randX, randY, 0);

        return transform.position - spawnOffSet;
    }
}
