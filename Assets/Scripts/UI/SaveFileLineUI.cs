using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveFileLineUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TextMeshProUGUI DateText;

    SaveFile CurrentSave;

    LoadGameWindowUI Window;

    public void SetInfo(SaveFile save, LoadGameWindowUI window)
    {
        Window = window;
        CurrentSave = save;
        RefreshUI();
    }

    public void RefreshUI()
    {
        Title.text = CurrentSave.Name;
        DateText.text = CurrentSave.Date;
    }

    public void Delete()
    {
        WarningWindowUI.Instance.Show("DELETE " + CurrentSave.Name + "? ARE YOU SURE?", ConfirmDelete);
    }

    void ConfirmDelete()
    {
        Window.DeleteSave(CurrentSave);
    }

    public void Load()
    {
        WarningWindowUI.Instance.Show("Load this save file?", ConfirmLoad);
    }

    void ConfirmLoad()
    {
        Window.LoadSave(CurrentSave);
    }

}
