using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinControll : MonoBehaviour
{
    // con fig vars //
    [Header("Collect")]
    [SerializeField]
    int[] coinValues = { 10, 20, 30 };
    [SerializeField]
    float fallSpeed = .5f;
    [Header("FX")]
    [SerializeField]
    ParticleSystem spawnFX = null;

    // state vars //
    float spawnDelay = .1f;
    SpriteRenderer myRender;
    Animator anim;
    int coinHash = Animator.StringToHash("Coin Type");
    Rigidbody2D myRigidbody;
    int coinType;
    GUIControll guiCon;
    string[] coinNames = { "", "Bronze Coin", "Silver Coin", "Gold Coin" };

    // state vars for collecting the coin //
    bool coinCollected = false;

    private void Awake()
    {
        // Initialize the coin //
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

    // Determine weather the coin is gold, silver, or bronze //
    private void SetCoinType()
    {
        coinType = RandInt(3);

        anim = GetComponent<Animator>();
        anim.SetInteger(coinHash, coinType);

        gameObject.name = coinNames[coinType];
    }

    // Gets a random int between 1 and max //
    private static int RandInt(int max)
    {
        // Add 1 to max so that it is in the range of possible outputs //
        max += 1;
        return Random.Range(1, max);
    }

    // Activate a VFX for spawning //
    IEnumerator SpawnFX()
    {
        myRender.enabled = true;
        yield return new WaitForSeconds(spawnDelay);
        spawnFX.Play();
        myRigidbody.velocity = Vector2.down * fallSpeed;
    }

    private void LinkGUI()
    {
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();

        // Sanity Check //
        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
    }

    void Update()
    {
        CollectCoin();
    }

    // Collect the coin and add its value to the player's current coin count //
    void CollectCoin()
    {
        if (transform.position.y <= 0.5f) coinCollected = true;

        if (coinCollected)
        {
            if (guiCon)
            {
                guiCon.AddCoins(coinValues[coinType - 1]);
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
