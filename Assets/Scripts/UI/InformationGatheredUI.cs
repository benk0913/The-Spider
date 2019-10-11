using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationGatheredUI : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI Title;

    [SerializeField]
    protected PortraitUI CharacterPortrait;

    InformationLogUI.NewInformationInstance CurrentInstance;

    public virtual void SetInfo(InformationLogUI.NewInformationInstance instance)
    {
        CurrentInstance = instance;

        Title.text = "You have learned <color=green>"+instance.AboutCharacter.name+ "'s</color> <color=yellow>\n" + instance.Information+ "</color>";
        CharacterPortrait.SetCharacter(instance.AboutCharacter);
    }

}
