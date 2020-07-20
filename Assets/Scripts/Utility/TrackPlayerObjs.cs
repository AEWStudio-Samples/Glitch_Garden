using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the script that tracks the player units.
/// </summary>
public class TrackPlayerObjs : MonoBehaviour
{
    // con fig vars //
    [SerializeField, Space(10)]
    bool isMovable = false;

    [SerializeField, Space(10)]
    TrackType type = TrackType.Token;

    [SerializeField, Space(10)]
    GameObject objSpawnRef = null;

    // state vars //
    GUIControll guiCon;
    UpgradeManager[] upgradables;
    static List<Vector2> usedLocations = new List<Vector2>();

    public Vector2 gridPos;
    bool isBeingHeld;
    static bool objSpawn;
    static GameObject objToSpawn;
    static bool objUpgrade = false;
    static bool objDelete = false;

    enum TrackType { Button, Spawn, Token, Upgrade, Delete }

    private void PlayButtonSnd()
    {
        guiCon.soundManager.PlayButtonSnd();
    }

    private void Awake()
    {
        foreach (GUIControll guiTest in FindObjectsOfType<GUIControll>())
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
    }

    public void ResetTracker()
    {
        usedLocations = new List<Vector2>();
    }

    private void Start()
    {
        gridPos = transform.position;
        CheckUsedLocations(true);
    }

