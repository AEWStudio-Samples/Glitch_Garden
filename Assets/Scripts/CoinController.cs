using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    // con fig vars
    [Header("Value")]
    [SerializeField]
    int bronze = 10;
    [SerializeField]
    int Silver = 20, gold = 30;
    [Header("Collect")]
    [SerializeField]
    float speed = .5f;
    [Header("FX")]
    [SerializeField]
    ParticleSystem spawnFX = null;
    float spawnDelay = .5f;

    // state vars
    Animator anim;
    int coinHash = Animator.StringToHash("Coin Type");
    Canvas gui;
    Vector2 mousePos;

    private bool isClicked = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger(coinHash, RandInt());
        StartCoroutine(SpawnFX());

        Canvas[] canvasList = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvasList)
        {
            if (canvas.CompareTag("GUI")) gui = canvas;
        }
    }

    private static int RandInt()
    {
        return Random.Range(1, 4);
    }

    IEnumerator SpawnFX()
    {
        yield return new WaitForSeconds(spawnDelay);
        spawnFX.Play();
        Destroy(spawnFX, spawnDelay);
    }

    void Update()
    {
        if (isClicked) print(mousePos);
    }

    void CollectCoin()
    {
        transform.Translate(new Vector3(1, 5, 0));
    }

    private void OnMouseDown()
    {
        CollectCoin();
    }
}
