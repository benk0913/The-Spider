using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CreateQuestAgentAction", menuName = "DataObjects/AgentActions/Spying/CreateQuestAgentAction", order = 2)]
public class CreateQuestAgentAction : AgentAction //DO NOT INHERIT FROM
{
    public List<LetterPreset> PossibleQuestLetters = new List<LetterPreset>();

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if (character.TopEmployer == CORE.PC)
        {
            LetterPreset QuestLetterPreset = PossibleQuestLetters[Random.Range(0, PossibleQuestLetters.Count)].CreateClone();

            Dictionary<string, object> letterParameters = new Dictionary<string, object>();

            letterParameters.Add("Letter_From", character);
            letterParameters.Add("Letter_To", character.TopEmployer);

            LetterDispenserEntity.Instance.DispenseLetter(new Letter(QuestLetterPreset, letterParameters));
        }
        //AI / BOT - Alternative for letter.... (maybe quest / gain bonus / whatever)
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
