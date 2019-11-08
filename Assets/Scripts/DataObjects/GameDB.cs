using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Database", menuName = "DataObjects/Game Database", order = 2)]
public class GameDB : ScriptableObject
{
    public GameStats Stats;

    //TODO Add tooltips for each serialized field.
    public List<RaceSet> Races = new List<RaceSet>();

    public Character HumanReference;

    public Character GOD;

    public Property EmptyProperty;

    public Property DefaultLocationProperty;

    public PropertyTrait PublicAreaTrait;
    public PropertyTrait LawAreaTrait;
    public PropertyTrait WildernessAreaTrait;
    public PropertyTrait RumorsHubTrait;
    public PropertyTrait HouseOfWorshipTrait;
    public PropertyTrait HouseOfPleasureTrait;
    public PropertyTrait BurialGroundTrait;

    public GameObject UnknownFigurePrefab;

    public Faction DefaultFaction;
    public Faction NoFaction;

    public PlotType UniquePlotType;

    public Rumor CustomRumor;

    public List<Character> PresetCharacters = new List<Character>();

    public List<Property> Properties = new List<Property>();
    public List<Faction> Factions = new List<Faction>();

    public AgentAction SleepAction;

    public List<PlayerAction> PlayerActionsOnAgent = new List<PlayerAction>();

    public List<AgentAction> AgentActionsOnAgent = new List<AgentAction>();

    public List<LongTermTask> LongTermTasks = new List<LongTermTask>();

    public BonusType[] BonusTypes;

    public Trait UnknownTrait;

    public Trait[] Traits;

    public Quest TutorialQuest;

    public Quest[] AllQuests;

    public Item[] AllItems;

    public TimelineInstance[] Timeline;

    public Item GetItem(string itemKey)
    {
        for (int i = 0; i < AllItems.Length; i++)
        {
            if (AllItems[i].name == itemKey)
            {
                return AllItems[i];
            }
        }

        return null;
    }

    public Quest GetQuest(string questName)
    {
        for (int i = 0; i < AllQuests.Length; i++)
        {
            if (AllQuests[i].name == questName)
            {
                return AllQuests[i];
            }
        }

        return null;
    }

    public Rumor GetRumor(string rumorName)
    {
        for(int i=0;i<Timeline.Length;i++)
        {
            for(int r=0;r<Timeline[i].Rumors.Length;r++)
            {
                if (Timeline[i].Rumors[r].name == rumorName)
                {
                    return Timeline[i].Rumors[r];
                }
            }
        }

        return null;
    }

    public LongTermTask GetLongTermTaskByName(string taskName)
    {
        foreach (LongTermTask longTermTask in LongTermTasks)
        {
            if (longTermTask.name == taskName)
            {
                return longTermTask;
            }
        }

        return null;
    }

    public Property GetPropertyByName(string propertyName)
    {
        foreach (Property property in Properties)
        {
            if (property.name == propertyName)
            {
                return property;
            }
        }

        return null;
    }

    public Faction GetFactionByName(string factionName)
    {
        foreach (Faction faction in Factions)
        {
            if (faction.name == factionName)
            {
                return faction;
            }
        }

        return DefaultFaction;
    }

    public RaceSet GetRace(string raceName, bool fallback = true)
    {
        for (int i = 0; i < Races.Count; i++)
        {
            if (raceName == Races[i].name)
            {
                return Races[i];
            }
        }

        if (fallback)
        {
            return Races[0];
        }

        return null;
    }


    public Trait GetTrait(string traitName)
    {
        for(int i=0;i<Traits.Length;i++)
        {
            if(Traits[i].name == traitName)
            {
                return Traits[i];
            }
        }

        return null;
    }

    public Trait GetRandomTrait()
    {
        return Traits[Random.Range(0, Traits.Length)];
    }

    public Trait[] GetRandomTraits()
    {
        List<Trait> GeneratedTraits = new List<Trait>();

        for(int i=0;i<Traits.Length;i++)
        {
            if (Random.Range(0f, 1f) <= Traits[i].DropChance)
            {
                GeneratedTraits.Add(Traits[i]);
            }
        }

        return GeneratedTraits.ToArray();
    }

    public BonusType GetBonusType(string bonusTypeName)
    {
        foreach(BonusType bonusType in BonusTypes)
        {
            if(bonusTypeName == bonusType.name)
            {
                return bonusType;
            }
        }

        return null;
    }



    public AgentAction[] EventActions;

    public AgentAction GetEventAction(string key)
    {
        foreach(AgentAction action in EventActions)
        {
            if(action.name == key)
            {
                return action;
            }
        }

        return null;
    }

}

[System.Serializable]
public class GameStats
{
    public float GlobalRevenueMultiplier = 1f;
}

[System.Serializable]
public class TimelineInstance
{
    public Rumor[] Rumors;

    public LetterPreset[] Letters;
}
   