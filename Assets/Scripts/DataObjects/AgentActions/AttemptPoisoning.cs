using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttemptPoisoning", menuName = "DataObjects/AgentActions/AttemptPoisoning", order = 2)]
public class AttemptPoisoning : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;
    public bool RandomLocation;
    public PropertyTrait LocationTrait;

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

        if(Task == null)
        {
            return;
        }

        if (LocationTrait != null)
        {
            if(RandomLocation || character.CurrentLocation == null)
            {
                character.GoToLocation(CORE.Instance.GetRandomLocationWithTrait(LocationTrait));
            }
            else
            {
                character.GoToLocation(CORE.Instance.GetClosestLocationWithTrait(LocationTrait, character.CurrentLocation));
            }
        }
        else
        {
            if (RandomLocation)
            {
                character.GoToLocation(CORE.Instance.GetRandomLocation());
            }
            else
            {
                if (target.GetType() == typeof(LocationEntity))
                {
                    character.GoToLocation((LocationEntity)target);
                }
                else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
                {
                    character.GoToLocation(((PortraitUI)target).CurrentCharacter.CurrentLocation);
                }
            }
        }

        if (target.GetType() == typeof(LocationEntity))
        {
            target = character.CurrentLocation;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target, null, -1, null, this);
        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetChar = ((PortraitUI)target).CurrentCharacter;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetChar.CurrentLocation, targetChar, -1, null, this);
        }

        requester.Belogings.Remove(requester.GetItem("Poison"));
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

        if (targetEntity.OwnerCharacter == null)
        {
            return false;
        }

        if (targetEntity.OwnerCharacter.TopEmployer == character.TopEmployer)
        {
            return false;
        }

        if (character.TopEmployer != character)
        {
            if (character.Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")) || character.Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))
            {
                reason = new FailReason(character.name + " refuses. This act is too evil");
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
        }

        if (requester.GetItem("Poison") == null)
        {
            reason = new FailReason("You are required to have 'Poison' in your possession.");
            return false;
        }

        return true;
    }
}
