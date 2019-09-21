using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class QuestsPanelUI : MonoBehaviour, ISaveFileCompatible
{
    public static QuestsPanelUI Instance;
    
    public List<Quest> ActiveQuests = new List<Quest>();
    public List<Quest> CompletedQuests = new List<Quest>();

    [SerializeField]
    Transform questsContainer;

    [SerializeField]
    Button activeQuestsButton;

    [SerializeField]
    Button completeQuestsButton;

    private void Awake()
    {
        Instance = this;
    }


    void OnEnable()
    {
        ShowActive();
    }
    
    public void AddNewQuest(Quest quest)
    {
        Quest newQuest = quest.CreateClone();
        ActiveQuests.Add(newQuest);
        AddQuestToContainer(newQuest);
    }

    public void Complete(Quest quest)
    {
        ActiveQuests.Remove(quest);
        CompletedQuests.Add(quest);
    }

    public void ShowActive()
    {
        ClearContainer();

        activeQuestsButton.interactable = false;
        completeQuestsButton.interactable = true;

        foreach (Quest quest in ActiveQuests)
        {
            AddQuestToContainer(quest);
        }
    }

    public void ShowArchived()
    {
        ClearContainer();

        activeQuestsButton.interactable = true;
        completeQuestsButton.interactable = false;

        foreach (Quest quest in CompletedQuests)
        {
            AddQuestToContainer(quest);
        }
    }

    void AddQuestToContainer(Quest quest)
    {
        GameObject rumorPanel = ResourcesLoader.Instance.GetRecycledObject("QuestHeadlineUI");
        rumorPanel.transform.SetParent(questsContainer, false);
        rumorPanel.GetComponent<QuestHeadlineUI>().SetInfo(quest, this);
    }

    void ClearContainer()
    {
        while (questsContainer.childCount > 0)
        {
            questsContainer.GetChild(0).gameObject.SetActive(false);
            questsContainer.GetChild(0).SetParent(transform);
        }
    }


    public bool HasQuest(Quest questAttachment)
    {
        foreach(Quest quest in ActiveQuests)
        {
            if(quest.name == questAttachment.name)
            {
                return true;
            }
        }

        foreach (Quest quest in CompletedQuests)
        {
            if (quest.name == questAttachment.name)
            {
                return true;
            }
        }

        return false;
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        for(int i=0;i<ActiveQuests.Count;i++)
        {
            node["ActiveQuests"][i] = ActiveQuests[i].ToJSON();
        }

        for (int i = 0; i < CompletedQuests.Count; i++)
        {
            node["CompletedQuests"][i] = CompletedQuests[i].ToJSON();
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        ActiveQuests.Clear();
        CompletedQuests.Clear();

        for (int i = 0; i < node["ActiveQuests"].Count; i++)
        {
            Quest quest = CORE.Instance.Database.GetQuest(node["ActiveQuests"][i]["Key"]).CreateClone();
            quest.FromJSON(node["ActiveQuests"][i]);

            ActiveQuests.Add(quest);
        }

        for (int i = 0; i < node["CompletedQuests"].Count; i++)
        {
            Quest quest = CORE.Instance.Database.GetQuest(node["CompletedQuests"][i]["Key"]).CreateClone();
            quest.FromJSON(node["CompletedQuests"][i]);

            CompletedQuests.Add(quest);
        }

    }

    public void ImplementIDs()
    {
    }
}
