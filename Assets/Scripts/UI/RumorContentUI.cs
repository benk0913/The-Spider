using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RumorContentUI : HeadlineContentUI
{
    public Rumor CurrentRumor;

    [SerializeField]
    PortraitUI RelevantCharacterPortrait;

    [SerializeField]
    LocationPortraitUI RelevantLocationPortrait;


    public void SetInfo(Rumor rumor)
    {
        CurrentRumor = rumor;

        ContentText.text = CurrentRumor.Description;

        if (CurrentRumor.Icon != null)
        {
            Icon.sprite = CurrentRumor.Icon;
        }
        else
        {
            Icon.sprite = DefaultIcon;
        }

        if(!string.IsNullOrEmpty(CurrentRumor.RelevantCharacterID))
        {
            RelevantCharacterPortrait.gameObject.SetActive(true);
            RelevantCharacterPortrait.SetCharacter(CORE.Instance.GetCharacterByID(CurrentRumor.RelevantCharacterID));
        }
        else
        {
            RelevantCharacterPortrait.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(CurrentRumor.RelevantLocationID))
        {
            RelevantLocationPortrait.gameObject.SetActive(true);
            RelevantLocationPortrait.SetLocation(CORE.Instance.GetLocationByID(CurrentRumor.RelevantLocationID));
        }
        else
        {
            RelevantLocationPortrait.gameObject.SetActive(false);
        }
    }
}
