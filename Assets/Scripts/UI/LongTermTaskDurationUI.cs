using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LongTermTaskDurationUI : MonoBehaviour
{
    LongTermTaskEntity CurrentEntity;

    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TextMeshProUGUI TurnsLeft;

    [SerializeField]
    Image Icon;
    
    [SerializeField]
    Image Filer;

    [SerializeField]
    WorldPositionLerperUI Lerper;

    [SerializeField]
    PortraitUI CharacterPortrait;


    public void SetInfo(LongTermTaskEntity entity)
    {
        CurrentEntity = entity;
        Icon.sprite = CurrentEntity.CurrentTask.Icon;
        Title.text = CurrentEntity.CurrentTask.name;
        Lerper.SetTransform(CurrentEntity.transform);

        Refresh();

        CharacterPortrait.SetCharacter(entity.CurrentCharacter);
    }

    public void Refresh()
    {
        TurnsLeft.text = CurrentEntity.TurnsLeft+"<size=10>-Turns...</size>";
        Filer.fillAmount = (float)CurrentEntity.TurnsLeft / CurrentEntity.CurrentTask.TurnsToComplete;
    }
}
