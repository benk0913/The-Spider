using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SpyAroundComplete", menuName = "DataObjects/AgentActions/Spying/SpyAroundComplete", order = 2)]
public class SpyAroundComplete : AgentAction 
{
    [SerializeField]
    LongTermTask Task;

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

        if (target.GetType() == typeof(LocationEntity))
        {
            LocationEntity location = (LocationEntity)target;

            location.Known.Know("Existance");

            List<Character> possibleTargets = new List<Character>();
            possibleTargets = location.CharactersInLocation.FindAll(
                (Character charInQuestion) =>
                {
                    return charInQuestion.isImportant || location.EmployeesCharacters.Contains(charInQuestion);
                });

            bool foundSomething = false;
            foreach (Character charInLocation in possibleTargets)
            {
                float enemyValue = charInLocation.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
                float agentValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value;

                if (charInLocation.IsKnown("Appearance")
                    && charInLocation.IsKnown("CurrentLocation")
                    && charInLocation.IsKnown("WorkLocation"))
                {
                    continue;
                }

                if (Random.Range(0, Mathf.RoundToInt(enemyValue + agentValue)) > agentValue) // if lost in stat duel
                {
                    continue;
                }
                
                charInLocation.Known.Know("Appearance");
                charInLocation.Known.Know("CurrentLocation");

                if (charInLocation.WorkLocation == location)
                {
                    charInLocation.Known.Know("WorkLocation");
                }

                foundSomething = true;
                break;
            }

            if (!foundSomething)
            {
                CORE.Instance.ShowHoverMessage(
                    "Found nothing of interest...",
                    ResourcesLoader.Instance.GetSprite("Unsatisfied"),
                    character.CurrentLocation.transform);
            }
            else
            {
                CORE.Instance.ShowHoverMessage(
                     "Found interesting information!",
                     ResourcesLoader.Instance.GetSprite("Satisfied"),
                     character.CurrentLocation.transform);
            }

            if (Task == null)
            {
                return;
            }

            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}