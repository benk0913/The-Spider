using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplaceOwner", menuName = "DataObjects/AgentActions/ReplaceOwner", order = 2)]
public class ReplaceOwner : AgentAction
{

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

        LocationEntity location = (LocationEntity)target;

        if(character.WorkLocation == location && character.Employer != requester)
        {
            WarningWindowUI.Instance.Show("Warning! Doing this will cut off the chain of command you have on this agent: \n <color=red> * this means it will no longer work for you. \n * All of it's properties will be out of your control. </color>", delegate 
            {
                DoTheReplacement(requester, character, location);
            });

            return;
        }

        DoTheReplacement(requester, character, location);
    }

    void DoTheReplacement(Character requester, Character character, LocationEntity location)
    {
        if (location == character.WorkLocation)
        {
            character.StopWorkingForCurrentLocation();
        }


        character.DynamicRelationsModifiers.Add
        (
        new DynamicRelationsModifier(
        new RelationsModifier("Preferred me over someone else!", 5)
        , 10
        , requester)
        );

        location.OwnerCharacter.DynamicRelationsModifiers.Add
        (
        new DynamicRelationsModifier(
        new RelationsModifier("Preferred someone else over me!", -7)
        , 10
        , requester)
        );

        character.StartOwningLocation(location);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        LocationEntity location = (LocationEntity)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (character.TopEmployer != requester)
        {
            return false;   
        }

        if(location.OwnerCharacter == requester)
        {
            return false;
        }

        if(location.OwnerCharacter != null && location.OwnerCharacter.TopEmployer != requester)
        {
            return false;
        }

        if(character.PropertiesOwned.Contains(location))
        {
            return false;
        }

        return true;
    }
}
