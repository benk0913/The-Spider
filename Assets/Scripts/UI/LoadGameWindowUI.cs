using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LoadGameWindowUI : MonoBehaviour
{
    [SerializeField]
    Transform SaveLinesContainer;

    [SerializeField]
    public GameObject NoSaveFilesPanel;

    public static LoadGameWindowUI Instance;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        RefreshUI();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void RefreshUI()
    {
        ClearSaveLines();

        CORE.Instance.ReadAllSaveFiles();

        NoSaveFilesPanel.gameObject.SetActive(CORE.Instance.SaveFiles.Count == 0);

        for (int i=0;i< CORE.Instance.SaveFiles.Count;i++)
        {
            GameObject tempLine = ResourcesLoader.Instance.GetRecycledObject("SaveFileLineUI");
            tempLine.transform.SetParent(SaveLinesContainer, false);
            tempLine.GetComponent<SaveFileLineUI>().SetInfo(CORE.Instance.SaveFiles[i], this);
        }
    }

    void ClearSaveLines()
    {
        while(SaveLinesContainer.childCount > 0)
        {
            SaveLinesContainer.GetChild(0).gameObject.SetActive(false);
            SaveLinesContainer.GetChild(0).SetParent(transform);
        }
    }

    public void DeleteSave(SaveFile file)
    {
        CORE.Instance.RemoveSave(file);
        RefreshUI();
    }

    public void LoadSave(SaveFile file)
    {
        CORE.Instance.LoadGame(file);
        CORE.Instance.InvokeEvent("HideEscapeMenu");
        Hide();
    }
}
