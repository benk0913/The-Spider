using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PortraitUIEmployee : PortraitUI
{

    [SerializeField]
    Color NegativeColor;

    [SerializeField]
    GameObject RecruitButton;

    LocationEntity CurrentLocation;

    void Start()
    {
        base.Start();
    }

    public void SetCharacter(Character character, LocationEntity RelevantLocation = null, bool emptySlot = false)
    {
        CurrentLocation = RelevantLocation;

        RecruitButton.gameObject.SetActive(emptySlot && CurrentLocation.OwnerCharacter != null && CurrentLocation.OwnerCharacter.TopEmployer == CORE.PC);

        base.SetCharacter(character);

        if (character != null && !character.IsKnown("WorkLocation"))
        {

            Face.color = Color.black;
            Hair.color = Color.black;
            Clothing.color = Color.black;
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;

            if (ActionPortrait != null)
            {
                ActionPortrait.gameObject.SetActive(false);
            }

            TooltipTarget.SetTooltip("This character is unknown");

            QuestionMark.gameObject.SetActive(true);

            return;
        }
    }

    public void RecruitEmployee()
    {
        if(CORE.PC.Connections < 3) //TODO replace magic number...
        {
            GlobalMessagePrompterUI.Instance.Show("Need more connections ("+CORE.PC.Connections + "/" + 3.ToString(),1f,Color.red);
            return;
        }

        CORE.PC.Connections -= 3;

        Character randomNewEmployee = CORE.Instance.GenerateCharacter(
               CurrentLocation.CurrentProperty.RecruitingGenderType,
               CurrentLocation.CurrentProperty.MinAge,
               CurrentLocation.CurrentProperty.MaxAge);

        randomNewEmployee.Known.KnowAllBasic();
        randomNewEmployee.StartWorkingFor(CurrentLocation);
    }


}
