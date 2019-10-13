using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PurchasableEntity : AgentInteractable, ISaveFileCompatible
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

    [SerializeField]
    List<PlayerAction> PossiblePlayerActions = new List<PlayerAction>();

    public bool Available = true;

    [SerializeField]
    bool PresetPlot;

    void Awake()
    {
        if (PresetPlot)
        {
            CORE.Instance.PurchasablePlots.Add(this);
        }
    }

    public void SetInfo(float revenueMulti, float riskMulti, PlotType type)
    {
        this.RevenueMultiplier = revenueMulti;
        this.RiskMultiplier = riskMulti;
        this.Type = type;
    }

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

    public override List<PlayerAction> GetPossiblePlayerActions()
    {
        return PossiblePlayerActions;
    }

    public void PurchasePlot(Character funder, Character forCharacter)
    {
        if (funder.Gold < Price)
        {
            GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                "(You need more " + (Price - funder.Gold) + ")", 1f, Color.red);

            return;
        }


        LocationEntity location = CORE.Instance.GenerateNewLocation(transform.position, transform.rotation);

        location.SetInfo(Util.GenerateUniqueID(), CORE.Instance.Database.EmptyProperty, RevenueMultiplier, RiskMultiplier, true);
        forCharacter.StartOwningLocation(location);

        CORE.Instance.Locations.Add(location);

        funder.Gold -= Price;

        if (funder.CurrentFaction == CORE.PC.CurrentFaction)
        {
            CORE.Instance.ShowHoverMessage(string.Format("{0:n0}", Price.ToString()), ResourcesLoader.Instance.GetSprite("pay_money"), transform);
        }



        forCharacter.DynamicRelationsModifiers.Add
        (
        new DynamicRelationsModifier(
        new RelationsModifier("Purchased a property for me!", 5)
        , 10
        , funder)
        );

        SetUnavailable();
    }

    public void SetAvailable()
    {
        Available = true;
        this.gameObject.SetActive(true);
    }

    public void SetUnavailable()
    {
        Available = false;
        this.gameObject.SetActive(false);
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();
        
        node["Available"] = Available.ToString();

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        if(bool.Parse(node["Available"]))
        {
            SetAvailable();
        }
        else
        {
            SetUnavailable();
        }
    }

    public void ImplementIDs()
    {
    }
}
