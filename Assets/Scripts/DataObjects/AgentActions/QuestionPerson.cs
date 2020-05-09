using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QuestionPerson", menuName = "DataObjects/AgentActions/Constabulary/QuestionPerson", order = 2)]
public class QuestionPerson : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        QuestioningWindowUI.Instance.Show(character, targetChar);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (targetChar.CurrentQuestioningInstance == null)
        {
            reason = new FailReason("You have nothing to ask...");
            return false;
        }

        if (!targetChar.IsKnown("HomeLocation", character.TopEmployer))
        {
            reason = new FailReason("You don't know where this person lives!");
            return false;
        }

        if (!targetChar.IsKnown("Appearance", character.TopEmployer))
        {
            reason = new FailReason("You don't know how this person looks like...");
            return false;
        }

        if (!targetChar.IsKnown("Name", character.TopEmployer))
        {
            reason = new FailReason("You don't the name of this person...");
            return false;
        }

        return true;
    }
}
