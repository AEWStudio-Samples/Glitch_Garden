﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggerLink : MonoBehaviour
{
    // con fig vars
    [SerializeField, Space(10)]
    GameObject root = null;
    [SerializeField, Space(10)]
    AnimType animationType = AnimType.Mine;

    // state vars
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

    private void FinishTitle()
    {
        if (animationType != AnimType.Title) { return; }
        GUIControll guiCon = FindObjectOfType<GUIControll>();
        guiCon.GoToScene("MainMenu");
    }

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
                //phantomControll.FinishSpawn();
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

    private void ThrowKunai()
    {
        ninjaControll.ThrowKunai();
    }

    private void Walk()
    {
        if (animationType == AnimType.Title) { return; }
        zombieControll.Walk();
    }


}
