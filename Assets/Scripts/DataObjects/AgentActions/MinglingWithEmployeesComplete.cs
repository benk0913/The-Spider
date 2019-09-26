using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MinglingWithEmployeesComplete", menuName = "DataObjects/AgentActions/Spying/MinglingWithEmployeesComplete", order = 2)]
public class MinglingWithEmployeesComplete : AgentAction 
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if (!CanDoAction(requester, character, target))
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

        List<Character> possibleTargets = new List<Character>();
        possibleTargets.InsertRange(0, location.CharactersInLocation);

        foreach(Character charInLocation in possibleTargets)
        {
            float enemyValue = charInLocation.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
            float agentValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value;

            if (Random.Range(0, Mathf.RoundToInt(enemyValue+agentValue)) > agentValue) // if lost in stat duel
            {
                continue;
            }

            if (!charInLocation.IsKnown("Name"))
            {
                charInLocation.Known.Know("Name");
            }
            if (!charInLocation.IsKnown("Appearance"))
            {
                charInLocation.Known.Know("Appearance");
            }
            if (!charInLocation.IsKnown("CurrentLocation"))
            {
                charInLocation.Known.Know("CurrentLocation");
            }
        }

        foreach (Character charInLocation in possibleTargets)
        {

            float enemyValue = charInLocation.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value + 5; // EXTRA DEFFICULTY
            float agentValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value;

            if (Random.Range(0, Mathf.RoundToInt(enemyValue + agentValue)) > agentValue) // if lost in stat duel
            {
                continue;
            }

            if (!charInLocation.IsKnown("Personality"))
            {
                charInLocation.Known.Know("Personality");
            }
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {

        if (!base.CanDoAction(requester, character, target))
        {
            return false;
        }

        return true;
    }
}