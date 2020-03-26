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

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("ShowMap", Show);

        CORE.Instance.SubscribeToEvent("HideMap", Hide);
    }

    private void OnDestroy()
    {
        if (CORE.Instance == null)
        {
            return;
        }

        CORE.Instance.UnsubscribeFromEvent("ShowMap", Show);
        CORE.Instance.UnsubscribeFromEvent("HideMap", Hide);
    }

    public void Show()
    {
        if (!MapViewManager.Instance.MapElementsContainer.gameObject.activeInHierarchy)
        {
            return;
        }

        this.gameObject.SetActive(true);

        Refresh();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void SetInfo(LongTermTaskEntity entity)
    {

        CurrentEntity = entity;

        Show();
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
