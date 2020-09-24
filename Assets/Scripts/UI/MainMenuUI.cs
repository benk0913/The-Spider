using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void QuitApplication()
    {
        Application.Quit();
    }

    public void OpenLoadGame()
    {
        LoadGameWindowUI.Instance.Show();
    }

    public void LinkToCommunity()
    {
        Application.OpenURL("https://steamcommunity.com/app/1380010/discussions/");
    }
}
