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

    [SerializeField]
    NotificationUI Notification;

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

        if (newQuest.RelevantCharacter != null)
        {
            newQuest.RelevantCharacter.Pinned = true;

            foreach (string infoKey in newQuest.InfoGivenOnCharacter)
            {
                newQuest.RelevantCharacter.Known.Know(infoKey);
            }
        }

        if (newQuest.Tutorial)
        {
            WorldMissionPanelUI.Instance.ShowQuest(newQuest);
        }

        foreach (QuestObjective objective in newQuest.Objectives)
        {
            if(objective.ValidateRoutine != null)
            {
                CORE.Instance.StopCoroutine(objective.ValidateRoutine);
            }

            objective.ValidateRoutine = CORE.Instance.StartCoroutine(ValidateObjectiveRoutine(objective));

            if (objective.WorldMarker != null)
            {
                objective.WorldMarker.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(objective.WorldMarkerTarget))
            {
                objective.WorldMarker = ResourcesLoader.Instance.GetRecycledObject("MarkerWorld");
                objective.WorldMarker.transform.SetParent(CORE.Instance.MainCanvas.transform);
                objective.WorldMarker.transform.SetAsLastSibling();
                objective.WorldMarker.GetComponent<WorldPositionLerperUI>().SetTransform(GameObject.Find(objective.WorldMarkerTarget).transform);
            }
        }

        Notification.Add(1);
    }

    IEnumerator ValidateObjectiveRoutine(QuestObjective objective)
    {
        yield return 0;

        while(!objective.Validate())
        {
            yield return 0;

            //TODO CHEAT MC CHEATSON 
            if (Input.GetKey(KeyCode.K))
            {
                break;
            }
        }

        yield return 0;

        objective.Complete();
        objective.ValidateRoutine = null;
    }

    public void QuestComplete(Quest quest)
    {
        if(quest.Tutorial)
        {
            WorldMissionPanelUI.Instance.QuestComplete();
        }

        ActiveQuests.Remove(quest);
        AddCompletedQuest(quest);
        
        if(!activeQuestsButton.interactable)
        {
            for(int i=0;i<questsContainer.childCount;i++)
            {
                QuestHeadlineUI headline = questsContainer.GetChild(i).GetComponent<QuestHeadlineUI>();
                if (headline != null && headline.CurrentQuest == quest)
                {
                    headline.Complete();
                }
            }
        }

        if (quest.CompletionLetter != null)
        {
            Dictionary<string, object> letterParameters = new Dictionary<string, object>();

            Character sender = CORE.Instance.GetCharacter(quest.CompletionLetter.PresetSender.name);

            letterParameters.Add("Letter_From", sender);
            letterParameters.Add("Letter_To", CORE.PC);

            LetterDispenserEntity.Instance.DispenseLetter(new Letter(quest.CompletionLetter, letterParameters));
        }

        if (quest.NextQuest != null)
        {
            AddNewQuest(quest.NextQuest);
        }
    }

    public void ObjectiveComplete(QuestObjective objective)
    {
        Quest parentQuest = null;

        foreach(Quest quest in ActiveQuests)
        {
            if (quest.HasObjective(objective))
            {
                parentQuest = quest;
                break;
            }
        }

        if(parentQuest == null)
        {
            return;
        }

        if(parentQuest.Tutorial)
        {
            WorldMissionPanelUI.Instance.ObjectiveComplete();
        }

        if (!activeQuestsButton.interactable)
        {
            foreach (Transform questHeadlineTransform in questsContainer)
            {
                QuestHeadlineUI headline = questHeadlineTransform.GetComponent<QuestHeadlineUI>();

                if (headline == null)
                {
                    continue;
                }

                if (headline.CurrentQuest != parentQuest)
                {
                    continue;
                }

                headline.Show();

                QuestContentUI content = headline.ShowingObject.GetComponent<QuestContentUI>();
                content.Refresh();
                content.Notify();
            }
        }
        else
        {
            Notification.Add(1);
        }

        GlobalMessagePrompterUI.Instance.Show(objective.name + " is complete!", 1f, Color.green);

        if (objective.WorldMarker != null)
        {
            objective.WorldMarker.gameObject.SetActive(false);
            objective.WorldMarker = null;
        }

        foreach (QuestObjective qObjective in parentQuest.Objectives)
        {
            if(!qObjective.IsComplete)
            {
                return;
            }
        }

        GlobalMessagePrompterUI.Instance.Show(parentQuest.name + " is complete!", 1f, Color.green);
        QuestComplete(parentQuest);

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

        Notification.Wipe();
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

    public void AddCompletedQuest(Quest quest)
    {
        CompletedQuests.Add(quest);
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
