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
    }


}
