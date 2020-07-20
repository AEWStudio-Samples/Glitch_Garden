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

    private void PlayButtonSnd()
    {
        guiCon.soundManager.PlayButtonSnd();
    }

    // Begin Code for sending commands to GUIControll.cs //
    public void StartGame(bool chk)
    {
        PlayButtonSnd();
        guiCon.StartGame(chk);
    }

    public void StartRound()
    {
        PlayButtonSnd();
        guiCon.ClearRound(false);
    }

    public void StartWithTut(bool chk)
    {
        PlayButtonSnd();
        guiCon.StartWithTut(chk);
    }
    
    public void PauseGame(bool chk)
    {
        PlayButtonSnd();
        guiCon.PauseGame(chk);
    }

    public void QuitGame(bool chk)
    {
        PlayButtonSnd();
        guiCon.QuitGame(chk);
    }

    public void ExitGame()
    {
        PlayButtonSnd();
        guiCon.StartCoroutine(guiCon.ExitGame());
    }
    // End Code for sending commands to GUIControll.cs //
}
