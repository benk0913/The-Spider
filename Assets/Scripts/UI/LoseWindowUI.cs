using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseWindowUI : MonoBehaviour
{
    [SerializeField]
    Transform AvatarContainer;

    public static LoseWindowUI Instance;

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
        this.gameObject.SetActive(false);
        WarningWindowUI.Instance.Show("Go, nobody will miss you...", ()=> { BedUtilityScreen.Instance.QuitGame(); });
    }
}
