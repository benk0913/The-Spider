using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionPortraitUI : AgentInteractable, IPointerClickHandler
{
    [SerializeField]
    Image Icon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    List<PlayerAction> PossiblePlayerActions = new List<PlayerAction>();

    public LongTermTaskEntity CurrentEntity;

    public void SetAction(LongTermTaskEntity entity)
    {
        CurrentEntity = entity;

        Icon.sprite = entity.CurrentTask.Icon;

        TooltipTarget.Text = "<size=20><u>" + entity.CurrentTask.name + "</u></size> \n " + entity.CurrentTask.Description;
    }



    public override List<PlayerAction> GetPossiblePlayerActions()
    {
        return PossiblePlayerActions;
    }

    public void OnRightClick()
    {
        ShowActionMenu();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ShowActionMenu();
        }
    }
}
