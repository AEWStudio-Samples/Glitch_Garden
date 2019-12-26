using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinControll : MonoBehaviour
{
    // con fig vars
    [Header("Collect")]
    [SerializeField]
    int[] coinValues = { 10, 20, 30 };
    [SerializeField]
    float fallSpeed = .5f;
    [Header("FX")]
    [SerializeField]
    ParticleSystem spawnFX = null;

    // state vars
    float spawnDelay = .1f;
    SpriteRenderer myRender;
    Animator anim;
    int coinHash = Animator.StringToHash("Coin Type");
    Rigidbody2D myRigidbody;
    Canvas gui; // change to GUIControll

    // state vars for collecting the coin
    int coinValue;
    bool coinCollected = false;
    Vector3 moveFallBack = new Vector3(1, 1, 0);
    Vector3 moveTo;
    int moveX, moveY;

    private void Awake()
    {
        myRender = GetComponent<SpriteRenderer>();
        myRender.enabled = false;
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        SetCoinType();
        StartCoroutine(SpawnFX());
        LinkGUI();
    }

    private void SetCoinType()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger(coinHash, coinValue = RandInt(3));
        //print(coinValue);
    }

    // gets a random int between 1 and max
    private static int RandInt(int max)
    {
        max += 1;
        return Random.Range(1, max);
    }

    IEnumerator SpawnFX()
    {
        myRender.enabled = true;
        yield return new WaitForSeconds(spawnDelay);
        PlayFX();
        MoveCoin(true);
    }

    private void LinkGUI()
    {
        Canvas[] canvasList = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvasList)
        {
            if (canvas.CompareTag("GUI")) gui = canvas;
        }

        //if (gui) { moveTo = gui.GetCoinCollector().position }
        //else { moveTo = moveFallBack }
        moveTo = moveFallBack; // remove after GUI is setup
    }

    private void PlayFX()
    {
        spawnFX.Play();
    }

    private void MoveCoin(bool initialize)
    {
        if (initialize) { myRigidbody.velocity = Vector2.down * fallSpeed; return; }

        Vector3 moveDirection = (moveTo - transform.position);
        myRigidbody.velocity = moveDirection;

        if (moveDirection.magnitude <= .2f)
        {
            //gui.AddCoins(coinValues[coinValue])
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CollectCoin();
    }

    void CollectCoin()
    {
        if (transform.position.y <= 0.5f) coinCollected = true;

        if (coinCollected)
        {
            MoveCoin(false);
        }
    }

    private void OnMouseDown()
    {
        if (!coinCollected)
        {
            coinCollected = true;
        }
    }
}
