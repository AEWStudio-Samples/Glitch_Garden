using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLink : MonoBehaviour
{
    // state vars //
    GUIControll guiCon;
    PriceManager priceManager;
    TutControll tutCon;
    HUDLink hudLink;

    /*[SerializeField]
    MenuType menuType = MenuType.Main;

    enum MenuType { Main, Sub }*/

    void Awake()
    {
        GUIControll[] guiList = FindObjectsOfType<GUIControll>();

        // Sanity Check //
        foreach (GUIControll guiTest in guiList)
        {
            if (guiTest.CompareTag("GUI")) guiCon = guiTest;
        }
    }

    public void StartGame(bool chk)
    {
        guiCon.StartGame(chk);
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
}
