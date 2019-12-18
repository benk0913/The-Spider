using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CreateQuestAgentAction", menuName = "DataObjects/AgentActions/Spying/CreateQuestAgentAction", order = 2)]
public class CreateQuestAgentAction : AgentAction //DO NOT INHERIT FROM
{
    public List<PossibleOpportunity> PossibleQuestLetters = new List<PossibleOpportunity>();

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        if (character.TopEmployer == CORE.PC)
        {
            PossibleQuestLetters.RemoveAll(x => !ValidateCondition(x));

            if(PossibleQuestLetters.Count == 0)
            {
                return;
            }

            base.Execute(requester, character, target);

            LetterPreset QuestLetterPreset = PossibleQuestLetters[Random.Range(0, PossibleQuestLetters.Count)].Letter.CreateClone();

            Dictionary<string, object> letterParameters = new Dictionary<string, object>();

            letterParameters.Add("Letter_From", character);
            letterParameters.Add("Letter_To", character.TopEmployer);

            LetterDispenserEntity.Instance.DispenseLetter(new Letter(QuestLetterPreset, letterParameters));
        }
        //AI / BOT - Alternative for letter.... (maybe quest / gain bonus / whatever)
    }

    bool ValidateCondition(PossibleOpportunity opportunity)
    {
        if(opportunity.HostilePropertyCondition != null)
        {
            if (CORE.Instance.Locations.Find(
                x => x.CurrentProperty == opportunity.HostilePropertyCondition 
            && ((x.OwnerCharacter != null && x.OwnerCharacter.TopEmployer != CORE.PC) || x.OwnerCharacter == null)) 
            
            == null)
            {
                return false;
            }
        }

        return true;
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }

    [System.Serializable]
    public class PossibleOpportunity
    {
        public LetterPreset Letter;
        public Property HostilePropertyCondition;
    }
}
