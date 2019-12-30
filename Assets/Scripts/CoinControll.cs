using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    int heartHash = Animator.StringToHash("Heart");
    Rigidbody2D myRigidbody;
    Canvas gui; // change to GUIControll

    // state vars for collecting the coin
    int coinType;
    bool coinCollected = false;
    Vector3 moveFallBack = new Vector3(1, 1, 0);
    Vector3 coinCollector;
    Vector3 heartCollector;
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
        coinType = RandInt(3);

        anim = GetComponent<Animator>();
        anim.SetInteger(coinHash, coinType);

        if (RandInt(100) > 90)
        {
            coinType = 4;
            anim.SetTrigger(heartHash);
        }
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

        /*  TODO Update after getting the GUIControll.cs code done
        if (gui)
        {
            coinCollector = gui.GetCoinCollector().position;
            heartCollector = gui.GetHeartCollector().position;
        }
        else
        {
            coinCollector = moveFallBack;
            heartCollector = moveFallBack;
        }
        */

        coinCollector = moveFallBack; // remove after GUI is setup
        heartCollector = moveFallBack; // remove after GUI is setup
    }

    private void PlayFX()
    {
        spawnFX.Play();
    }

    private void MoveCoin(bool initialize)
    {
        if (initialize) { myRigidbody.velocity = Vector2.down * fallSpeed; return; }

        Vector3 collector = coinCollector;
        if (coinType == 4) collector = heartCollector;

        Vector3 moveDirection = (collector - transform.position);
        moveDirection.z = 0;
        myRigidbody.velocity = moveDirection;

        if (moveDirection.magnitude <= 0.2f)
        {
            /*
            if (coinType == 4) { gui.AddHeart(); }
            else { gui.AddCoins(coinValues[coinType - 1]); }
            */Destroy(gameObject);
        }
    }

    void Update()
    {
        CollectCoin();
    }

    void CollectCoin()
    {
        if (transform.position.y <= 0.5f && coinType != 4) coinCollected = true;

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
