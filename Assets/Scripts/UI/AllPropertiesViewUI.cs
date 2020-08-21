using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPropertiesViewUI : SelectLocationViewUI
{
    public static AllPropertiesViewUI Instance;
    public GridType CurrentType;


    protected override void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void ChangeType(int toType)
    {
        CurrentType = (GridType)toType;
        Refresh();
    }

    void Refresh()
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if (CORE.Instance.TurnPassedRoutineInstance != null)
        {
            return;
        }

        switch (CurrentType)
        {
            case GridType.All:
                {
                    base.Show((LocationEntity x) => { SelectedPanelUI.Instance.Select(x); }, (LocationEntity locationInQuestion) =>
                    {
                        return locationInQuestion != null
                        && locationInQuestion.Known != null
                        && locationInQuestion.Known.GetIsAnythingKnown(CORE.PC)
                        && !locationInQuestion.IsDisabled;

                    });
                    break;
                }
            case GridType.Agents:
                {
                    base.Show((LocationEntity x)=> { SelectedPanelUI.Instance.Select(x); }, (LocationEntity locationInQuestion) =>
                    {
                        return locationInQuestion != null 
                        && locationInQuestion.Known != null 
                        && locationInQuestion.Known.GetIsAnythingKnown(CORE.PC)
                        && !locationInQuestion.IsDisabled
                        && locationInQuestion.OwnerCharacter != null
                        && locationInQuestion.OwnerCharacter.TopEmployer == CORE.PC;
                    });
                    break;
                }
            case GridType.HasSomethingToSell:
                {
                    base.Show((LocationEntity x) => { SelectedPanelUI.Instance.Select(x); }, (LocationEntity locationInQuestion) =>
                    {
                        return locationInQuestion != null
                        && locationInQuestion.Known != null
                        && locationInQuestion.Known.GetIsAnythingKnown(CORE.PC)
                        && !locationInQuestion.IsDisabled
                        && locationInQuestion.Inventory.Count > 0;
                    });
                    break;
                }
        }
    }

    public void Show()
    {
        Show(null, null);
    }

    public override void Show(Action<LocationEntity> onSelect = null, Predicate<LocationEntity> filter = null, string title = "Select Location:", LocationEntity topLocation = null)
    {
        this.gameObject.SetActive(true);
        CORE.Instance.SubscribeToEvent("PassTimeComplete", Refresh);
        CORE.Instance.SubscribeToEvent("OnLocationChanged", Refresh);

        Refresh();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", Refresh);
        CORE.Instance.SubscribeToEvent("OnLocationChanged", Refresh);
    }

    [System.Serializable]
    public enum GridType
    {
        All,Agents,Important,HasSomethingToSell
    }
}
