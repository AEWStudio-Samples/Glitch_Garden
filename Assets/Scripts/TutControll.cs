using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutControll : MonoBehaviour
{
    // con fig vars
    [Header("Tool Box Buttons")]
    [SerializeField]
    Button ninjaButton = null;
    [SerializeField]
    Button wallButton = null;
    [SerializeField]
    Button pitButton = null;
    [SerializeField]
    Button mineButton = null;
    [SerializeField]
    Button phantomButton = null;

    // state vars

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void DisableAllButtons()
    {
        ToggleNinja(false);
        ToggleWall(false);
        TogglePit(false);
        ToggleMine(false);
        TogglePhantom(false);
    }

    public void ToggleNinja(bool tgl)
    {
        ninjaButton.interactable = tgl;
    }

    public void ToggleWall(bool tgl)
    {
        wallButton.interactable = tgl;
    }

    public void TogglePit(bool tgl)
    {
        pitButton.interactable = tgl;
    }

    public void ToggleMine(bool tgl)
    {
        mineButton.interactable = tgl;
    }

    public void TogglePhantom(bool tgl)
    {
        phantomButton.interactable = tgl;
    }

    internal void StartRound()
    {

    }
}
