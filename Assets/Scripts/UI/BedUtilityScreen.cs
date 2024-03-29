﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedUtilityScreen : MonoBehaviour
{
    public static BedUtilityScreen Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("ShowEscapeMenu", Show);
        CORE.Instance.SubscribeToEvent("HideEscapeMenu", Hide);

        Hide();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Resume()
    {
        CORE.Instance.InvokeEvent("QuitBed");
    }

    public void QuitGame()
    {
        WarningWindowUI.Instance.Show("Go, nobody will miss you...", QuitFinal);
    }

    public void QuitFinal()
    {
        CORE.Instance.DisposeCurrentGame();
        SceneManager.LoadScene(0);
        Hide();
    }

    public void QuitTotalGame()
    {
        WarningWindowUI.Instance.Show("I will miss you...", ()=> { Application.Quit(); });
    }

    public void SaveGame()
    {
        TextInputWindowUI.Instance.Show((saveName) =>
        {
            WarningWindowUI.Instance.Show("Save Game?", () =>
            {
                CORE.Instance.SaveGame(saveName); Resume();
            });
        },"Save Name:", "Save" + CORE.Instance.SaveFiles.Count);
    }

}
