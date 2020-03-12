using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryWindowUI : MonoBehaviour
{
    [SerializeField]
    Transform AvatarContainer;

    public static VictoryWindowUI Instance;

    private void Awake()
    {
        Instance = this;

        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);

        ClearContainer(AvatarContainer);

        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(CORE.PC.CurrentFaction.FactionSelectionPrefab);
        tempObj.transform.SetParent(AvatarContainer, false);
        tempObj.transform.localScale = Vector3.one;
        tempObj.transform.position = AvatarContainer.position;
    }

    void ClearContainer(Transform container)
    {
        while (container.childCount > 0)
        {
            container.transform.GetChild(0).gameObject.SetActive(false);
            container.transform.GetChild(0).SetParent(transform);
        }
    }

    public void Confirm()
    {
        MapViewManager.Instance.MapFocusView.Deactivate();
        this.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        WarningWindowUI.Instance.Show("Go, nobody will miss you...", Application.Quit);
    }
}
