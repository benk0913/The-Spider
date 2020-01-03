using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LureToLocation", menuName = "DataObjects/AgentActions/LureToLocation", order = 2)]
public class LureToLocation : AgentAction //DO NOT INHERIT FROM
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


        SelectCharacterViewUI.Instance.Show(
               (Character targetCharacter) =>
               {
                   if (targetCharacter.CurrentTaskEntity != null)
                   {
                       targetCharacter.StopDoingCurrentTask();
                   }

                   LocationEntity targetLocation = (LocationEntity)target;
                   character.GoToLocation(targetLocation);
                   targetCharacter.GoToLocation(targetLocation);
                   targetCharacter.Known.Know("CurrentLocation", requester);
                   CORE.Instance.Database.GetEventAction("Hang Out").Execute(targetCharacter.TopEmployer, targetCharacter, targetLocation);
                   CORE.Instance.Database.GetEventAction("Hang Out").Execute(character.TopEmployer, character, targetLocation);
               }
               , (Character charInQuestion) =>
               {
                   return
                    charInQuestion.Known.GetIsAnythingKnown(requester)
                    && charInQuestion.TopEmployer != requester
                    && charInQuestion != requester
                    && charInQuestion.GetRelationsWith(character) > 6
                    && charInQuestion.CurrentTaskEntity == null 
                    || (charInQuestion.CurrentTaskEntity != null && charInQuestion.CurrentTaskEntity.CurrentTask.Cancelable);
               }
               , "Select Who To Invite (Relations above 6)");

    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        LocationEntity targetEntity = (LocationEntity)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }
        if (!targetEntity.Known.GetKnowledgeInstance("Existance").IsKnownByCharacter(character.TopEmployer))
        {
            //reason = new FailReason("This location is not known to you.");

            return false;
        }

        return true;
    }
}
