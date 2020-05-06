using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LayAmbush", menuName = "DataObjects/AgentActions/LayAmbush", order = 2)]
public class LayAmbush : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;
    public AgentAction ActionPerTurn;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if(Task == null)
        {
            return;
        }


        SelectCharacterViewUI.Instance.Show(
               (Character targetCharacter) => 
               {
                   CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target, targetCharacter, -1, ActionPerTurn, this);
               }
               , (Character charInQuestion) => 
               {
                   return
                    charInQuestion.Known.GetIsAnythingKnown(requester)
                    && charInQuestion.TopEmployer != requester
                    && charInQuestion != requester
                    && !charInQuestion.IsDisabled;
               }
               ,"Select Ambush Target",null,this);
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
        if (targetEntity.VisibilityState == LocationEntity.VisibilityStateEnum.QuestionMark || targetEntity.VisibilityState == LocationEntity.VisibilityStateEnum.Hidden)
        {
            //reason = new FailReason("This location is not known to you.");

            return false;
        }

        if(targetEntity.OwnerCharacter == null)
        {
            reason = new FailReason("This location is not owned by you.");
            return false;
        }

        if (targetEntity.OwnerCharacter.TopEmployer != requester)
        {
            reason = new FailReason("This location is not owned by you.");
            return false;
        }

        if (character.Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")) || character.Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))
        {
            reason = new FailReason(character.name + " refuses. This act is too evil");
            return false;
        }
        return true;
    }
}
