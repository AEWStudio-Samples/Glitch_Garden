using UnityEngine;

public class MenuLink : MonoBehaviour
{
    // state vars //
    private GUIControll guiCon;

    private void Awake()
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
        guiCon.StartMenu(chk);
    }

    public void StartWithTut(bool chk)
    {
        PlayButtonSnd();
        guiCon.StartWithTut(chk);
    }

    public void StartRound()
    {
        PlayButtonSnd();
        guiCon.ClearRound(false);
    }

    public void PauseGame(bool chk)
    {
        PlayButtonSnd();
        guiCon.PauseGame(chk);
    }

    public void Options(bool chk)
    {
        PlayButtonSnd();
        guiCon.OptionsMenu(chk);
    }

    public void QuitGame(bool chk)
    {
        PlayButtonSnd();
        guiCon.QuitGame(chk);
    }

    public void SettingChanged(bool chk)
    {
        PlayButtonSnd();
        if (chk)
        {
            guiCon.opManager.RevertSetings();
            guiCon.SettingChanged(!chk);
            guiCon.OptionsMenu(!chk);
        }
        else if (!chk)
        {
            guiCon.SettingChanged(chk);
        }
    }

    public void ExitGame()
    {
        PlayButtonSnd();
        guiCon.StartCoroutine(guiCon.ExitGame());
    }

    // End Code for sending commands to GUIControll.cs //
}