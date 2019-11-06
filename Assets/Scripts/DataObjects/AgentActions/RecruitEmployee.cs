using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RecruitEmployee", menuName = "DataObjects/AgentActions/RecruitEmployee", order = 2)]
public class RecruitEmployee : AgentAction
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

        if(location.EmployeesCharacters.Count >= location.CurrentProperty.PropertyLevels[location.Level - 1].MaxEmployees)
        {
            GlobalMessagePrompterUI.Instance.Show(location.CurrentProperty.name + " has no more space for another recruit!", 1f, Color.red);
            return;
        }

        LocationEntity closestLocation = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.PublicAreaTrait, location);

        Character randomNewEmployee = CORE.Instance.GenerateCharacter(
            location.CurrentProperty.RecruitingGenderType,
            location.CurrentProperty.MinAge,
            location.CurrentProperty.MaxAge);

        
        if(randomNewEmployee == null)
        {
            return;
        }

        randomNewEmployee.StartWorkingFor(location);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != CORE.Instance.Database.GOD && character.TopEmployer != requester && requester != character)
        {
            return false;
        }

        return true;
    }
}
