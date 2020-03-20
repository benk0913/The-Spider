using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecruitEmployeePlayer", menuName = "DataObjects/PlayerActions/RecruitEmployeePlayer", order = 2)]
public class RecruitEmployeePlayer : PlayerAction
{
    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot recruit for this property.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;

        SelectAgentWindowUI.Instance.Show(
             (Character character) => 
             {
                 character.StartWorkingFor(location);
                 requester.CGold -= 100;
             }
             , (Character charInQuestion) => 
             {
                 return charInQuestion != requester
                        && charInQuestion.IsKnown("Name", requester)
                        && charInQuestion.WorkLocation == null
                        && charInQuestion.PropertiesOwned.Count == 0
                        && charInQuestion.Age > location.CurrentProperty.MinAge
                        && charInQuestion.Age < location.CurrentProperty.MaxAge
                        && !location.FiredEmployeees.Contains(charInQuestion)
                        && (location.CurrentProperty.RecruitingGenderType == -1 ?
                        true
                        :
                        charInQuestion.Gender == (GenderType)location.CurrentProperty.RecruitingGenderType);
             });
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        LocationEntity location = (LocationEntity)target;

        reason = null;

        if (location.OwnerCharacter == null || location.OwnerCharacter != requester)
        {
            return false;
        }

        if(location.EmployeesCharacters.Count >= location.CurrentProperty.PropertyLevels[location.Level-1].MaxEmployees)
        {
            reason = new FailReason("No empty slot for a new employee.");
            return false;
        }

        if(requester.CGold < 100)
        {
            reason = new FailReason("Not enough gold! (" +requester.CGold+"/100)",100);
            return false;
        }

        return true;
    }
}
