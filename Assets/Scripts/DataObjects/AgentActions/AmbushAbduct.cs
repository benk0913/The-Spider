using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AmbushAbduct", menuName = "DataObjects/AgentActions/Agression/AmbushAbduct", order = 2)]
public class AmbushAbduct : AgentAction //DO NOT INHERIT FROM
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
                    List<LocationEntity> locations = character.TopEmployer.PropertiesOwned;
                    LocationEntity location = locations.Find(x => x.HasFreePrisonCell);

                    if (location == null)
                    {
                        GlobalMessagePrompterUI.Instance.Show("No Free Prison Cells", 1f, Color.red);
                    }
                    else
                    {
                        targetChar.StopDoingCurrentTask(false);
                        CORE.Instance.Database.GetAgentAction("Get Abducted").Execute(CORE.Instance.Database.GOD, targetChar, location);
                    }

                }));
            }
            else
            {
                List<LocationEntity> locations = character.TopEmployer.PropertiesOwned;
                LocationEntity location = locations.Find(x => x.HasFreePrisonCell);

                if (location != null)
                { 
                    targetChar.StopDoingCurrentTask(false);
                    CORE.Instance.Database.GetAgentAction("Get Abducted").Execute(CORE.Instance.Database.GOD, targetChar, location);
                }
                else
                {
                    Debug.LogError("Abduct attempt while no prison cells (AI)");
                }
            }
        }
        else
        {
            CORE.Instance.ShowPortraitEffect(CORE.Instance.Database.FailWorldEffectPrefab, character, targetChar.CurrentLocation);
            CORE.Instance.Database.GetAgentAction("Wounded").Execute(character.TopEmployer, character, character.HomeLocation);
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
