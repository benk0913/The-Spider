using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InstantReleasePrisoner", menuName = "DataObjects/AgentActions/InstantReleasePrisoner", order = 2)]
public class InstantReleasePrisoner : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }
        
        if (FailureResult != null && !RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        List<LocationEntity> propertiesInCommand = character.PropertiesInCommand;
        SelectCharacterViewUI.Instance.Show((x) => 
        {
            CORE.Instance.ShowHoverMessage(x.name + " was released.", ResourcesLoader.Instance.GetSprite("Satisfied"), character.CurrentLocation.transform);
            x.ExitPrison();
        }
        , x=> x.PrisonLocation != null && (propertiesInCommand.Contains(x.PrisonLocation) || character.WorkLocation == x),"Select Agent:",null, this);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        LocationEntity targetEntity = (LocationEntity)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
