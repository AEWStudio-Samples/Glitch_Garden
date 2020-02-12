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
    GUIControll guiControll;
    string[] coinNames = { "", "Bronze Coin", "Silver Coin", "Gold Coin", "Heart" };

    // state vars for collecting the coin
    int coinType;
    bool coinCollected = false;
    Vector3 moveFallBack = new Vector3(1, 1, 0);
    Vector3 coinCollector;
    bool isHeart = false;
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

        gameObject.name = coinNames[coinType];

        if (RandInt(100) > 90)
        {
            isHeart = true;
            anim.SetTrigger(heartHash);
            gameObject.name = coinNames[4];
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
        MoveCoin();
    }

    private void LinkGUI()
    {
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();

        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiControll = guiTest;
        }

        if (guiControll)
        {
            coinCollector = guiControll.GetCoinCollector().position;
            heartCollector = guiControll.GetHeartCollector().position;
        }
        else
        {
            coinCollector = moveFallBack;
            heartCollector = moveFallBack;
        }
        
    }

    private void PlayFX()
    {
        spawnFX.Play();
    }

    private void MoveCoin()
    {
        myRigidbody.velocity = Vector2.down * fallSpeed;
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
            if (guiControll)
            {
                if (isHeart) { guiControll.AddHeart(coinValues[coinType - 1]); }
                else { guiControll.AddCoins(coinValues[coinType - 1]); }
            }
            Destroy(gameObject);    
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
