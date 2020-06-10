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

    [SerializeField]
    FactionBriefPanelUI FactionPanel;

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
                factionObj.GetComponent<TooltipTargetUI>().SetTooltip("<color=yellow>" + faction.name + "</color> " + System.Environment.NewLine + "- To Unlock: " + faction.UnlockDescription);
                continue;
            }

            factionObj = ResourcesLoader.Instance.GetRecycledObject(faction.FactionSelectionPrefab);
            factionObj.transform.SetParent(FactionContainer, false);
            factionObj.GetComponent<Button>().onClick.AddListener(() => { FactionPanel.Show(faction); });
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
