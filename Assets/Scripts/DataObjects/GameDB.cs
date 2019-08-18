using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Database", menuName = "DataObjects/Game Database", order = 2)]
public class GameDB : ScriptableObject
{
    public GameStats Stats;

    //TODO Add tooltips for each serialized field.
    public List<RaceSet> Races = new List<RaceSet>();

    public Character PlayerCharacter;

    public Character HumanReference;

    public Character GOD;

    public Property EmptyProperty;

    public Property DefaultLocationProperty;

    public PropertyTrait PublicAreaTrait;

    public PropertyTrait LawAreaTrait;

    public Faction DefaultFaction;

    public PlotType UniquePlotType;


    public List<Character> PresetCharacters = new List<Character>();

    public List<Property> Properties = new List<Property>();
    public List<Faction> Factions = new List<Faction>();
    
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

    public List<PlayerAction> PlayerActionsOnAgent = new List<PlayerAction>();

    public List<AgentAction> AgentActionsOnAgent = new List<AgentAction>();

    public BonusType[] BonusTypes;

    public Trait[] Traits;

    
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
