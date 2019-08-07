using System.Collections;
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
        ShowActionMenu();
    }

    public override List<AgentAction> GetPossibleAgentActions(Character forCharacter)
    {
        return PossibleActions;
    }

    public void PurchasePlot(Character forCharacter)
    {
        if (forCharacter.TopEmployer.Gold < Price)
        {
            GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                "(You need more " + (Price - forCharacter.TopEmployer.Gold) + ")", 1f, Color.red);

            return;
        }

        GameObject locationPrefab = ResourcesLoader.Instance.GetRecycledObject("Location");

        locationPrefab.transform.SetParent(MapViewManager.Instance.MapElementsContainer);
        locationPrefab.transform.position = transform.position;
        locationPrefab.transform.rotation = transform.rotation;


        LocationEntity location = locationPrefab.GetComponent<LocationEntity>();

        location.SetInfo(CORE.Instance.Database.EmptyProperty);
        forCharacter.StartOwningLocation(location);

        HoverPanelUI hoverPanel = ResourcesLoader.Instance.GetRecycledObject("HoverPanelUI").GetComponent<HoverPanelUI>();
        hoverPanel.transform.SetParent(CORE.Instance.MainCanvas.transform);
        hoverPanel.Show(transform, string.Format("{0:n0}", Price.ToString()), ResourcesLoader.Instance.GetSprite("pay_money"));


        Destroy(this.gameObject);
    }
}