    private bool CheckUsedLocations(bool init)
    {
        Vector2 testPos = GetGridPos();

        // Restrict grid movement and avoid used locations //
        if (!init)
        {
            if (testPos.x > 7)
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

        // Remove old location from usedLocations //
        if (usedLocations.Contains(gridPos))
        {
            usedLocations.Remove(gridPos);
        }

        // Set new location //
        if (!init) gridPos = testPos;
        usedLocations.Add(gridPos);
        return true;
    }

    // Gets the grid position that is being used //
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

        upgradables = FindObjectsOfType<UpgradeManager>();

        if (guiCon.roundClear)
        {
            objSpawn = false;
            objUpgrade = false;
            objDelete = false;
        }

        if (objUpgrade)
        {
            foreach (var upgrade in upgradables)
            {
                if (upgrade.CheckCoinCount())
                {
                    upgrade.upgradable = true;
                }
                else
                {
                    upgrade.upgradable = false;
                }
            }
        }
        else
        {
            foreach (var upgrade in upgradables)
            {
                upgrade.upgradable = false;
            }
            
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

    // Determines what to do when gameObject is clicked on //
    public void ObjInterface()
    {
        switch(type)
        {
            case TrackType.Button:
                guiCon.soundManager.soundUI.Play();
                ButtonPressed();
                break;
            case TrackType.Spawn:
                SpawnObj();
                break;
            case TrackType.Token:
                if (objDelete)
                {
                    DelUnit();
                    break;
                }
                if (objUpgrade)
                {
                    ObjUpgrade();
                    break;
                }
                if (isMovable && !guiCon.roundClear)
                {
                    isBeingHeld = true;
                }
                break;
            case TrackType.Upgrade:
                guiCon.soundManager.PlayButtonSnd();
                ToggleUpgrade();
                break;
            case TrackType.Delete:
                guiCon.soundManager.PlayButtonSnd();
                ToggleDelete();
                break;
        }
    }

    // Used when pressing the Ninja, Wall, Pit or Mine buttons //
    void ButtonPressed()
    {
        if (objUpgrade)
        {
            objUpgrade = false;
            TogglePriceTag();
        }

        if (objDelete)
        {
            objDelete = false;
            ToggleDeleteMark();
        }

        if (objSpawnRef && !objSpawn)
        {
            objToSpawn = objSpawnRef;
            objSpawn = true;
        }
        else if (objSpawnRef && objSpawnRef != objToSpawn)
        {
            objToSpawn = objSpawnRef;
        }
        else { objSpawn = false; }
    }

    // Tries to spawn the object associated with the button pressed //
    void SpawnObj()
    {
        if (objSpawn)
        {
            if(!CheckUsedLocations(false)) return;
            Instantiate(objToSpawn, gridPos, Quaternion.identity);
            objSpawn = false;
        }
    }

    // Toggles upgrade on and off //
    void ToggleUpgrade()
    {
        if (objSpawn)
        {
            objSpawn = false;
        }

        if (objDelete)
        {
            objDelete = false;
            ToggleDeleteMark();
        }

        objUpgrade = !objUpgrade;

        TogglePriceTag();
    }

    // Toggles the upgrade price tags for upgrades that can be bought //
    private void TogglePriceTag()
    {
        if (objUpgrade)
        {
            // Turn price tag on //
            foreach (var upgrade in upgradables)
            {
                upgrade.TogglePlate(true);
            }
        }
        else
        {
            // Turn price tag off //
            foreach (var upgrade in upgradables)
            {
                upgrade.TogglePlate(false);
            }
        }
    }

    // Upgrades the object that is clicked on //
    private void ObjUpgrade()
    {
        // Get current coin count //
        int curCoinCount = guiCon.conTrack.curCoinCount;

        int coinAdj = 0;
        switch(gameObject.tag)
        {
            // Upgrade Ninja //
            case "Ninja":
                if (GetComponent<UpgradeManager>().upgradable)
                {
                    coinAdj = GetComponent<UpgradeManager>().upgradeCost;
                    var controll = GetComponent<NinjaControll>();
                    var rank = controll.GetRank();
                    controll.SetStats(rank + 1);
                    objUpgrade = false;
                    TogglePriceTag();
                }
                break;

            // Upgrade Wall //
            case "Wall":
                if (GetComponent<UpgradeManager>().upgradable)
                {
                    coinAdj = GetComponent<UpgradeManager>().upgradeCost;
                    var controll = GetComponent<WallControll>();
                    var rank = controll.GetRank();
                    controll.SetStats(rank + 1);
                    objUpgrade = false;
                    TogglePriceTag();
                }
                break;

            // Upgrade Pit //
            case "Pit":
                if (GetComponent<UpgradeManager>().upgradable)
                {
                    coinAdj = GetComponent<UpgradeManager>().upgradeCost;
                    var controll = GetComponent<PitControll>();
                    var rank = controll.GetRank();
                    controll.SetStats(rank + 1);
                    objUpgrade = false;
                    TogglePriceTag();
                }
                break;

            // Upgrade Mine //
            case "Mine":
                if (GetComponent<UpgradeManager>().upgradable)
                {
                    coinAdj = GetComponent<UpgradeManager>().upgradeCost;
                    var controll = GetComponent<MineControll>();
                    var rank = controll.GetRank();
                    controll.SetStats(rank + 1);
                    objUpgrade = false;
                    TogglePriceTag();
                }
                break;
        }

        // Update coin count //
        curCoinCount -= coinAdj;
        guiCon.UpdateCoinCount(curCoinCount);
    }

    private void ToggleDelete()
    {
        if (objSpawn)
        {
            objSpawn = false;
        }

        if (objUpgrade)
        {
            objUpgrade = false;
            TogglePriceTag();
        }

        objDelete = !objDelete;

        ToggleDeleteMark();
    }

    private void ToggleDeleteMark()
    {
        if (objDelete)
        {
            foreach (var delTarg in upgradables)
            {
                delTarg.deletable = true;
                delTarg.ToggleDelMark(true);
            }
        }
        else
        {
            foreach (var delTarg in upgradables)
            {
                delTarg.deletable = false;
                delTarg.ToggleDelMark(false);
            }
        }
    }

    private void DelUnit()
    {
        switch (gameObject.tag)
        {
            // Delete Ninja //
            case "Ninja":
                if (GetComponent<UpgradeManager>().deletable)
                {
                    var controll = GetComponent<NinjaControll>();
                    controll.HandleDeath();
                    objDelete = false;
                    ToggleDelete();
                }
                break;

            // Delete Wall //
            case "Wall":
                if (GetComponent<UpgradeManager>().deletable)
                {
                    var controll = GetComponent<WallControll>();
                    controll.DestroyWall();
                    objDelete = false;
                    ToggleDelete();
                }
                break;

            // Delete Pit //
            case "Pit":
                if (GetComponent<UpgradeManager>().deletable)
                {
                    var controll = GetComponent<PitControll>();
                    controll.DestroyPit();
                    objDelete = false;
                    ToggleDelete();
                }
                break;

            // Delete Mine //
            case "Mine":
                if (GetComponent<UpgradeManager>().deletable)
                {
                    var controll = GetComponent<MineControll>();
                    controll.DestroyMine();
                    objDelete = false;
                    ToggleDelete();
                }
                break;
        }
    }
    
    public void HandleDeath()
    {
        // Remove from used location list //
        usedLocations.Remove(gridPos);
    }
}
