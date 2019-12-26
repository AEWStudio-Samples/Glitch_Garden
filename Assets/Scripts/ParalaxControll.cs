using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxControll : MonoBehaviour
{
    // con fig vars
    [SerializeField]
    float scrollSpeed = 0.5f;

    // state vars
    Material myMat;
    Vector2 offSet;

    void Start()
    {
        myMat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        offSet = Camera.main.transform.position;
        myMat.mainTextureOffset = offSet * scrollSpeed;
    }
}
