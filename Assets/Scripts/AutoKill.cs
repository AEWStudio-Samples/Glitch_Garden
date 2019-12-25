using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKill : MonoBehaviour
{
    // con fig vars
    [SerializeField]
    float killTimer = 2f;

    // state vars

    void Start()
    {
        Destroy(gameObject, killTimer);
    }
}
