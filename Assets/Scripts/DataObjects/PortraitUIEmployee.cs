using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PortraitUIEmployee : PortraitUI
{
    [SerializeField]
    TextMeshProUGUI ProductivityText;

    [SerializeField]
    Color PositiveColor;

    [SerializeField]
    Color NegativeColor;

    void Start()
    {
        base.Start();
    }

    public void SetCharacter(Character character, string bottomRightMessage, bool positiveState = true)
    {
        base.SetCharacter(character);

        ProductivityText.text = bottomRightMessage;
        ProductivityText.color = positiveState ? PositiveColor : NegativeColor;

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

            TooltipTarget.Text = "This character is unknown";

            QuestionMark.gameObject.SetActive(true);

            return;
        }
    }


}
