using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to control the tutorial
/// and to manage access to the buttons for
/// buying new units.
/// </summary>
public class TutControll : MonoBehaviour
{
    // con fig vars //
    [Header("Tool Box Buttons")]
    [SerializeField]
    Button ninjaButton = null;

    [SerializeField]
    Button wallButton = null;

    [SerializeField]
    Button pitButton = null;

    [SerializeField]
    Button mineButton = null;

    // state vars //

    void Start()
    {
        
    }

    void Update()
    {

    }

    // Disables the buttons for buying new units //
    public void DisableAllButtons()
    {
        ToggleNinja(false);
        ToggleWall(false);
        TogglePit(false);
        ToggleMine(false);
    }

    // Begin button toggle functions //
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
    // End button toggle functions //

    // Start a tutorial round //
    internal void StartRound()
    {

    }
}
