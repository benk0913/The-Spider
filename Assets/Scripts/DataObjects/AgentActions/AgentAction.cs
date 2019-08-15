using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentAction", menuName = "DataObjects/AgentActions/AgentAction", order = 2)]
public class AgentAction : ScriptableObject
{
    public Sprite Icon;

    [TextArea(2,3)]
    public string Description;

    public List<Trait> RequiredTraits = new List<Trait>();

    public int MinimumAge = 0;

    public BonusChallenge Challenge;

    public AgentAction FailureResult;

    public virtual void Execute(Character requester, Character character, AgentInteractable target)
    {
        if(!CanDoAction(requester, character, target))
        {
            GlobalMessagePrompterUI.Instance.Show("This character can not do this action! ", 1f, Color.red);

            return;
        }

        if(!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }
        }
    }

    public virtual bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {
        if(character.Age < MinimumAge)
        {
            return false;
        }

        return true;
    }

    public virtual bool RollSucceed(Character character)
    {
        if(this.Challenge == null || this.Challenge.Type == null)
        {
            return true;
        }

        float characterSkill = character.GetBonus(this.Challenge.Type).Value;
        float result = Random.Range(0f, characterSkill + Challenge.ChallengeValue);

        return (characterSkill >= result);
    }
}
