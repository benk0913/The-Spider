using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAClose", menuName = "DataObjects/Dialog/Actions/DDAClose", order = 2)]
public class DDAClose : DialogDecisionAction
{
    [SerializeField]
    public AgentAction UniqueAction;

    public override void Activate()
    {
        string endResult = "<u>The Dialog Has Ended</u>";

        Character actor = (Character)DialogWindowUI.Instance.GetDialogParameter("Actor");

        if (UniqueAction != null)
        {
            LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");

            UniqueAction.Execute(CORE.Instance.Database.GOD, actor, location);

            endResult += "\n With Result: " + UniqueAction.name;

        }

        //if (actor.Belogings.Count > 0)
        //{
        //    endResult += "\n Items Looted: ";

        //    foreach (Item item in actor.Belogings)
        //    {
        //        endResult += item.name + " - ";
        //    }

        //    actor.Belogings.Clear();
        //}

        //object objGainedTraits = DialogWindowUI.Instance.GetDialogParameter("GainedTraits");
        //if (objGainedTraits != null)
        //{
        //    endResult += "\n Gained Traits: ";
        //    endResult += "\n "+((string) objGainedTraits);
        //}

        WarningWindowUI.Instance.Show(endResult, EndDialog);
    }

    void EndDialog()
    {
        object wasPoisioned = DialogWindowUI.Instance.GetDialogParameter("PoisonedLocation");

        if(wasPoisioned != null && (string) wasPoisioned == "True")
        {
            PoisonedLocation();
        }

        DialogWindowUI.Instance.HideCurrentDialog();
    }

    void PoisonedLocation()
    {
        LocationEntity currentLocation = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");

        List<Character> charactersToKill = new List<Character>();
        foreach (Character character in currentLocation.CharactersInLocation)
        {
            if(Random.Range(0,3) == 0)
            {
                continue;
            }

            charactersToKill.Add(character);
        }

        //TODO Add rumor

        while(charactersToKill.Count > 0)
        {
            CORE.Instance.Database.GetAgentAction("Death").Execute(CORE.Instance.Database.GOD, charactersToKill[0], charactersToKill[0].CurrentLocation);
            charactersToKill.RemoveAt(0);
        }
    }
}
