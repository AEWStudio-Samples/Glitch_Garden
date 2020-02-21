using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayerObjs : MonoBehaviour
{
    // con fig vars
    [SerializeField, Space(10)]
    TrackType type = TrackType.Token;

    [SerializeField, Space(10)]
    GameObject objSpawnRef = null;

    // state vars
    static List<Vector2> usedLocations = new List<Vector2>();

    public Vector2 gridPos;
    bool isBeingHeld;
    static bool objSpawn;
    static GameObject objToSpawn;
    static bool objUpgrade;

    enum TrackType { Button, Spawn, Token, Upgrade, Delete }

    private void Start()
    {
        gridPos = transform.position;
        CheckUsedLocations(true);
    }

    private bool CheckUsedLocations(bool init)
    {
        Vector2 testPos = GetGridPos();

        // restrict grid movement and avoid used locations
        if (!init)
        {
            if (testPos.x > 6)
                return false;
            else if (testPos.x < 1)
                return false;
            else if (testPos.y > 5)
                return false;
            else if (testPos.y < 1)
                return false;
            else if (usedLocations.Contains(testPos))
                return false;
        }

        // remove old location from usedLocations
        if (usedLocations.Contains(gridPos))
        {
            usedLocations.Remove(gridPos);
        }

        // set new location
        if (!init) gridPos = testPos;
        usedLocations.Add(gridPos);
        return true;
    }

    private Vector2 GetGridPos()
    {
        float newX, newY;

        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        newX = Mathf.RoundToInt(mousePos.x);
        newY = Mathf.RoundToInt(mousePos.y);

        return new Vector2(newX, newY);
    }

    void Update()
    {
        if (isBeingHeld)
        {
            if(!CheckUsedLocations(false)) return;
            this.gameObject.transform.position = gridPos;
        }
    }

    private void OnMouseDown()
    {
        ObjInterface();
    }

    private void OnMouseUp()
    {
        isBeingHeld = false;
    }

    public void ObjInterface()
    {
        switch(type)
        {
            case TrackType.Button:
                ButtonPressed();
                break;
            case TrackType.Spawn:
                SpawnObj();
                break;
            case TrackType.Token:
                if (objUpgrade)
                {
                    ObjUpgrade();
                }
                isBeingHeld = true;
                break;
            case TrackType.Upgrade:
                UpgradeDef();
                break;
        }
    }

    void ButtonPressed()
    {
        if (objSpawnRef && !objSpawn)
        {
            objToSpawn = objSpawnRef;
            objSpawn = true;
        }
        else { objSpawn = false; }
    }

    void SpawnObj()
    {
        if (objSpawn)
        {
            if(!CheckUsedLocations(false)) return;
            Instantiate(objToSpawn, gridPos, Quaternion.identity);
            objSpawn = false;
        }
    }

    void UpgradeDef()
    {
        Debug.Log("Checking objUpgrade.");
        if (!objUpgrade)
        {
            Debug.Log("Select defender to upgrade.");
            objUpgrade = true;
        }
        else
        {
            Debug.Log("Defender upgrade canceled.");
            objUpgrade = false;
        }
    }

    private void ObjUpgrade()
    {
        switch(gameObject.tag)
        {
            case "Ninja":
                Debug.Log("Upgrading Ninja.");
                var nCon = GetComponent<NinjaControll>();
                var rank = nCon.GetRank();
                nCon.SetStats(rank + 1);
                objUpgrade = false;
                break;
            case "Wall":
                break;
            case "Pit":
                break;
            case "Mine":
                break;
        }
    }
    
    public void HandleDeath()
    {
        usedLocations.Remove(gridPos);
    }
}
