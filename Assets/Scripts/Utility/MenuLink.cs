using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLink : MonoBehaviour
{
    // state vars //
    GUIControll guiCon;

    void Awake()
    {
        // Sanity Check //
        foreach (GUIControll guiTest in FindObjectsOfType<GUIControll>())
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
    }

    // Begin Code for sending commands to GUIControll.cs //
    public void StartGame(bool chk)
    {
        guiCon.StartGame(chk);
    }

    public void StartRound()
    {
        guiCon.ClearRound(false);
    }

    public void StartWithTut(bool chk)
    {
        guiCon.StartWithTut(chk);
    }
    
    public void PauseGame(bool chk)
    {
        guiCon.PauseGame(chk);
    }

    public void QuitGame(bool chk)
    {
        guiCon.QuitGame(chk);
    }

    public void ExitGame()
    {
        guiCon.ExitGame();
    }
    // End Code for sending commands to GUIControll.cs //
}
