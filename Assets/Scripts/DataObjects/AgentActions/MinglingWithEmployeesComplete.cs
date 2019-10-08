using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MinglingWithEmployeesComplete", menuName = "DataObjects/AgentActions/Spying/MinglingWithEmployeesComplete", order = 2)]
public class MinglingWithEmployeesComplete : AgentAction 
{
    [SerializeField]
    LongTermTask Task;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        string reason;
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

        foreach(Character charInLocation in possibleTargets)
        {
            float enemyValue = charInLocation.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
            float agentValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value;
            
            if (charInLocation.IsKnown("Name") 
                && charInLocation.IsKnown("Appearance") 
                && charInLocation.IsKnown("CurrentLocation") 
                && charInLocation.IsKnown("Personality"))
            {
                continue;
            }

            if (Random.Range(0, Mathf.RoundToInt(enemyValue+agentValue)) > agentValue) // if lost in stat duel
            {
                continue;
            }

            charInLocation.Known.Know("Name");
            charInLocation.Known.Know("Appearance");
            charInLocation.Known.Know("CurrentLocation");
            charInLocation.Known.Know("Personality");
            

            break;
        }

        if (Task == null)
        {
            return;
        }

        CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}