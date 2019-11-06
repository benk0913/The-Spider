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
        CurrentLocation.RecruitEmployee(CORE.PC);
    }


}
