using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AmbushAssault", menuName = "DataObjects/AgentActions/Agression/AmbushAssault", order = 2)]
public class AmbushAssault : AgentAction //DO NOT INHERIT FROM
{
    [SerializeField]
    PopupDataPreset WinPopup;


    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        targetChar.CurrentFaction.Relations.GetRelations(character.CurrentFaction).TotalValue -= 1;

        float charSTR = character.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;
        float targetSTR = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;

        if(Random.Range(0f, (charSTR+targetSTR)) < charSTR)
        {
            if (character.TopEmployer == CORE.PC)
            {
                TurnReportUI.Instance.Log.Add(
                new TurnReportLogItemInstance(
                    character.name + " has assaulted " + targetChar.name,
                    ResourcesLoader.Instance.GetSprite("Satisfied"),
                    targetChar));

                PopupWindowUI.Instance.AddPopup(new PopupData(WinPopup, new List<Character> { character }, new List<Character> { targetChar }, () => 
                {
                    targetChar.StopDoingCurrentTask(false);
                    targetChar.Known.Forget("CurrentLocation", character.TopEmployer);
                    CORE.Instance.Database.GetEventAction("Wounded").Execute(targetChar.TopEmployer, targetChar, character.HomeLocation);

                }));
            }
            else
            {
                targetChar.StopDoingCurrentTask(false);
                targetChar.Known.Forget("CurrentLocation", character.TopEmployer);
                CORE.Instance.Database.GetEventAction("Wounded").Execute(targetChar.TopEmployer, targetChar, character.HomeLocation);
            }
        }
        else
        {
            CORE.Instance.ShowPortraitEffect(CORE.Instance.Database.FailWorldEffectPrefab, character, targetChar.CurrentLocation);
            CORE.Instance.Database.GetEventAction("Wounded").Execute(character.TopEmployer, character, character.HomeLocation);
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
