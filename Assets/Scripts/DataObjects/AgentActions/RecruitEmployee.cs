﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RecruitEmployee", menuName = "DataObjects/AgentActions/RecruitEmployee", order = 2)]
public class RecruitEmployee : AgentAction
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if (!CanDoAction(requester, character, target))
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

        if(location.EmployeesCharacters.Count >= location.CurrentProperty.PropertyLevels[location.Level - 1].MaxEmployees)
        {
            GlobalMessagePrompterUI.Instance.Show(location.CurrentProperty.name + " has no more space for another recruit!", 1f, Color.red);
            return;
        }

        LocationEntity closestLocation = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.PublicAreaTrait, location);

        Character randomNewEmployee =
            CORE.Instance.Characters.Find(delegate (Character charInQuestion) 
            {
                return
                charInQuestion != CORE.PC
                && charInQuestion.WorkLocation == null
                && charInQuestion.PropertiesOwned.Count == 0
                && charInQuestion.Age > location.CurrentProperty.MinAge
                && charInQuestion.Age < location.CurrentProperty.MaxAge
                && !location.FiredEmployeees.Contains(charInQuestion)
                && (location.CurrentProperty.RecruitingGenderType == - 1? 
                        true 
                        :
                        charInQuestion.Gender == (GenderType)location.CurrentProperty.RecruitingGenderType);
            });

        if(randomNewEmployee == null)
        {
            return;
        }

        randomNewEmployee.StartWorkingFor(location);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {
        if (!base.CanDoAction(requester, character, target))
        {
            return false;
        }

        if (requester != CORE.Instance.Database.GOD && character.TopEmployer != requester && requester != character)
        {
            return false;
        }

        return true;
    }
}
