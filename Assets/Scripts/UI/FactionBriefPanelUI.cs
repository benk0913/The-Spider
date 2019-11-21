﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FactionBriefPanelUI : MonoBehaviour
{
    Faction CurrentFaction;

    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TextMeshProUGUI Description;

    [SerializeField]
    Transform AvatarContainer;

    [SerializeField]
    Transform GoalsContainer;

    [SerializeField]
    Transform RulesContainer;

    public void Show(Faction faction)
    {
        this.gameObject.SetActive(true);

        CurrentFaction = faction;
        Title.text = faction.name;
        Description.text = faction.Description;

        ClearContainer(AvatarContainer);
        ClearContainer(GoalsContainer);
        ClearContainer(RulesContainer);

        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(faction.FactionSelectionPrefab);
        tempObj.transform.SetParent(AvatarContainer, false);
        tempObj.transform.localScale = Vector3.one;
        tempObj.transform.position = AvatarContainer.position;

        foreach(Quest quest in faction.Goals)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("QuestContentUI");
            tempObj.transform.SetParent(GoalsContainer, false);
            tempObj.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            tempObj.GetComponent<QuestContentUI>().SetInfo(quest);
        }

    }

    void ClearContainer(Transform container)
    {
        while(container.childCount > 0)
        {
            container.transform.GetChild(0).gameObject.SetActive(false);
            container.transform.GetChild(0).SetParent(transform);
        }
    }

    public void PlayOnCurrentFaction()
    {
        CORE.Instance.NewGame(CurrentFaction);
    }
}
