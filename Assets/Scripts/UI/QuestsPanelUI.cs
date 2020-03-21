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

    public List<Character> RelevantCharacters = new List<Character>();
    


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
        AddNewExistingQuest(newQuest);
    }

    public void AddNewExistingQuest(Quest quest, bool loadedFromSave = false)
    {
        if(!loadedFromSave)
        {
            AudioControl.Instance.Play("quest_accept");
        }

        ActiveQuests.Add(quest);

        AddQuestToContainer(quest);

        if (quest.RelevantCharacter != null)
        {
            quest.RelevantCharacter.Pinned = true;

            foreach (string infoKey in quest.InfoGivenOnCharacter)
            {
                quest.RelevantCharacter.Known.Know(infoKey, quest.ForCharacter);
            }
        }

        if (quest.Tutorial)
        {
            WorldMissionPanelUI.Instance.ShowQuest(quest);
        }

        foreach (QuestObjective objective in quest.Objectives)
        {
            if (objective.ValidateRoutine != null)
            {
                CORE.Instance.StopCoroutine(objective.ValidateRoutine);
            }

            objective.ValidateRoutine = CORE.Instance.StartCoroutine(ValidateObjectiveRoutine(objective));

            if (quest.ForCharacter != CORE.PC)
            {
                continue;
            }
        }

        if (quest.ForCharacter == CORE.PC)
        {
            Notification.Add(1);
        }

        if (quest.RelevantCharacter != null)
        {
            RelevantCharacters.Add(quest.RelevantCharacter);
        }

        if(!loadedFromSave)
        {
            if(quest.OnStartQuestAction != null)
            {
                quest.OnStartQuestAction.Activate();
            }
        }

        CORE.Instance.InvokeEvent("QuestStarted");
    }

    public void RemoveExistingQuest(Quest givenQuest)
    {
        Quest quest = ActiveQuests.Find(x => x.name == givenQuest.name);

        if(quest == null)
        {
            Debug.LogError("Couldn't Find Quest " + givenQuest.name);
            return;
        }

        if (quest.ForCharacter == CORE.PC)
        {
            if (!activeQuestsButton.interactable)
            {
                for (int i = 0; i < questsContainer.childCount; i++)
                {
                    QuestHeadlineUI headline = questsContainer.GetChild(i).GetComponent<QuestHeadlineUI>();
                    if (headline != null && headline.CurrentQuest == quest)
                    {
                        headline.Complete();
                    }
                }
            }
        }

        ActiveQuests.Remove(quest);

        if (quest.Tutorial)
        {
            WorldMissionPanelUI.Instance.HideQuest(quest);
        }

        foreach (QuestObjective objective in quest.Objectives)
        {
            if (objective.ValidateRoutine != null)
            {
                CORE.Instance.StopCoroutine(objective.ValidateRoutine);
            }
        }

        if (quest.ForCharacter == CORE.PC)
        {
            Notification.Add(0);
        }

        if (quest.RelevantCharacter != null)
        {
            RelevantCharacters.Remove(quest.RelevantCharacter);
        }
    }

    IEnumerator ValidateObjectiveRoutine(QuestObjective objective)
    {
        yield return 0;

        while(!objective.Validate())
        {
            if (objective.ShowObjectiveMarker)
            {
                objective.RefreshMarker();
            }

            if(objective.Failed())
            {
                objective.ValidateRoutine = null;
                yield break;
            }

            yield return 0;
        }

        yield return 0;

        objective.Complete();
        objective.ValidateRoutine = null;
    }

    public void QuestComplete(Quest quest)
    {
        if (quest.HasSounds)
        {
            AudioControl.Instance.Play("quest_complete");
        }

        if (quest.Tutorial)
        {
            WorldMissionPanelUI.Instance.QuestComplete();
        }

        ActiveQuests.Remove(quest);
        AddCompletedQuest(quest);

        if (quest.ForCharacter == CORE.PC)
        {
            if (!activeQuestsButton.interactable)
            {
                for (int i = 0; i < questsContainer.childCount; i++)
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

                CORE.Instance.DelayedInvokation(1f, () => { LetterDispenserEntity.Instance.DispenseLetter(new Letter(quest.CompletionLetter, letterParameters)); });
            }
        }

        foreach(QuestReward reward in quest.Rewards)
        {
            reward.Claim(quest.ForCharacter);
        }

        if (quest.NextQuest != null)
        {
            Quest questClone = quest.NextQuest.CreateClone();
            questClone.ForCharacter = quest.ForCharacter;
            AddNewExistingQuest(questClone);
        }

        if(quest.CompletePopup != null)
        {
            PopupWindowUI.Instance.AddPopup(new PopupData(quest.CompletePopup,null,null,null));
        }

        if (quest.RelevantCharacter != null)
        {
            RelevantCharacters.Remove(quest.RelevantCharacter);
        }
    }

    public void ObjectiveComplete(QuestObjective objective)
    {
        if (objective.ParentQuest != null && objective.ParentQuest.HasSounds)
        {
            AudioControl.Instance.Play("quest_objective_complete");
        }

        Quest parentQuest = objective.ParentQuest;

        if(parentQuest == null)
        {
            return;
        }

        if (parentQuest.ForCharacter == CORE.PC)
        {
            if (parentQuest.Tutorial)
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


        }

        foreach (QuestObjective qObjective in parentQuest.Objectives)
        {
            if(objective == qObjective)
            {
                continue;
            }

            if (!qObjective.IsComplete)
            {
                return;
            }
        }

        if(parentQuest.ForCharacter == CORE.PC)
        {
            GlobalMessagePrompterUI.Instance.Show(parentQuest.name + " is complete!", 1f, Color.green);
        }

        CORE.Instance.InvokeEvent("ObjectiveComplete");

        QuestComplete(parentQuest);

    }


    public void ObjectiveFailed(QuestObjective objective)
    {
        Quest parentQuest = objective.ParentQuest;

        if (parentQuest == null)
        {
            return;
        }

        if (parentQuest.ForCharacter == CORE.PC)
        {

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

            GlobalMessagePrompterUI.Instance.Show(objective.name + " has failed!", 3f, Color.red);


        }

        foreach (QuestObjective qObjective in parentQuest.Objectives)
        {
            objective.Complete();
        }

        if (parentQuest.ForCharacter == CORE.PC)
        {
            GlobalMessagePrompterUI.Instance.Show(parentQuest.name + " has failed!", 3f, Color.red);
        }

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
        if(quest.ForCharacter != CORE.PC)
        {
            return;
        }

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


    public bool HasQuest(Quest questAttachment, Character forCharacter)
    {
        foreach(Quest quest in ActiveQuests)
        {
            if(quest.ForCharacter == forCharacter && quest.name == questAttachment.name)
            {
                return true;
            }
        }

        foreach (Quest quest in CompletedQuests)
        {
            if (quest.ForCharacter == forCharacter && quest.name == questAttachment.name)
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
        ActiveQuests.ForEach((x) =>
        {
            foreach (QuestObjective objective in x.Objectives)
            {
                StopCoroutine(objective.ValidateRoutine);
            }
        });

        ActiveQuests.Clear();
        CompletedQuests.Clear();

        for (int i = 0; i < node["ActiveQuests"].Count; i++)
        {
            Quest quest = CORE.Instance.Database.GetQuest(node["ActiveQuests"][i]["Key"]).CreateClone();
            quest.FromJSON(node["ActiveQuests"][i]);

            AddNewExistingQuest(quest, true);
        }

        for (int i = 0; i < node["CompletedQuests"].Count; i++)
        {
            Quest quest = CORE.Instance.Database.GetQuest(node["CompletedQuests"][i]["Key"]).CreateClone();
            quest.FromJSON(node["CompletedQuests"][i]);

            AddCompletedQuest(quest);
        }

    }

    public void ImplementIDs()
    {
        foreach(Quest quest in ActiveQuests)
        {
            quest.ImplementIDs();
        }

        foreach (Quest quest in CompletedQuests)
        {
            quest.ImplementIDs();
        }
    }
}
