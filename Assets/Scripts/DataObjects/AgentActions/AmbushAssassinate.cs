using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AmbushAssassinate", menuName = "DataObjects/AgentActions/Agression/AmbushAssassinate", order = 2)]
public class AmbushAssassinate : AgentAction //DO NOT INHERIT FROM
{
    [SerializeField]
    PopupDataPreset WinPopup;


    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (targetChar.CurrentFaction.Relations != null)
        {
            targetChar.CurrentFaction.Relations.GetRelations(character.CurrentFaction).TotalValue -= 2;
        }

        if(targetChar.NeverDED)
        {
            return;
        }

        float charSTR = character.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;
        float targetSTR = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;

        if(Random.Range(0f, (charSTR+targetSTR)) < charSTR)
        {
            if (character.TopEmployer == CORE.PC)
            {
                TurnReportUI.Instance.Log.Add(
                new TurnReportLogItemInstance(
                    character.name + " has assasinated " + targetChar.name,
                    ResourcesLoader.Instance.GetSprite("Satisfied"),
                    targetChar));

                PopupWindowUI.Instance.AddPopup(new PopupData(WinPopup, new List<Character> { character }, new List<Character> { targetChar }, () => 
                {
                    targetChar.StopDoingCurrentTask(false);
                    CORE.Instance.Database.GetAgentAction("Death").Execute(targetChar.TopEmployer, targetChar, character.HomeLocation);

                }));
            }
            else
            {
                targetChar.StopDoingCurrentTask(false);
                CORE.Instance.Database.GetAgentAction("Death").Execute(targetChar.TopEmployer, targetChar, character.HomeLocation);
            }
        }
        else
        {
            CORE.Instance.ShowPortraitEffect(CORE.Instance.Database.FailWorldEffectPrefab, character, targetChar.CurrentLocation);
            CORE.Instance.Database.GetAgentAction("Wound").Execute(character.TopEmployer, character, character.HomeLocation);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if(targetChar.TopEmployer == character.TopEmployer)
        {
            return false;
        }
        
        if(targetChar.CurrentLocation != character.CurrentLocation)
        {
            return false;
        }


        return true;
    }
}
