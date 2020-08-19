using System.Collections;
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

            if (data.Target.GetType() == typeof(LocationEntity))
            {
                LocationEntity location = (LocationEntity)data.Target;

                if (location.OwnerCharacter != null)
                {
                    if (location.OwnerCharacter.CurrentFaction.Relations != null)
                    {
                        location.OwnerCharacter.CurrentFaction.Relations.GetRelations(entryParticipants[0].CurrentFaction).TotalValue -= 3; //VANDETTA
                    }

                    if (entryParticipants[0].CurrentFaction.Relations != null)
                    {
                        entryParticipants[0].CurrentFaction.Relations.GetRelations(location.OwnerCharacter.CurrentFaction).TotalValue += 2; //GOT MY VANDETTA
                    }
                }
            }
            else if(data.Target.GetType() == typeof(PortraitUI) || data.Target.GetType() == typeof(PortraitUIEmployee))
            {
                Character targetChar = data.TargetCharacter;

                if (targetChar != null && targetChar.CurrentFaction != null && targetChar.CurrentFaction.Relations != null)
                {
                    if (targetChar.CurrentFaction.Relations != null)
                    {
                        targetChar.CurrentFaction.Relations.GetRelations(entryParticipants[0].CurrentFaction).TotalValue -= 3; //VANDETTA
                    }

                    if (entryParticipants[0].CurrentFaction.Relations != null)
                    {
                        entryParticipants[0].CurrentFaction.Relations.GetRelations(targetChar.CurrentFaction).TotalValue += 2; //GOT MY VANDETTA
                    }
                }
            }

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
        if (data.Target.GetType() == typeof(PortraitUI) || data.Target.GetType() == typeof(PortraitUIEmployee))
        {
            targetCharacter = data.TargetCharacter;
            InLocation = targetCharacter.CurrentLocation;
        }
        else if (data.Target.GetType() == typeof(LocationEntity))
        {
            InLocation = (LocationEntity)data.Target;
        }

        if(data.TargetParticipants.Count == 0)
        {
            WinResult(new DuelResultData(data, data.Participants, new List<Character>(), null));
            return;
        }

        data.Participants.ForEach((x) => x.Known.Know("Appearance", data.TargetParticipants[0].TopEmployer));
        data.TargetParticipants.ForEach((x) => x.Known.Know("Appearance", data.Participants[0].TopEmployer));
        
        PlottingDuelUI.Instance.Show(data, InLocation, onComplete);

        //if (method == BruteMethod && !(targetCharacter != null && targetParticipants[0] == targetCharacter))
        //{
        //    AggressiveDuelResult(randomParticipant, targetParticipants[0], false);
        //}
        
    }

    public virtual void WinResult(DuelResultData result)
    {
        if(result.Plot.Plotter.TopEmployer == CORE.PC)
        {
            AudioControl.Instance.Play("plot_win");
        }

        result.Plot.Plotter.Reputation += 1;
        result.Plot.Plotter.TopEmployer.Reputation += 1;

        if (result.Plot.Target.GetType() == typeof(LocationEntity))
        {
            LocationEntity location = (LocationEntity)result.Plot.Target;

            if (location.OwnerCharacter != null && location.OwnerCharacter.CurrentFaction.Relations != null)
            {
                location.OwnerCharacter.CurrentFaction.Relations.GetRelations(result.Plot.Participants[0].CurrentFaction).TotalValue -= 3; //VANDETTA
                result.Plot.Participants[0].CurrentFaction.Relations.GetRelations(location.OwnerCharacter.CurrentFaction).TotalValue += 4; //GOT MY VANDETTA
            }

            if (result.Plot.Plotter.TopEmployer == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject("GoodReputationCollectedWorld",
                  location.transform,
                  StatsViewUI.Instance.transform,
                  null,
                  false);
            }

            if (location.OwnerCharacter != null)
            {
                location.OwnerCharacter.Reputation -= 1;
                location.OwnerCharacter.TopEmployer.Reputation -= 1;

                if (location.OwnerCharacter.TopEmployer == CORE.PC)
                {
                    CORE.Instance.SplineAnimationObject("BadReputationCollectedWorld",
                      location.transform,
                      StatsViewUI.Instance.transform,
                      null,
                      false);
                }
            }
        }
        else if (result.Plot.Target.GetType() == typeof(PortraitUI) || result.Plot.Target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetChar = result.Plot.TargetCharacter;

            if (targetChar != null && targetChar.CurrentFaction.Relations != null)
            {
                targetChar.CurrentFaction.Relations.GetRelations(result.Plot.Participants[0].CurrentFaction).TotalValue -= 2; //VANDETTA
                result.Plot.Participants[0].CurrentFaction.Relations.GetRelations(targetChar.CurrentFaction).TotalValue += 3; //GOT MY VANDETTA
            }

            if (result.Plot.Plotter.TopEmployer == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject("GoodReputationCollectedWorld",
                  targetChar.CurrentLocation.transform,
                  StatsViewUI.Instance.transform,
                  null,
                  false);
            }

            if (targetChar != null)
            {
                targetChar.Reputation -= 1;
                targetChar.TopEmployer.Reputation -= 1;

                if (targetChar.TopEmployer == CORE.PC)
                {
                    CORE.Instance.SplineAnimationObject("BadReputationCollectedWorld",
                      targetChar.CurrentLocation.transform,
                      StatsViewUI.Instance.transform,
                      null,
                      false);
                }
            }
        }
        else
        {
            Character targetChar = result.Plot.TargetCharacter;

            if (targetChar != null && targetChar.CurrentFaction.Relations != null)
            {
                targetChar.CurrentFaction.Relations.GetRelations(result.Plot.Participants[0].CurrentFaction).TotalValue -= 2; //VANDETTA
                result.Plot.Participants[0].CurrentFaction.Relations.GetRelations(targetChar.CurrentFaction).TotalValue += 3; //GOT MY VANDETTA
            }
        }

        foreach (Character participant in result.Plot.Participants)
        {
            if (!result.LeftParticipants.Contains(participant))
            {
                AggressiveDuelResult(participant, false);
            }
        }

        if (((result.Plot.Target.GetType() == typeof(PortraitUI)) || result.Plot.Target.GetType() == typeof(PortraitUIEmployee)) 
            && result.Plot.Participants.Contains(result.Plot.TargetCharacter))
        {
            result.Plot.TargetParticipants.Remove(result.Plot.TargetCharacter);
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
        else if (result.Plot.Target.GetType() == typeof(PortraitUI) || result.Plot.Target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetCharacter = result.Plot.TargetCharacter ;
            CORE.Instance.OnSchemeWin.Invoke(this, null, targetCharacter);
        }
    }

    public virtual void LoseResult(DuelResultData result)
    {
        if (result.Plot.Plotter.TopEmployer == CORE.PC)
        {
            AudioControl.Instance.Play("plot_fail");
        }

        foreach (Character participant in result.Plot.Participants)
        {
            if(!result.LeftParticipants.Contains(participant))
            {
                AggressiveDuelResult(participant, true);
            }
        }

        if (((result.Plot.Target.GetType() == typeof(PortraitUI)) || result.Plot.Target.GetType() == typeof(PortraitUIEmployee))
            && result.Plot.Participants.Contains(result.Plot.TargetCharacter))
        {
            result.Plot.TargetParticipants.Remove(result.Plot.TargetCharacter);
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
            if (allowArrest && loser.CurrentFaction.name != "Constabulary")
            {
                PopupWindowUI.Instance.AddPopup(
                    new PopupData(DuelResultArrestScenario.PopupData, new List<Character> { loser }, new List<Character>()
                    , () => { CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));

            }
            else
            {
                PopupWindowUI.Instance.AddPopup(new PopupData(DuelResultWoundScenario.PopupData, new List<Character> { loser }, new List<Character>()
                , () => { CORE.Instance.Database.GetAgentAction("Wounded").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));
                
            }
        }
        else
        {
            PopupWindowUI.Instance.AddPopup(new PopupData(DuelResultDeathScenario.PopupData, new List<Character> { loser }, new List<Character>()
                 , () => { CORE.Instance.Database.GetAgentAction("Death").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));
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
    public AgentInteractable Target
    {
        get
        {
            return _target;
        }
        set
        {
            if (value != null && (value.GetType() == typeof(PortraitUI) || value.GetType() == typeof(PortraitUIEmployee)))
            {
                TargetCharacter = ((PortraitUI)value).CurrentCharacter;
            }

            _target = value;
        }
    }
    AgentInteractable _target;
    public Character TargetCharacter;
    public PlotMethod Method;
    public PlotEntry Entry;
    public PlotMethod BaseMethod;
    public string Name;

    public List<DuelProc> ProcsUsed = new List<DuelProc>();

    public PlotData(string name,Character requester, Character plotter, List<Character> participants, List<Character> targetParticipants, AgentInteractable target, PlotMethod method, PlotEntry entry)
    {
        this.Requester = requester;
        this.Plotter = plotter;
        this.Participants = participants;
        this.TargetParticipants = targetParticipants;
        this.Target = target;
        this.Method = method;
        this.Entry = entry;
        this.Name = name;
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
