using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongTermTaskEffectUI : MonoBehaviour
{
    [SerializeField]
    WorldPositionLerperUI Lerper;

    LongTermTaskEntity CurrentEntity;

    public bool DisplayOnHidden = false;
    public bool DisplayOnQuestionMark = false;
    public bool DisplayOnOtherProperties = false;

    public void SetInfo(LongTermTaskEntity entity)
    {
        CurrentEntity = entity;

        Refresh();
    }

    public void Refresh()
    {
        if (CurrentEntity == null
            || !CurrentEntity.gameObject.activeInHierarchy
            || (!DisplayOnHidden && CurrentEntity.CurrentTargetLocation.VisibilityState == LocationEntity.VisibilityStateEnum.Hidden)
            || (!DisplayOnQuestionMark && CurrentEntity.CurrentTargetLocation.VisibilityState == LocationEntity.VisibilityStateEnum.QuestionMark)
            || (!DisplayOnOtherProperties && (CurrentEntity.CurrentTargetLocation.OwnerCharacter == null || (CurrentEntity.CurrentTargetLocation.OwnerCharacter.TopEmployer != CORE.PC))))
        {
            this.gameObject.SetActive(false);
            return;
        }

        Lerper.SetTransform(CurrentEntity.transform);
    }
}
