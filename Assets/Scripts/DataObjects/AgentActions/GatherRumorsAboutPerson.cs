using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GatherRumorsAboutPerson", menuName = "DataObjects/AgentActions/Spying/GatherRumorsAboutPerson", order = 2)]
public class GatherRumorsAboutPerson : AgentAction //DO NOT INHERIT FROM
{

    [SerializeField]
    GameObject WinEffectPrefab;


    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if(target.GetType() != typeof(PortraitUI) && target.GetType() != typeof(PortraitUIEmployee))
        {
            Debug.LogError("TASK ONLY FOR TARGHET CHARS " + target.name);
            return;
        }

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        float awareValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value;
        float targetDiscreetValue = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;

        CORE.Instance.GainInformation(character.CurrentLocation.transform, targetChar);

        CORE.Instance.ShowPortraitEffect(WinEffectPrefab, character, character.CurrentLocation);

        GameObject tempObj = CORE.Instance.SplineAnimationObject(
                                   "PortraitCollection",
                                    character.CurrentLocation.transform,
                                    InformationLogUI.Instance.transform,
                                    () => { AudioControl.Instance.PlayInPosition("resource_rumors", character.CurrentLocation.transform.position); },
                                    true,
                                    true);

        if (targetChar.IsKnown("Appearance", CORE.PC))
        {
            tempObj.transform.GetComponentInChildren<PortraitUI>().SetCharacter(targetChar);
        }
        else
        {
            tempObj.transform.GetComponentInChildren<PortraitUI>().SetCharacter(null);
        }


        if (Random.Range(0, awareValue + targetDiscreetValue) <= awareValue)
        {
            CORE.Instance.GainInformation(character.CurrentLocation.transform, targetChar);

            CORE.Instance.ShowHoverMessage("Bonus X1 Information - Aware VS Discreet", ResourcesLoader.Instance.GetSprite("scroll-unfurled"), character.CurrentLocation.transform);

            CORE.Instance.DelayedInvokation(0.5f, () =>
            {
                tempObj = CORE.Instance.SplineAnimationObject(
                                   "PortraitCollection",
                                    character.CurrentLocation.transform,
                                     InformationLogUI.Instance.transform,
                                    () => { AudioControl.Instance.PlayInPosition("resource_rumors", character.CurrentLocation.transform.position); },
                                    true,
                                    true);

                if (targetChar.IsKnown("Appearance", CORE.PC))
                {
                    tempObj.transform.GetComponentInChildren<PortraitUI>().SetCharacter(targetChar);
                }
                else
                {
                    tempObj.transform.GetComponentInChildren<PortraitUI>().SetCharacter(null);
                }
            });
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if(target.GetType() != typeof(PortraitUI) && target.GetType() != typeof(PortraitUIEmployee))
        {
            return false;
        }

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        if(targetChar == character)
        {
            return false;
        }

        if (targetChar.TopEmployer == character.TopEmployer)
        {
            return false;
        }

        return true;
    }
}
