using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RumorContentUI : MonoBehaviour
{
    public Rumor CurrentRumor;

    [SerializeField]
    TextMeshProUGUI ContentText;

    [SerializeField]
    Image RumorIcon;

    [SerializeField]
    PortraitUI RelevantCharacterPortrait;

    [SerializeField]
    Sprite DefaultIcon;

    public void SetInfo(Rumor rumor)
    {
        CurrentRumor = rumor;

        ContentText.text = CurrentRumor.Description;

        if (CurrentRumor.Icon != null)
        {
            RumorIcon.sprite = CurrentRumor.Icon;
        }
        else
        {
            RumorIcon.sprite = DefaultIcon;
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
    }
}
