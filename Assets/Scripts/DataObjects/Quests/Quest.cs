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

    public List<string> InfoGivenOnCharacter = new List<string>();

    public Quest NextQuest;

    public bool Tutorial = false;

    public LetterPreset CompletionLetter;

    public Character ForCharacter;

    public Quest CreateClone()
    {
        Quest quest = Instantiate(this);

        quest.name = this.name;

        List<QuestObjective> objectives = new List<QuestObjective>();
        foreach(QuestObjective objective in this.Objectives)
        {
            QuestObjective objectiveClone = objective.CreateClone();
            objectiveClone.ParentQuest = quest;
            objectives.Add(objectiveClone);
        }

        quest.Objectives = objectives.ToArray();


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

        if (ForCharacter != null)
        {
            ForCharacter = CORE.Instance.GetCharacter(ForCharacter.name);
        }

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
    }

    public void ImplementIDs()
    {
        
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        for(int i=0;i<Objectives.Length;i++)
        {
            node["Objectives"][i]["Key"] = Objectives[i].name;
            node["Objectives"][i]["IsComplete"] = Objectives[i].IsComplete.ToString();
        }

        return node;
    }
}