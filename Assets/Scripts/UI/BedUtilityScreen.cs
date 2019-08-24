using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        WarningWindowUI.Instance.Show("Go, nobody will miss you...", Application.Quit);
    }

    public void SaveGame()
    {
        CORE.Instance.SaveGame();
    }
}
