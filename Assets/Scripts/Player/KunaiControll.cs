using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the script that controls the kunai.
/// </summary>
public class KunaiControll : MonoBehaviour
{
    // TODO add code for damage boost and crit upgrades //
    // con fig vars //
    [SerializeField, Space(10)]
    public int damage = 1;
    public float spnOffset = .05f;

    [SerializeField, Space(10)]
    Vector2 velocityVector = new Vector2();

    // state vars //
    Rigidbody2D myBody;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
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
