﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scheme Type", menuName = "DataObjects/Plots/Scheme Type", order = 2)]
public class SchemeType : ScriptableObject
{
    [TextArea(3, 6)]
    public string Description;
    public Sprite Icon;

    public PlotMethod BaseMethod;

    public List<PlotEntry> PossibleEntries;
    public List<PlotMethod> PossibleMethods;

    public List<ScenarioPopup> EntryScenarios = new List<ScenarioPopup>();
    public List<ScenarioPopup> ExitScenarios = new List<ScenarioPopup>();

    public List<ScenarioPopup> AgentMethodFailureScenarios = new List<ScenarioPopup>();
    public List<ScenarioPopup> AgentMethodSuccessScenarios = new List<ScenarioPopup>();

    public ScenarioPopup DuelResultArrestScenario;
    public ScenarioPopup DuelResultWoundScenario;
    public ScenarioPopup DuelResultDeathScenario;

    public bool TargetIsLocation;

    public virtual PopupDataPreset GetScenarioPopup(PlotEntry entry, PlotMethod method, List<ScenarioPopup> scenarioList)
    {
        ScenarioPopup scenario = scenarioList.Find(x =>
        (x.EntryMatch == entry || x.EntryMatch == null) &&
        (x.MethodMatch == method || x.MethodMatch == null));

        if (scenario == null)
        {
            return null;
        }

        return scenario.PopupData;
    }

    public virtual void Execute(PlotData data)
    {
        Init(data, ()=> 
        {
            List<Character> entryTargets = new List<Character>();
            entryTargets.AddRange(data.TargetParticipants);

            List<Character> entryParticipants = new List<Character>();
            entryParticipants.AddRange(data.Participants);

            //Dueling
            Dueling(data,
                (result) =>
                {
                    if (result.FailReason == null)
                    {
                        WinResult(result);
                    }
                    else
                    {
                        LoseResult(result);
                    }
                });
        });
    }

    public virtual void Init(PlotData data, System.Action OnInitComplete = null)
    {
        foreach (Character participant in data.Participants)
        {
            foreach (Item item in data.Method.ItemsRequired)
            {
                data.Requester.Belogings.Remove(data.Requester.GetItem(item.name));
            }
        }

        foreach (Item item in data.Entry.ItemsRequired)
        {
            data.Requester.Belogings.Remove(data.Requester.GetItem(item.name));
        }


        PopupWindowUI.Instance.AddPopup(
            new PopupData(GetScenarioPopup(data.Entry, data.Method, EntryScenarios), data.Participants, data.TargetParticipants, OnInitComplete));
    }

    public virtual void Dueling(PlotData data,
        System.Action<DuelResultData> onComplete)
    {
        Character targetCharacter = null;
        LocationEntity InLocation = null;
        if (data.Target.GetType() == typeof(PortraitUI))
        {
            targetCharacter = ((PortraitUI)data.Target).CurrentCharacter;
            InLocation = targetCharacter.CurrentLocation;
        }
        else if (data.GetType() == typeof(LocationEntity))
        {
            InLocation = (LocationEntity)data.Target;
        }

        if(data.TargetParticipants.Count == 0)
        {
            WinResult(new DuelResultData(data, data.Participants, new List<Character>(), null));
            return;
        }

        PlottingDuelUI.Instance.Show(data, InLocation, onComplete);

        //if (method == BruteMethod && !(targetCharacter != null && targetParticipants[0] == targetCharacter))
        //{
        //    AggressiveDuelResult(randomParticipant, targetParticipants[0], false);
        //}
        
    }

    public virtual void WinResult(DuelResultData result)
    {
        foreach (Character participant in result.Plot.Participants)
        {
            if (!result.LeftParticipants.Contains(participant))
            {
                AggressiveDuelResult(participant, false);
            }
        }

        foreach (Character target in result.Plot.TargetParticipants)
        {
            if (!result.LeftTargets.Contains(target))
            {
                AggressiveDuelResult(target, false);
            }
        }


        if (result.Plot.Target.GetType() == typeof(LocationEntity))
        {
            LocationEntity targetLocation = (LocationEntity)result.Plot.Target;
            CORE.Instance.OnSchemeWin.Invoke(this, targetLocation, null);
        }
        else if (result.Plot.Target.GetType() == typeof(PortraitUI))
        {
            Character targetCharacter = ((PortraitUI)result.Plot.Target).CurrentCharacter;
            CORE.Instance.OnSchemeWin.Invoke(this, null, targetCharacter);
        }
    }

    public virtual void LoseResult(DuelResultData result)
    {
        foreach(Character participant in result.Plot.Participants)
        {
            if(!result.LeftParticipants.Contains(participant))
            {
                AggressiveDuelResult(participant, true);
            }
        }

        foreach (Character target in result.Plot.TargetParticipants)
        {
            if (!result.LeftTargets.Contains(target))
            {
                AggressiveDuelResult(target, false);
            }
        }


    }

    public virtual void AggressiveDuelResult(Character loser, bool allowArrest = false)
    {
        float randomResult = Random.Range(0f, 1f);

        if (randomResult > 0.5f)
        {
            if (allowArrest) // Not a Good guy?
            {
                PopupWindowUI.Instance.AddPopup(
                    new PopupData(DuelResultArrestScenario.PopupData, new List<Character> { loser }, new List<Character>()
                    , () => { CORE.Instance.Database.GetEventAction("Get Arrested").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));

            }
            else
            {
                PopupWindowUI.Instance.AddPopup(new PopupData(DuelResultWoundScenario.PopupData, new List<Character> { loser }, new List<Character>()
                , () => { CORE.Instance.Database.GetEventAction("Wounded").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));
                
            }
        }
        else
        {
            PopupWindowUI.Instance.AddPopup(new PopupData(DuelResultDeathScenario.PopupData, new List<Character> { loser }, new List<Character>()
                 , () => { CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));
        }
    }
}

[System.Serializable]
public class ScenarioPopup
{
    public PlotEntry EntryMatch;
    public PlotMethod MethodMatch;
    public PopupDataPreset PopupData;
}

public class PlotData
{
    public Character Requester;
    public Character Plotter;
    public List<Character> Participants;
    public List<Character> TargetParticipants;
    public AgentInteractable Target;
    public PlotMethod Method;
    public PlotEntry Entry;
    public PlotMethod BaseMethod;

    public PlotData(Character requester, Character plotter, List<Character> participants, List<Character> targetParticipants, AgentInteractable target, PlotMethod method, PlotEntry entry)
    {
        this.Requester = requester;
        this.Plotter = plotter;
        this.Participants = participants;
        this.TargetParticipants = targetParticipants;
        this.Target = target;
        this.Method = method;
        this.Entry = entry;
    }
}

public class DuelResultData
{
    public PlotData Plot;

    public List<Character> LeftParticipants;
    public List<Character> LeftTargets;

    public FailReason FailReason;

    public DuelResultData(PlotData plotData, List<Character> leftParticipants, List<Character> leftTargets, FailReason reason = null)
    {
        this.Plot = plotData;
        this.LeftParticipants = leftParticipants;
        this.LeftTargets = leftTargets;
        this.FailReason = reason;
    }
}
