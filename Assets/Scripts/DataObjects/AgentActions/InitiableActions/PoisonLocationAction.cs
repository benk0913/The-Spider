using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonLocationAction", menuName = "DataObjects/AgentActions/PoisonLocationAction", order = 2)]
public class PoisonLocationAction : AgentAction
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if(!RollSucceed(character))
        {
            if(FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }


        LocationEntity targetEntity = (LocationEntity)target;

        List<Character> charactersToKill = new List<Character>();
        foreach (Character targetChar in targetEntity.EmployeesCharacters)
        {
            if (Random.Range(0, 2) == 0)
            {
                charactersToKill.Add(targetChar);
            }
        }

        if (Random.Range(0, 3) == 0)
        {
            charactersToKill.Add(targetEntity.OwnerCharacter);
        }

        while (charactersToKill.Count > 0)
        {
            charactersToKill[0].StopDoingCurrentTask(true);
            CORE.Instance.Database.GetAgentAction("Death").Execute(CORE.Instance.Database.GOD, charactersToKill[0], target);
            charactersToKill.RemoveAt(0);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        LocationEntity targetEntity = (LocationEntity)target;

        if(!targetEntity.Known.GetKnowledgeInstance("Existance").IsKnownByCharacter(character.TopEmployer))
        {
            //reason = new FailReason("This location is not known to you.");

            return false;
        }
        
        if(targetEntity.OwnerCharacter == null)
        {
            return false;
        }

        if (targetEntity.OwnerCharacter.TopEmployer == character.TopEmployer)
        {
            return false;
        }

        if(character.Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")) || character.Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))
        {
            reason = new FailReason(character.name+" refuses. This act is too evil");
            return false;
        }

        float discreet = character.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
        if (discreet < 3)
        {
            reason = new FailReason(character.name + " is not 'Discreet' enough (atleast 3)");
            return false;
        }

        float stealthy = character.GetBonus(CORE.Instance.Database.GetBonusType("Stealthy")).Value;
        if (stealthy < 3)
        {
            reason = new FailReason(character.name + " is not 'Stealthy' enough (atleast 3)");
            return false;
        }

        if (InventoryPanelUI.Instance.GetItem("Poison") == null)
        {
            reason = new FailReason("You are required to have 'Poison' in your possession.");
            return false;
        }

        return true;
    }
}
