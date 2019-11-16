using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LookAroundComplete", menuName = "DataObjects/AgentActions/Spying/LookAroundComplete", order = 2)]
public class LookAroundComplete : AgentAction 
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

        LocationEntity location = (LocationEntity)target;

        List<Character> possibleTargets = new List<Character>();
        possibleTargets.InsertRange(0, location.CharactersInLocation);
        possibleTargets.Remove(character);

        foreach (Character charInLocation in possibleTargets)
        {
            float enemyValue = charInLocation.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
            float agentValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value;

            if(charInLocation.IsKnown("Appearance", character.TopEmployer) && charInLocation.IsKnown("CurrentLocation", character.TopEmployer))
            {
                continue;
            }

            if (Random.Range(0, Mathf.RoundToInt(enemyValue+agentValue)) > agentValue) // if lost in stat duel
            {
                continue;
            }


            charInLocation.Known.Know("Appearance", character.TopEmployer);
            charInLocation.Known.Know("CurrentLocation", character.TopEmployer);
            

            enemyValue = charInLocation.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value + 5; //BONUS DEFFICULTY
            agentValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value;

            break;
        }


        if (Task == null)
        {
            return;
        }

        CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target);
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