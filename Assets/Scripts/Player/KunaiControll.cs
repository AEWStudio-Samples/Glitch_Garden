using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiControll : MonoBehaviour
{
    // con fig vars
    [SerializeField, Space(10)]
    public int damage = 1;

    [SerializeField, Space(10)]
    Vector2 velocityVector = new Vector2();

    // state vars
    Rigidbody2D myBody;

    // state vars for the animator
    Animator anim;
    int hitHash = Animator.StringToHash("Hit");

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        SetVelocity();
    }

    private void SetVelocity()
    {
        myBody.velocity = velocityVector;
    }

    private void Update()
    {
        if (transform.position.x > 9.5f) { Destroy(gameObject); }
    }
}
