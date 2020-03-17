using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesEntity : MonoBehaviour
{
    public List<QuestObjective> Objectives = new List<QuestObjective>();

    [SerializeField]
    GameObject TargetObject;

    private void OnEnable()
    {
        if(QuestsPanelUI.Instance == null)
        {
            return;
        }

        Refresh();
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("ObjectiveComplete", Refresh);
        CORE.Instance.SubscribeToEvent("QuestStarted", Refresh);
    }

    public void Refresh()
    {
        foreach (QuestObjective objective in Objectives)
        {
            foreach(Quest quest in QuestsPanelUI.Instance.ActiveQuests)
            {
                if(quest.ForCharacter == null)
                {
                    continue;
                }

                if(quest.ForCharacter != CORE.PC)
                {
                    continue;
                }

                QuestObjective tempObj = quest.GetObjective(objective.name);

                if(tempObj == null)
                {
                    continue;
                }

                if(tempObj.IsComplete)
                {
                    continue;
                }

                TargetObject.SetActive(true);
                return;
            }
        }

        TargetObject.SetActive(false);
    }
}
