using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scheme Type", menuName = "DataObjects/Plots/Scheme Type", order = 2)]
public class SchemeType : ScriptableObject
{
    [TextArea(3, 6)]
    public string Description;
    public Sprite Icon;


    public PlotMethod BruteMethod;

    public List<PlotEntry> PossibleEntries;
    public List<PlotMethod> PossibleMethods;

    public List<ScenarioPopup> EntryScenarios = new List<ScenarioPopup>();
    public List<ScenarioPopup> ExitScenarios = new List<ScenarioPopup>();

    public List<ScenarioPopup> AgentMethodFailureScenarios = new List<ScenarioPopup>();
    public List<ScenarioPopup> AgentMethodSuccessScenarios = new List<ScenarioPopup>();

    public ScenarioPopup DuelResultArrestScenario;
    public ScenarioPopup DuelResultWoundScenario;
    public ScenarioPopup DuelResultDeathScenario;

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

    public virtual FailReason Execute(
        Character requester, 
        Character plotter, 
        List<Character> participants,
        List<Character> targetParticipants,
        AgentInteractable target, 
        PlotMethod method, 
        PlotEntry entry)
    {

        WinResult(requester,plotter,participants,targetParticipants,target,method,entry);
        return null;
    }

    public virtual void Init(Character requester,
        Character plotter,
        List<Character> participants,
        List<Character> targetParticipants,
        AgentInteractable target,
        PlotMethod method,
        PlotEntry entry)
    {
        foreach (Character participant in participants)
        {
            foreach (Item item in method.ItemsRequired)
            {
                requester.Belogings.Remove(requester.GetItem(item.name));
            }
        }

        foreach (Item item in entry.ItemsRequired)
        {
            requester.Belogings.Remove(requester.GetItem(item.name));
        }


        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(entry, method, EntryScenarios), participants, targetParticipants));
    }

    public virtual FailReason Dueling(Character plotter,
        List<Character> participants,
        List<Character> entryParticipants,
        List<Character> entryTargets,
        List<Character> targetParticipants,
        AgentInteractable target,
        PlotMethod method,
        PlotEntry entry)
    {
        while (entryTargets.Count > 0 && entryParticipants.Count > 0)
        {
            Character randomParticipant = entryParticipants[Random.Range(0, entryParticipants.Count)];

            float offenseSkill = randomParticipant.GetBonus(method.OffenseSkill).Value;
            if (entry.Skill == method.OffenseSkill)
            {
                offenseSkill += entry.BonusToSkill;
            }

            float defenceSkill = entryTargets[0].GetBonus(method.DefenceSkill).Value;

            if (Random.Range(0, offenseSkill + defenceSkill) < offenseSkill) //Win
            {
                PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(entry, method, AgentMethodSuccessScenarios), participants, targetParticipants));

                if (method == BruteMethod)
                {
                    AggressiveDuelResult(randomParticipant, targetParticipants[0]);
                }

                entryTargets.RemoveAt(0);
            }
            else // Lose
            {
                PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(entry, method, AgentMethodFailureScenarios), participants, targetParticipants));

                if (method != BruteMethod)
                {
                    method = BruteMethod;
                }

                AggressiveDuelResult(targetParticipants[0], randomParticipant);

                entryParticipants.Remove(randomParticipant);
            }
        }


        if (entryParticipants.Count <= 0)
        {
            return new FailReason("Plan Failed");
        }

        return null;
    }

    public virtual void WinResult(
        Character requester,
        Character plotter,
        List<Character> participants,
        List<Character> targetParticipants,
        AgentInteractable target,
        PlotMethod method,
        PlotEntry entry)
    {
    }

    public virtual void AggressiveDuelResult(Character winner, Character loser)
    {
        winner.Known.Know("Appearance", loser.TopEmployer);
        loser.Known.Know("Appearance", winner.TopEmployer);

        float randomResult = Random.Range(0f, 1f);

        if (randomResult > 0.5f)
        {
            if (!(winner.Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")) ||
                winner.Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))) // Not a Good guy?
            {
                PopupWindowUI.Instance.AddPopup(
                    new PopupData(DuelResultArrestScenario.PopupData, new List<Character> { loser }, new List<Character> { winner }
                    ,() => { CORE.Instance.Database.GetEventAction("Get Arrested").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));

            }
            else
            {
                PopupWindowUI.Instance.AddPopup(new PopupData(DuelResultDeathScenario.PopupData, new List<Character> { loser }, new List<Character> { winner }
                , () => { CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));
                
            }
        }
        else
        {
            PopupWindowUI.Instance.AddPopup(new PopupData(DuelResultWoundScenario.PopupData, new List<Character> { loser }, new List<Character> { winner }
            , () => { CORE.Instance.Database.GetEventAction("Wounded").Execute(CORE.Instance.Database.GOD, loser, loser.CurrentLocation); }));
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
