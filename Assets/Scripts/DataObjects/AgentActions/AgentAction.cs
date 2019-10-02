using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentAction", menuName = "DataObjects/AgentActions/AgentAction", order = 2)]
public class AgentAction : ScriptableObject
{
    public Sprite Icon;

    [TextArea(6, 10)]
    public string Description;

    public List<Trait> RequiredTraits = new List<Trait>();

    public int MinimumAge = 0;

    public BonusChallenge Challenge;

    public AgentAction FailureResult;

    public bool ShowHover = true;

    public LetterPreset employerLetterPreset;

    public virtual void Execute(Character requester, Character character, AgentInteractable target)
    {
        string reason;
        if (!CanDoAction(requester, character, target, out reason))
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

        if (character.CurrentTaskEntity != null && character.CurrentTaskEntity.CurrentTask.Cancelable)
        {
            character.CurrentTaskEntity.Cancel();
        }

        if (ShowHover && character.CurrentFaction == CORE.PC.CurrentFaction)
        {
            CORE.Instance.ShowHoverMessage(this.name, null, target.transform);
        }

        if(employerLetterPreset != null)
        {
            if (character.Employer != null && character.TopEmployer != null && character.Employer != character.TopEmployer)
            {
                Dictionary<string, object> letterParameters = new Dictionary<string, object>();

                letterParameters.Add("Target_Name", character.name);
                letterParameters.Add("Target_Role", character.CurrentRole);
                letterParameters.Add("Letter_From", character.Employer);
                letterParameters.Add("Letter_To", character.TopEmployer);
                letterParameters.Add("Letter_SubjectCharacter", character);

                LetterDispenserEntity.Instance.DispenseLetter(new Letter(employerLetterPreset, letterParameters));
            }
        }
    }

    public virtual bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        if(character.Age < MinimumAge)
        {
            reason = "Too young to do so.";
            return false;
        }

        if(character.CurrentTaskEntity != null && !character.CurrentTaskEntity.CurrentTask.Cancelable)
        {
            reason = "";
            return false;
        }

        reason = "";
        return true;
    }

    public virtual bool RollSucceed(Character character)
    {
        if (this.Challenge == null || this.Challenge.Type == null)
        {
            return true;
        }

        float characterSkill = character.GetBonus(this.Challenge.Type).Value;
        float result = Random.Range(0f, characterSkill + Challenge.ChallengeValue + Challenge.RarityValue);


        bool finalResult = !Challenge.InvertedChance ? (characterSkill >= result) : (characterSkill < result); ;

        return finalResult;
    }
}
