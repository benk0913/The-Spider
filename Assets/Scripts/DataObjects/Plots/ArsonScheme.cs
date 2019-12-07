using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArsonScheme", menuName = "DataObjects/Plots/ArsonScheme", order = 2)]
public class ArsonScheme : SchemeType
{
    [SerializeField]
    PopupDataPreset CharacterBurnt;

    public override FailReason Execute(
     Character requester,
     Character plotter,
     List<Character> participants,
     List<Character> targetParticipants,
     AgentInteractable target,
     PlotMethod method,
     PlotEntry entry)
    {

        List<Character> entryTargets = new List<Character>();
        entryTargets.AddRange(targetParticipants);

        List<Character> entryParticipants = new List<Character>();
        entryParticipants.AddRange(participants);

        base.Init(requester, plotter, participants, targetParticipants, target, method, entry);

        //Dueling
        FailReason failureReason = base.Dueling(plotter, participants, entryParticipants, entryTargets, targetParticipants, target, method, entry);

        if (failureReason != null)
        {
            return failureReason;
        }

        return base.Execute(requester, plotter, participants, targetParticipants, target, method, entry);
    }

    public override void WinResult(
        Character requester,
        Character plotter,
        List<Character> participants,
        List<Character> targetParticipants,
        AgentInteractable target,
        PlotMethod method,
        PlotEntry entry)
    {
        base.WinResult(requester, plotter, participants, targetParticipants, target, method, entry);

        LocationEntity targetLocation = (LocationEntity)target;

        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(entry, method, ExitScenarios), participants, targetParticipants,
            () => 
            {

                foreach (Character character in targetLocation.CharactersInLocation)
                {
                    if(character.TopEmployer != plotter)
                    {
                        if(Random.Range(0f,1f) < 0.5f) // more chance for location employees to burn.
                        {
                            PopupWindowUI.Instance.AddPopup(new PopupData(CharacterBurnt, new List<Character>() { character }, new List<Character>(){ plotter } ,
                            () =>
                            {
                                CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);
                            }));
                        }
                    }
                    else
                    {
                        if (Random.Range(0f, 1f) < 0.1f)
                        {
                            PopupWindowUI.Instance.AddPopup(new PopupData(CharacterBurnt, new List<Character>() { character }, new List<Character>() { plotter },
                            () =>
                            {

                                CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);
                            }));
                        }
                    }
                }

                targetLocation.Dispose();

            }));

    }
}
