using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InstantArson", menuName = "DataObjects/AgentActions/InstantArson", order = 2)]
public class InstantArson : AgentAction //DO NOT INHERIT FROM
{

    public bool KillEveryoneInvolved = false;
    public bool SelectLocation = true;

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

        if (SelectLocation)
        {
            SelectLocationViewUI.Instance.Show((x) =>
            {
                Arson(x, character);
            }, x => propertiesInCommand.Contains(x) || character.WorkLocation == x);
        }
        else
        {
            if(target.GetType() == typeof(LocationEntity))
            {
                Arson((LocationEntity)target,character);
            }

            Debug.Log("CANT DESTROY LOCATION");
        }
    }

    protected virtual void Arson(LocationEntity location, Character character)
    {
        List<Character> charactersToKill = new List<Character>();

        if (KillEveryoneInvolved)
        {
            charactersToKill.AddRange(location.CharactersInLocation);
            charactersToKill.Remove(character);
        }

        location.BecomeRuins();

        charactersToKill.ForEach((x) => x.Death());
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        LocationEntity targetEntity = (LocationEntity)target;
        reason = null;

        return true;
    }
}
