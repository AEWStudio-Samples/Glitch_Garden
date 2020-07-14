using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordManager : MonoBehaviour
{
    // state vars //
    string hordType;
    int hordCnt;

    void Start()
    {
        hordType = gameObject.name.Split('_')[1].Split('(')[0];
    }

    void Update()
    {
        hordCnt = 0;

        foreach (Transform mob in GetComponentsInChildren<Transform>())
        {
            if (mob.name == hordType)
            {
                hordCnt++;
            }
        }

        if (hordCnt == 0) Destroy(gameObject);
    }
}
