﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Database", menuName = "DataObjects/Game Database", order = 2)]
public class GameDB : ScriptableObject
{
    //TODO Add tooltips for each serialized field.
    public List<RaceSet> Races = new List<RaceSet>();

    public Character PlayerCharacter;

    public Character HumanReference;

    public Property EmptyProperty;

    public Property DefaultLocationProperty;

    public PropertyTrait PublicAreaTrait;

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

    
}
