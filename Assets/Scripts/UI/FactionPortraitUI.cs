using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FactionPortraitUI : MonoBehaviour, IPointerClickHandler
{

    public Faction CurrentFaction;

    [SerializeField]
    protected Image FactionIcon;

    [SerializeField]
    protected TooltipTargetUI TooltipTarget;

    [SerializeField]
    protected GameObject QuestionMark;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(CurrentFaction == null)
        {
            return;
        }

        FactionInfoUI.Instance.Show(CurrentFaction);
    }

    public void SetInfo(Faction faction)
    {
        this.CurrentFaction = faction;

        if (    CurrentFaction.name  == CORE.Instance.Database.DefaultFaction.name 
            || (CurrentFaction.Known != null && CurrentFaction.Known.IsKnown("Existance", CORE.PC)))
        {
            QuestionMark.gameObject.SetActive(false);

            FactionIcon.sprite = CurrentFaction.Icon;

            TooltipTarget.SetTooltip(CurrentFaction.name);
        }
        else
        {
            QuestionMark.gameObject.SetActive(true);

            FactionIcon.sprite = CurrentFaction.Icon;

            TooltipTarget.SetTooltip("Faction Unknown...");
        }
    }
}
