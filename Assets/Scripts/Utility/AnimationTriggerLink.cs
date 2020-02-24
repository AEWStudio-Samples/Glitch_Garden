using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script lets the animator send commands
/// to the controller script attached to the parent.
/// </summary>
public class AnimationTriggerLink : MonoBehaviour
{
    // con fig vars //
    [SerializeField, Space(10)]
    GameObject root = null;

    [SerializeField, Space(10)]
    AnimType animationType = AnimType.Mine;

    // state vars //
    enum AnimType { Mine, Ninja, Phantom, Pit, Wall, Zombie, Title}

    MineControll mineControll;
    NinjaControll ninjaControll;
    PhantomControll phantomControll;
    PitControll pitControll;
    WallControll wallControll;
    ZombieControll zombieControll;

    void Start()
    {
        SetController();
    }

    // Set the correct controller for the animator //
    void SetController()
    {
        switch(animationType)
        {
            case AnimType.Mine:
                mineControll = root.GetComponent<MineControll>();
                break;
            case AnimType.Ninja:
                ninjaControll = root.GetComponent<NinjaControll>();
                break;
            case AnimType.Phantom:
                phantomControll = root.GetComponent<PhantomControll>();
                break;
            case AnimType.Pit:
                pitControll = root.GetComponent<PitControll>();
                break;
            case AnimType.Wall:
                wallControll = root.GetComponent<WallControll>();
                break;
            case AnimType.Zombie:
                zombieControll = root.GetComponent<ZombieControll>();
                break;
        }
    }

    // Move to the main menu after the title animation is done //
    private void FinishTitle()
    {
        if (animationType != AnimType.Title) { return; }
        GUIControll guiCon = FindObjectOfType<GUIControll>();
        guiCon.GoToScene("MainMenu");
    }

    // Tell the controller that spawning has finished //
    private void FinishSpawn()
    {
        switch (animationType)
        {
            case AnimType.Mine:
                //mineControll.FinishSpawn();
                break;
            case AnimType.Ninja:
                ninjaControll.FinishSpawn();
                break;
            case AnimType.Phantom:
                phantomControll.FinishSpawn();
                break;
            case AnimType.Pit:
                //pitControll.FinishSpawn();
                break;
            case AnimType.Wall:
                //wallControll.FinishSpawn();
                break;
            case AnimType.Zombie:
                zombieControll.FinishSpawn();
                break;
        }
    }

    // Tell the controller to check the lane for a valid target //
    private void CheckLane()
    {
        switch (animationType)
        {
            case AnimType.Ninja:
                ninjaControll.CheckLane();
                break;
            case AnimType.Phantom:
                phantomControll.CheckLane();
                break;
            case AnimType.Zombie:
                zombieControll.CheckLane();
                break;
        }
    }

    // Tell the controller to attack //
    private void Attack()
    {
        switch (animationType)
        {
            case AnimType.Mine:
                //mineControll.Attack();
                break;
            case AnimType.Ninja:
                ninjaControll.Attack();
                break;
            case AnimType.Phantom:
                phantomControll.Attack();
                break;
            case AnimType.Pit:
                //pitControll.Attack();
                break;
            case AnimType.Zombie:
                zombieControll.Attack();
                break;
        }
    }

    // Tell the controller to throw a kunai //
    private void ThrowKunai()
    {
        ninjaControll.ThrowKunai();
    }

    // Tell the controller to walk //
    private void Walk()
    {
        if (animationType == AnimType.Title) { return; }
        switch (animationType)
        {
            case AnimType.Phantom:
                phantomControll.Walk();
                break;
            case AnimType.Zombie:
                zombieControll.Walk();
                break;
        }
    }

    // Tell the controller that it has just died //
    private void HandleDeath()
    {
        switch (animationType)
        {
            case AnimType.Mine:
                //mineControll.HandleDeath();
                break;
            case AnimType.Ninja:
                ninjaControll.HandleDeath();
                break;
            case AnimType.Phantom:
                phantomControll.HandleDeath(true);
                break;
            case AnimType.Pit:
                //pitControll.HandleDeath();
                break;
            case AnimType.Wall:
                //wallControll.HandleDeath();
                break;
            case AnimType.Zombie:
                zombieControll.HandleDeath();
                break;
        }
    }


}
