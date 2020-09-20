using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ForageEmployeeReleaseDocuments", menuName = "DataObjects/AgentActions/ForageEmployeeReleaseDocuments", order = 2)]
public class ForageEmployeeReleaseDocuments : AgentAction
{
    [SerializeField]
    GameObject PortraitEffectOnReleasedTarget;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        SelectCharacterViewUI.Instance.Show(
            (selected) =>
            {
                selected.ExitPrison();
                selected.StopDoingCurrentTask(true);
                
                Trait wantedTrait = selected.Traits.Find(x => x.name == "Wanted");

                if(wantedTrait != null)
                {
                    selected.RemoveTrait(wantedTrait);
                }

                selected.GoToLocation(selected.HomeLocation);

                CORE.Instance.ShowPortraitEffect(PortraitEffectOnReleasedTarget, selected, selected.CurrentLocation);

                CORE.PC.Heat--;
            },
            (x) =>
            {
                return x.TopEmployer == CORE.PC && x.PrisonLocation != null
                && x.PrisonLocation.Traits.Contains(CORE.Instance.Database.LawAreaTrait);
            }, 
            "Select an employee to release from prison.",null,this);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != CORE.Instance.Database.GOD && character.TopEmployer != requester && requester != character)
        {
            return false;
        }

        return true;
    }
}
