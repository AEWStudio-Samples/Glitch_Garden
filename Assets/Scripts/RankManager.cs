using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankManager : MonoBehaviour
{
    // con fig vars
    [SerializeField, Space(10)]
    Sprite[] rankInsignias = { };

    // state vars
    SpriteRenderer myRenderer;

    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetInsignia(int rank)
    {
        if (rank > 3) { rank = 3; }

        myRenderer.sprite = rankInsignias[rank];
    }
}
