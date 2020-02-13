using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShowFavorMenu", menuName = "DataObjects/AgentActions/ShowFavorMenu", order = 2)]
public class ShowFavorMenu : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        BribeFavorWindowUI.Instance.Show(targetChar);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        

        return true;
    }
}
