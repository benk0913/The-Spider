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
}
