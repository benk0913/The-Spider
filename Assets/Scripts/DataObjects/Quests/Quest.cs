using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestPreset", menuName = "DataObjects/Quests/Quest", order = 2)]
public class Quest : ScriptableObject, ISaveFileCompatible
{
    public Sprite Icon;

    [TextArea(2, 3)]
    public string Description;

    public QuestObjective[] Objectives;

    public QuestReward[] Rewards;

    public Character RelevantCharacter;

    public LocationEntity RelevantLocation;

    public List<string> InfoGivenOnCharacter = new List<string>();

    public Quest NextQuest;

    public bool Tutorial = false;

    public LetterPreset CompletionLetter;

    public Character ForCharacter;

    public PopupDataPreset CompletePopup;

    public Quest CreateClone()
    {
        Quest quest = Instantiate(this);

        quest.name = this.name;

        quest.relevantCharacterID = this.relevantCharacterID;
        quest.relevantLocationID = this.relevantLocationID;
        quest.forCharacterID = this.forCharacterID;

        List<QuestObjective> objectives = new List<QuestObjective>();
        foreach(QuestObjective objective in this.Objectives)
        {
            QuestObjective objectiveClone = objective.CreateClone();
            objectiveClone.ParentQuest = quest;
            objectives.Add(objectiveClone);
        }


        List<QuestReward> rewards = new List<QuestReward>();
        foreach (QuestReward reward in this.Rewards)
        {
            QuestReward rewardClone = reward.CreateClone();
            rewards.Add(rewardClone);
        }

        quest.Rewards = rewards.ToArray();


        if (RelevantCharacter != null)
        {
            quest.RelevantCharacter = CORE.Instance.GetCharacter(RelevantCharacter.name);
        }
        else
        {
            if (objectives.Count > 0)
            {
                QuestObjective objectiveWithCharacter = objectives.Find(x => x.TargetCharacter != null);

                if (objectiveWithCharacter != null)
                {
                    quest.RelevantCharacter = objectiveWithCharacter.TargetCharacter;
                }
            }
        }

        if (RelevantLocation != null)
        {
            quest.RelevantLocation = CORE.Instance.Locations.Find(x => x == quest.RelevantLocation);

            if(quest.RelevantLocation == null)
            {
                quest.RelevantLocation = CORE.Instance.Locations.Find(x => x.name == quest.RelevantLocation.name);
            }
        }
        else
        {
            if(objectives.Count > 0)
            {
                QuestObjective objectiveWithLocation = objectives.Find(x => x.TargetLocation != null);

                if(objectiveWithLocation != null)
                {
                    quest.RelevantLocation = objectiveWithLocation.TargetLocation;
                }
            }
        }

        if (ForCharacter != null)
        {
            quest.ForCharacter = CORE.Instance.GetCharacter(ForCharacter.name);
        }

        quest.Objectives = objectives.ToArray();

        return quest;
    }

    public bool HasObjective(QuestObjective objective)
    {
        foreach(QuestObjective qObjective in Objectives)
        {
            if(qObjective == objective)
            {
                return true;
            }
        }

        return false;
    }

    QuestObjective GetObjective(string objectiveKey)
    {
        foreach(QuestObjective objective in Objectives)
        {
            if(objective.name == objectiveKey)
            {
                return objective;
            }
        }

        return null;
    }

    public void FromJSON(JSONNode node)
    {
        List<QuestObjective> objectives = new List<QuestObjective>();

        for (int i = 0; i < node["Objectives"].Count; i++)
        {
            GetObjective(node["Objectives"][i]["Key"]).IsComplete = bool.Parse(node["Objectives"][i]["IsComplete"]);
        }

        relevantCharacterID = node["RelevantCharacterID"];
        relevantLocationID  = node["RelevantLocationID"];
        forCharacterID = node["ForCharacter"];
    }

    public string relevantCharacterID;
    public string relevantLocationID;
    public string forCharacterID;
    
    public void ImplementIDs()
    {
        if (!string.IsNullOrEmpty(relevantCharacterID))
        {
            RelevantCharacter = CORE.Instance.Characters.Find(x => x.ID == relevantCharacterID);
        }

        if (!string.IsNullOrEmpty(relevantLocationID))
        {
            RelevantLocation = CORE.Instance.Locations.Find(x => x.ID == relevantLocationID);
        }
        
        if (!string.IsNullOrEmpty(forCharacterID))
        {
            ForCharacter = CORE.Instance.Characters.Find(x => x.ID == forCharacterID);
        }
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["Key"] = this.name;

        for (int i=0;i<Objectives.Length;i++)
        {
            node["Objectives"][i]["Key"] = Objectives[i].name;
            node["Objectives"][i]["IsComplete"] = Objectives[i].IsComplete.ToString();
        }

        if (RelevantCharacter != null)
        {
            node["RelevantCharacterID"] = RelevantCharacter.ID;
        }

        if (RelevantLocation != null)
        {
            node["RelevantLocationID"] = RelevantLocation.ID;
        }

        if (ForCharacter != null)
        {
            node["ForCharacter"] = this.ForCharacter.ID;
        }

        return node;
    }
}