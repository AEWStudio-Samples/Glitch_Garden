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
    public Button ninjaButton = null;
    public Button wallButton = null;
    public Button pitButton = null;
    public Button mineButton = null;
    public Button upgButton = null;
    public Button delButton = null;

    // state vars //

    // Disables the buttons for buying, upgrading or deleting units //
    public void DisableAllButtons()
    {
        ToggleNinja(false);
        ToggleWall(false);
        TogglePit(false);
        ToggleMine(false);
        ToggleUpgBtn(false);
        ToggleDelBtn(false);
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

    public void ToggleUpgBtn(bool tgl)
    {
        upgButton.interactable = tgl;
    }

    public void ToggleDelBtn(bool tgl)
    {
        delButton.interactable = tgl;
    }
    // End button toggle functions //

    // Start a tutorial round //
    internal void StartRound()
    {

    }
}
