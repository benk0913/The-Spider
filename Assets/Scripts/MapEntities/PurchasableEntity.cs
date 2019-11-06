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
    List<AgentAction> PossibleActions = new List<AgentAction>();

    [SerializeField]
    List<PlayerAction> PossiblePlayerActions = new List<PlayerAction>();

    public bool Available = true;

    [SerializeField]
    bool PresetPlot;

    [SerializeField]
    GameObject Content;


    void Awake()
    {
        if (PresetPlot)
        {
            CORE.Instance.PurchasablePlots.Add(this);
        }
    }

    void Start()
    {
        CORE.Instance.SubscribeToEvent("ShowPurchasablePlots", SetUnHidden);
        CORE.Instance.SubscribeToEvent("HidePurchasablePlots", SetHidden);
    }

    public void SetInfo(PlotType type)
    {
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

    public FailReason PurchasePlot(Character funder, Character forCharacter)
    {
        if (funder.Gold < Price)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                    "(You need more " + (Price - funder.Gold) + ")", 1f, Color.red);
            }

            return new FailReason("Not Enough Gold");
        }

        if (!Available)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("LOCATION UNAVAILABLE ", 1f, Color.red);
            }

            return new FailReason("Location Unavailable");
        }

        LocationEntity location = CORE.Instance.GenerateNewLocation(transform.position, transform.rotation);

        location.SetInfo(Util.GenerateUniqueID(), Type.BaseProperty, true);
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

        return null;
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

    public void SetHidden()
    {
        Content.SetActive(false);
    }

    public void SetUnHidden()
    {
        Content.SetActive(true);
    }
}
