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

        Title.text = "You have learned "+instance.AboutCharacter.name+"'s \n"+ instance.Information;
        CharacterPortrait.SetCharacter(instance.AboutCharacter);
    }

}
