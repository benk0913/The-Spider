using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAGainTrait", menuName = "DataObjects/Dialog/Actions/DDAGainTrait", order = 2)]
public class DDAGainTrait : DialogDecisionAction
{
    [SerializeField]
    Trait TraitToGain;

    public override void Activate()
    {
        Character actor = (Character) DialogWindowUI.Instance.GetDialogParameter("Actor");
        actor.AddTrait(TraitToGain);

        object obj = DialogWindowUI.Instance.GetDialogParameter("GainedTraits");

        if (obj != null)
        {
            string gainedTraits = (string)DialogWindowUI.Instance.GetDialogParameter("GainedTraits");
            DialogWindowUI.Instance.SetDialogParameter("GainedTraits", gainedTraits + " - " + TraitToGain.name);
        }
        else
        {
            DialogWindowUI.Instance.SetDialogParameter("GainedTraits",TraitToGain.name);
        }
    }
}
