using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LongTermTaskExecuter", menuName = "DataObjects/AgentActions/LongTermTaskExecuter", order = 2)]
public class LongTermTaskExecuter : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;
    public bool RandomLocation;
    public PropertyTrait LocationTrait;
    public bool ActionDoneByTarget = false;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        if(ActionDoneByTarget)
        {
            if(target.GetType() == typeof(PortraitUI))
            {
                Character targetCharacter = ((PortraitUI)target).CurrentCharacter;

                if(targetCharacter == null || targetCharacter.CurrentTaskEntity != null)
                {
                    GlobalMessagePrompterUI.Instance.Show("The target is currently unavailable for this task.", 1f, Color.red);
                    return;
                }

                character = targetCharacter;
            }
            else if (target.GetType() == typeof(LocationEntity))
            {
                Character targetCharacter = ((LocationEntity)target).EmployeesCharacters.Find(x => x.CurrentTaskEntity == null);

                if(targetCharacter == null)
                {
                    GlobalMessagePrompterUI.Instance.Show("Couldn't find an available employee for the job.", 1f, Color.red);
                    return;
                }

                character = targetCharacter;

            }
        }

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
                else if (target.GetType() == typeof(PortraitUI))
                {
                    character.GoToLocation(((PortraitUI)target).CurrentCharacter.CurrentLocation);
                }
            }
        }

        if (target.GetType() == typeof(LocationEntity))
        {
            target = character.CurrentLocation;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target);
        }
        else if (target.GetType() == typeof(PortraitUI))
        {
            Character targetChar = ((PortraitUI)target).CurrentCharacter;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetChar.CurrentLocation, targetChar);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        return true;
    }
}
