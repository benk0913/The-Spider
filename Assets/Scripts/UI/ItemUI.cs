using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemUI : AgentInteractable, IPointerClickHandler
{
    public Image Icon;
    public TooltipTargetUI TooltipTarget;

    public Item CurrentItem;

    public List<AgentAction> AgentActions = new List<AgentAction>();
    public List<PlayerAction> PlayerActions = new List<PlayerAction>();

    public LocationEntity LocationParent;



    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ShowActionMenu();
        }
    }

    public void SetInfo(Item item, LocationEntity locationParent = null)
    {
        this.LocationParent = locationParent;

        CurrentItem = item;

        RefreshUI();
    }

    void RefreshUI()
    {
        Icon.sprite = CurrentItem.Icon;
        TooltipTarget.SetTooltip(CurrentItem.Description);
    }

    public override List<AgentAction> GetPossibleAgentActions(Character forCharacter)
    {
        return AgentActions;
    }

    public override List<PlayerAction> GetPossiblePlayerActions()
    {
        return PlayerActions;
    }
}
