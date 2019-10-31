using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionSelectionWindowUI : MonoBehaviour
{
    [SerializeField]
    Transform FactionContainer;

    [SerializeField]
    GameObject LockedFactionPrefab;

    private void OnEnable()
    {
        ClearContainer();

        GameObject factionObj;
        foreach (Faction faction in CORE.Instance.Database.Factions)
        {
            if(!faction.isPlayable)
            {
                continue;
            }
            
            if (faction.isLocked)
            {
                factionObj = ResourcesLoader.Instance.GetRecycledObject(LockedFactionPrefab);
                factionObj.transform.SetParent(FactionContainer, false);
                continue;
            }

            factionObj = ResourcesLoader.Instance.GetRecycledObject(faction.FactionSelectionPrefab);
            factionObj.transform.SetParent(FactionContainer, false);
            factionObj.GetComponent<Button>().onClick.AddListener(() => { CORE.Instance.NewGame(faction); });
        }
    }

    void ClearContainer()
    {
        while(FactionContainer.childCount > 0)
        {
            FactionContainer.GetChild(0).gameObject.SetActive(false);
            FactionContainer.GetChild(0).SetParent(transform);
        }
    }
}
