using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArsonScheme", menuName = "DataObjects/Plots/ArsonScheme", order = 2)]
public class ArsonScheme : SchemeType
{
    [SerializeField]
    PopupDataPreset CharacterBurnt;

    public override void Execute(PlotData data)
    {
        base.Execute(data);
    }

    public override void WinResult(DuelResultData data)
    {
        LocationEntity targetLocation = (LocationEntity)data.Plot.Target;

        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(data.Plot.Entry, data.Plot.Method, ExitScenarios), data.Plot.Participants, data.Plot.TargetParticipants,
        () =>
        {
            base.WinResult(data);

            if(targetLocation.CurrentProperty.name == "Orphanage")
            {
                data.Plot.Plotter.Reputation -= 5;
                data.Plot.Plotter.TopEmployer.Reputation -= 5;

                if (data.Plot.Plotter.TopEmployer == CORE.PC)
                {
                    WarningWindowUI.Instance.Show("Burning an orphanage, such a violent and ruthless act has decreased your reputation in 5! ", () =>
                    {
                        CORE.Instance.SplineAnimationObject("BadReputationCollectedWorld",
                            targetLocation.transform,
                            StatsViewUI.Instance.transform,
                            null,
                            false);

                    });
                }
            }

            foreach (Character character in targetLocation.CharactersInLocation)
            {
                if(character.TopEmployer != data.Plot.Plotter)
                {
                    if(Random.Range(0f,1f) < 0.5f) // more chance for location employees to burn.
                    {
                        PopupWindowUI.Instance.AddPopup(new PopupData(CharacterBurnt, new List<Character>() { character }, new List<Character>(){ data.Plot.Plotter } ,
                        () =>
                        {
                            CORE.Instance.Database.GetAgentAction("Death").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);
                        }));
                    }
                }
                else
                {
                    if (Random.Range(0f, 1f) < 0.1f)
                    {
                        PopupWindowUI.Instance.AddPopup(new PopupData(CharacterBurnt, new List<Character>() { character }, new List<Character>() { data.Plot.Plotter },
                        () =>
                        {

                            CORE.Instance.Database.GetAgentAction("Death").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);
                        }));
                    }
                }
            }

            targetLocation.BecomeRuins();

        }));

    }
}
