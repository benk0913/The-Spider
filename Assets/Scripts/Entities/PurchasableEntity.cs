﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableEntity : AgentInteractable
{
    [SerializeField]
    public int Price;

    [SerializeField]
    public PlotType Type;

    [SerializeField]
    public float RevenueMultiplier = 1f;

    [SerializeField]
    public float RiskMultiplier = 1f;

    [SerializeField]
    List<AgentAction> PossibleActions = new List<AgentAction>();

    public void OnClick()
    {
        PlotInfoPanelUI.Instance.Select(this);
    }
    
    public void OnRightClick()
    {
        if(ControlCharacterPanelUI.CurrentCharacter == null)
        {
            return;
        }

        List<AgentAction> currentActions = GetPossibleActions(ControlCharacterPanelUI.CurrentCharacter);
        List<DescribedAction> KeyActions = new List<DescribedAction>();

        foreach(AgentAction action in currentActions)
        {
            KeyActions.Add(
                new DescribedAction(
                    action.name,
                    () => action.Execute(ControlCharacterPanelUI.CurrentCharacter, this)
                    , action.Description));
        }

        RightClickDropDownPanelUI.Instance.Show(KeyActions, transform);
    }

    public override List<AgentAction> GetPossibleActions(Character forCharacter)
    {
        return PossibleActions;
    }

    public void PurchasePlot(Character forCharacter)
    {
        if (forCharacter.TopEmployer.Gold < Price)
        {
            GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                "(You need more " + (Price - forCharacter.TopEmployer.Gold) + ")", 1f, Color.red);
            //TODO NOT ENOUGH MONEY ALERT.
            return;
        }

        GameObject locationPrefab = ResourcesLoader.Instance.GetRecycledObject(DEF.LOCATION_PREFAB);

        locationPrefab.transform.position = transform.position;
        locationPrefab.transform.rotation = transform.rotation;


        LocationEntity location = locationPrefab.GetComponent<LocationEntity>();

        location.SetInfo(CORE.Instance.Database.EmptyProperty);
        location.OwnerCharacter = forCharacter;

        HoverPanelUI hoverPanel = ResourcesLoader.Instance.GetRecycledObject(DEF.HOVER_PANEL_PREFAB).GetComponent<HoverPanelUI>();
        hoverPanel.transform.SetParent(CORE.Instance.MainCanvas.transform);
        hoverPanel.Show(Camera.main.WorldToScreenPoint(transform.position), string.Format("{0:n0}", Price.ToString()), ResourcesLoader.Instance.GetSprite("pay_money"));


        Destroy(this.gameObject);
    }

    public void ShowTooltip()
    {

    }

    public void HideTooltip()
    {
    }
}
