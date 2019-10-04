using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttemptBefriendingPerson", menuName = "DataObjects/AgentActions/AttemptBefriendingPerson", order = 2)]
public class AttemptBefriendingPerson : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        string reason;
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

        if(Task == null)
        {
            return;
        }

        if (target.GetType() == typeof(LocationEntity))
        {
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target);
        }
        else if (target.GetType() == typeof(PortraitUI))
        {
            Character targetChar = ((PortraitUI)target).CurrentCharacter;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetChar.CurrentLocation, targetChar);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        if (targetChar == character)
        {
            return false;
        }

        if (targetChar.TopEmployer == CORE.PC)
        {
            return false;
        }

        if (!targetChar.IsKnown("CurrentLocation"))
        {
            reason = "You don't know where this person is...";
            return false;
        }

        int relations = targetChar.GetRelationsWith(character);
        float targetDiscreetValue = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;

        if (relations < targetDiscreetValue)
        {

            reason =
                targetChar.name + " doesn't like " + character.name + " enough... ("
                + relations + " \\ " + targetDiscreetValue + ")";
            return false;
        }


        return true;
    }
}
