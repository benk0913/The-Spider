using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableEntity : AgentInteractable
{
    [SerializeField]
    public int Price;

    [SerializeField]
    public PurchasablePlotType Type;

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
        List<KeyActionPair> KeyActions = new List<KeyActionPair>();

        foreach(AgentAction action in currentActions)
        {
            KeyActions.Add(new KeyActionPair(action.name,
                ()=> ExecuteAgentAction(ControlCharacterPanelUI.CurrentCharacter, action)));
        }

        RightClickDropDownPanelUI.Instance.Show(KeyActions, transform);
    }

    public override List<AgentAction> GetPossibleActions(Character forCharacter)
    {
        return PossibleActions;
    }

    public override void ExecuteAgentAction(Character byCharacter, AgentAction action)
    {
        switch(action.name)
        {
            case "Buy Property":
                {
                    PurchasePlot(byCharacter);
                    break;
                }
        }
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

        locationPrefab.GetComponent<LocationEntity>().SetInfo(CORE.Instance.Database.EmptyProperty);

        Destroy(this.gameObject);
    }

    public void ShowTooltip()
    {

    }

    public void HideTooltip()
    {
    }
}
