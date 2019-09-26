using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LookAroundComplete", menuName = "DataObjects/AgentActions/Spying/LookAroundComplete", order = 2)]
public class LookAroundComplete : AgentAction 
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
        possibleTargets.Remove(character);

        foreach (Character charInLocation in possibleTargets)
        {
            float enemyValue = charInLocation.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
            float agentValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value;

            if (Random.Range(0, Mathf.RoundToInt(enemyValue+agentValue)) > agentValue) // if lost in stat duel
            {
                continue;
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
            float enemyValue = charInLocation.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value + 5; //BONUS DEFFICULTY
            float agentValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value;

            if (Random.Range(0, Mathf.RoundToInt(enemyValue + agentValue)) > agentValue) // if lost in stat duel
            {
                continue;
            }

            if (!charInLocation.IsKnown("Name"))
            {
                charInLocation.Known.Know("Name");
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