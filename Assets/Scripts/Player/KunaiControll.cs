using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the script that controls the kunai.
/// </summary>
public class KunaiControll : MonoBehaviour
{
    // TODO add code for upgrades //
    // con fig vars //
    [Space(10)]
    public int damage = 10;
    public int pirceCnt = 0;
    public float spnOffset = .05f;
    public float despawnDelay = .05f;

    [SerializeField, Space(10)]
    Vector2 velocityVector = new Vector2();

    // state vars //
    Rigidbody2D myBody;
    GUIControll guiCon;

    private void OnTriggerEnter2D(Collider2D target)
    {
        // Find out what the kuni hit //
        switch (target.tag)
        {
            // Hit a zombie //
            case "Zombie":
                guiCon.dmgHand.DealDamage(GetFinalDmg(target.transform.position), target);

                if (pirceCnt == 0) { Destroy(gameObject, despawnDelay); }
                else { pirceCnt--; }

                break;
            // Hit a phantom //
            case "Phantom":
                guiCon.dmgHand.DealDamage(GetFinalDmg(target.transform.position), target);
                break;
        }
    }

    private int GetFinalDmg(Vector3 dtsp)
    {
        int finDmg;

        if (guiCon.conTrack.kuniCrit && guiCon.dmgHand.CheckCrit(guiCon.conTrack.kuniCRCur))
        {
            finDmg = damage * guiCon.conTrack.kuniCMPCur;

            guiCon.dmgHand.SpawnCDMGText(finDmg, dtsp);
        }
        else
        {
            finDmg = damage;

            guiCon.dmgHand.SpawnDMGText(finDmg, dtsp);
        }

        return finDmg;
    }

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();

        // Sanity Check //
        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
    }

    private void Start()
    {
        damage *= guiCon.conTrack.kuniDMGBoost;
        pirceCnt = guiCon.conTrack.kuniPirceCnt;
        SetVelocity();
    }

    // Make the kunai move to the zombies //
    private void SetVelocity()
    {
        myBody.velocity = velocityVector;
    }

    private void Update()
    {
        if (transform.position.x > 9.5f) { Destroy(gameObject); }
    }
}
