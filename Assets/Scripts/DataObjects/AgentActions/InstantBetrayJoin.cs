using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InstantBetrayJoin", menuName = "DataObjects/AgentActions/InstantBetrayJoin", order = 2)]
public class InstantBetrayJoin : AgentAction //DO NOT INHERIT FROM
{

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

        if (character.TopEmployer == character || character.TopEmployer == null)
        {
            GlobalMessagePrompterUI.Instance.Show(character.name + " is not stupid. (Will not betray self)", 3f, Color.red);
            return;
        }

        List<LocationEntity> possibleProperties = CORE.PC.PropertiesInCommand;
        possibleProperties.RemoveAll(
            x => !x.CurrentProperty.EmployeesAreAgents || x.EmployeesCharacters.Count >= x.CurrentProperty.PropertyLevels[x.Level - 1].MaxEmployees);

        SelectLocationViewUI.Instance.Show((x) =>
        {
            if(character.TopEmployer != null && character.CurrentFaction != null)
            {
                Faction foundFaciton = CORE.Instance.Factions.Find(cf => cf.name == character.CurrentFaction.name);

                if(foundFaciton == null)
                {
                    Debug.LogError("NO FACTION " + character.CurrentFaction.name);
                    foundFaciton = character.CurrentFaction;
                }

                foundFaciton.Relations.GetRelations(CORE.Instance.Factions.Find(f => f.name == CORE.PC.CurrentFaction.name))
                .TotalValue -= 10;
            }

            character.BetrayEmployer();
            CORE.Instance.DelayedInvokation(0.5f, ()=> 
            {
                character.StartWorkingFor(x);



            });
        }, x => possibleProperties.Contains(x), "Where the agent should join?",null,()=> { RefundAction(CORE.PC); });
         
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetChar = ((PortraitUI)target).CurrentCharacter;

            if (targetChar != null)
            {
                if (targetChar.TopEmployer == targetChar)
                {
                    reason = new FailReason(targetChar.name + " is not stupid. (Will not betray self)");
                    return false;
                }
            }
        }

        LocationEntity targetProperty = CORE.PC.PropertiesInCommand.Find(
            x => x.CurrentProperty.EmployeesAreAgents && x.EmployeesCharacters.Count < x.CurrentProperty.PropertyLevels[x.Level - 1].MaxEmployees);

        if (targetProperty == null)
        {
            reason = new FailReason("No Available Property To Accomadate " + character.name);
            return false;
        }

        return true;
    }
}
