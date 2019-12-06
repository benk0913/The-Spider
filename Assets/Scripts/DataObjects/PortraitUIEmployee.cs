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

    [SerializeField]
    TextMeshProUGUI RecruitPrice;

    LocationEntity CurrentLocation;

    public bool IsGuard = false;
    void Start()
    {
        base.Start();
    }

    public void SetCharacter(Character character, LocationEntity RelevantLocation = null, bool emptySlot = false, bool isGuard = false)
    {
        CurrentLocation = RelevantLocation;

        this.IsGuard = isGuard;

        if (emptySlot && CurrentLocation.OwnerCharacter != null && CurrentLocation.OwnerCharacter.TopEmployer == CORE.PC)
        {
            RecruitButton.gameObject.SetActive(true);

            int recruitmentCost = CORE.Instance.Database.BaseRecruitmentCost;
            recruitmentCost += CORE.Instance.Database.GetReputationType(CurrentLocation.OwnerCharacter.TopEmployer.Reputation).RecruitExtraCost;
            RecruitPrice.text = recruitmentCost.ToString();
        }
        else
        {
            RecruitButton.gameObject.SetActive(false);
        }

        base.SetCharacter(character);

        if (character != null && !character.IsKnown("WorkLocation", character.TopEmployer))
        {

            Face.color = Color.black;
            Hair.color = Color.black;
            Clothing.color = Color.black;
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
            FrameBG.color = CORE.Instance.Database.DefaultFaction.FactionColor;

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
        CurrentLocation.RecruitEmployee(CORE.PC, IsGuard);
    }


}
