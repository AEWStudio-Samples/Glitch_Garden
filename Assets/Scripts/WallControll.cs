using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallControll : MonoBehaviour
{
    // con fig vars
    [SerializeField, Space(10)]
    int hitPoints = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleHit(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {

        }
    }
}
