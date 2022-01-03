using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDADestroyProperty", menuName = "DataObjects/Dialog/Actions/DDADestroyProperty", order = 2)]
public class DDADestroyProperty : DialogDecisionAction
{
    public Property TheProperty;
    public Faction OfFaction;

    public override void Activate()
    {
        LocationEntity targetLocation = null;
        if (TheProperty != null && OfFaction != null)
        {
            targetLocation = CORE.Instance.Locations.Find(X => X.CurrentProperty == TheProperty && X.FactionInControl != null && X.FactionInControl.name == OfFaction.name);
        }
        else if(DialogWindowUI.Instance.GetDialogParameter("Location") != null)
        {
            targetLocation = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        }

        if (targetLocation == null)
        {
            Debug.LogError("NO RELEVANT LOCATION FOR " + this.name);
            return;
        }

        WarningWindowUI.Instance.Show("Would you like to pay for "+targetLocation.Name+"'s immidiate repairs? (40 Gold)", () =>
         {
             if (CORE.PC.CGold >= 40)
             {
                 CORE.PC.CGold -= 40;
             }
             else
             {
                 GlobalMessagePrompterUI.Instance.Show("Not enough gold...", 1f, Color.red);
                 targetLocation.BecomeRuins();
             }
         }, false, () => { targetLocation.BecomeRuins(); });
        
    }
}
