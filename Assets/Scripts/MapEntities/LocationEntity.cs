using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocationEntity : AgentInteractable
{
    public const float PORTRAITS_SPACING = 5f;
    public const int PORTRAITS_MAX_IN_ROW = 5;

    [SerializeField]
    public Property CurrentProperty;

    [SerializeField]
    public Character OwnerCharacter;

    [SerializeField]
    public List<Character> EmployeesCharacters = new List<Character>();

    [SerializeField]
    Transform FigurePoint;

    [SerializeField]
    Transform HoverPoint;

    [SerializeField]
    GameObject IdleStateObject;

    [SerializeField]
    Button UpgradeButton;

    [SerializeField]
    Button RebrandButton;

    public int Level = 1;

    public float RevneueMultiplier;
    public float RiskMultiplier;

    public bool IsUpgrading;

    public int CurrentUpgradeLength;

    public bool isRecruiting;

    public int CurrentRecruitmentLength;

    public bool IsVisible = false;

    public UnityEvent StateUpdated;

    public Property.PropertyAction CurrentAction;

    GameObject SelectedMarkerObject;

    [SerializeField]
    List<AgentAction> PossibleAgentActions = new List<AgentAction>();

    [SerializeField]
    List<PlayerAction> PossiblePlayerActions = new List<PlayerAction>();

    public bool IsOwnedByPlayer
    {
        get
        {
            return OwnerCharacter != null && (OwnerCharacter == CORE.PC || OwnerCharacter.TopEmployer == CORE.PC);
        }
    }

    public override List<AgentAction> GetPossibleAgentActions(Character forCharacter)
    {
        return PossibleAgentActions;
    }

    public override List<PlayerAction> GetPossiblePlayerActions()
    {
        return PossiblePlayerActions;
    }

    public void OnRightClick()
    {
        if (OwnerCharacter != null && OwnerCharacter.TopEmployer == CORE.PC)
        {
            ShowActionMenu();
        }
    }


    private void Start()
    {
        CORE.Instance.Locations.Add(this);

        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);
        GameClock.Instance.OnDayPassed.AddListener(DayPassed);

        if(CurrentProperty != null)
        {
            SetInfo(CurrentProperty);

            if (OwnerCharacter != null)
            {
                if (OwnerCharacter == CORE.Instance.Database.PlayerCharacter)
                {
                    CORE.PC.StartOwningLocation(this);
                }
            }

            List<Character> charactersToAdd = new List<Character>();
            while(EmployeesCharacters.Count > 0)
            {
                Character tempChar = CORE.Instance.GetCharacter(EmployeesCharacters[0].name);

                EmployeesCharacters.RemoveAt(0);

                if (tempChar == null)
                {
                    continue;
                }

                charactersToAdd.Add(tempChar);
            }
            foreach(Character character in charactersToAdd)
            {
                character.StartWorkingFor(this);
            }


        }
    }

    void TurnPassed()
    {
        ProgressUpgrade();

        ProgressRecruiting();
 
        RefreshState();
    }

    void DayPassed()
    {
        JobActionComplete();
    }

    public void OnClick()
    {
        SelectedPanelUI.Instance.Select(this);
    }

    public void SetSelected()
    {
        SelectedMarkerObject = ResourcesLoader.Instance.GetRecycledObject("LocationMarker");
        SelectedMarkerObject.transform.SetParent(transform);
        SelectedMarkerObject.transform.position = transform.position;

        if (IdleStateObject != null)
        {
            IdleStateObject.gameObject.SetActive(true);
        }
    }

    public void SetDeselected()
    {

        SelectedMarkerObject.gameObject.SetActive(false);
        SelectedMarkerObject = null;

        if (IdleStateObject != null)
        {
            IdleStateObject.gameObject.SetActive(false);
        }
    }

    public void SetInfo(Property property, float revenueMultiplier = 1f, float riskMultiplier = 1f)
    {
        this.RevneueMultiplier = revenueMultiplier;
        this.RiskMultiplier = RiskMultiplier;

        CurrentProperty = property;
        CurrentAction = CurrentProperty.Actions[0];
        RefreshState();
    }

    public void RefreshState()
    {
        //TODO Better clearing solution then GetChilds
        while(FigurePoint.transform.childCount > 0)
        {
            FigurePoint.transform.GetChild(0).gameObject.SetActive(false);
            FigurePoint.transform.GetChild(0).SetParent(transform);
        }

        for(int i=0;i<HoverPoint.transform.childCount;i++)
        {
            Destroy(HoverPoint.transform.GetChild(i).gameObject);
        }

        GameObject tempFigure = ResourcesLoader.Instance.GetRecycledObject(CurrentProperty.FigurePrefab);
        tempFigure.transform.SetParent(FigurePoint);
        tempFigure.transform.position = FigurePoint.position;
        tempFigure.transform.rotation = FigurePoint.rotation;

        GameObject hoverModel = Instantiate(CurrentProperty.HoverPrefab);
        hoverModel.transform.SetParent(HoverPoint);
        hoverModel.transform.position = HoverPoint.position;
        hoverModel.transform.rotation = HoverPoint.rotation;


        if (CurrentProperty.MaterialOverride != null)
        {
            tempFigure.GetComponent<FigureController>().SetMaterial(CurrentProperty.MaterialOverride);
        }
        else
        {
            if (OwnerCharacter == null)
            {
                tempFigure.GetComponent<FigureController>().SetMaterial(CORE.Instance.Database.DefaultFaction.WaxMaterial);
            }
            else
            {
                tempFigure.GetComponent<FigureController>().SetMaterial(OwnerCharacter.CurrentFaction.WaxMaterial);
            }
        }


        StateUpdated.Invoke();
    }

    public void PurchaseUpgrade()
    {
        PurchaseUpgrade(OwnerCharacter.TopEmployer);
    }

    public void PurchaseUpgrade(Character funder)
    { 
        if(!IsOwnedByPlayer)
        {
            GlobalMessagePrompterUI.Instance.Show("YOU DON'T OWN THIS PLACE!", 1f, Color.red);
            return;
        }

        if (Level >= CurrentProperty.PropertyLevels.Count)
        {
            return;
        }

        if (funder.Gold < CurrentProperty.PropertyLevels[Level].UpgradePrice)
        {
            GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                "(You need more " + (CurrentProperty.PropertyLevels[Level].UpgradePrice - funder.Gold)+")", 1f, Color.red);

            return;
        }

        funder.Gold -= CurrentProperty.PropertyLevels[Level].UpgradePrice;
        IsUpgrading = true;
        CurrentUpgradeLength = CurrentProperty.PropertyLevels[Level].UpgradeLength;
        StateUpdated.Invoke();


        OwnerCharacter.DynamicRelationsModifiers.Add
        (
        new DynamicRelationsModifier(
        new RelationsModifier("Upgraded my property!", 2)
        , 10
        , funder)
        );
    }

    public void ProgressUpgrade()
    {
        if (!IsUpgrading)
        {
            return;
        }
        
        CurrentUpgradeLength--;

        if (CurrentUpgradeLength <= 0)
        {
            IsUpgrading = false;
            Level++;

            CORE.Instance.ShowHoverMessage("Upgrade Complete", ResourcesLoader.Instance.GetSprite("thumb-up"), transform);
        }
    }

    public void CancelUpgrade()
    {
        if (!IsUpgrading)
        {
            return;
        }

        OwnerCharacter.TopEmployer.Gold += CurrentProperty.PropertyLevels[Level].UpgradePrice;
        IsUpgrading = false;
        StateUpdated.Invoke();
    }

    public void StartRecruiting()
    {
        if(isRecruiting)
        {
            return;
        }

        if (EmployeesCharacters.Count >= CurrentProperty.PropertyLevels[Level - 1].MaxEmployees)
        {
            return;
        }

        CurrentRecruitmentLength = CurrentProperty.PropertyLevels[Level - 1].RecruitmentLength;

        isRecruiting = true;
    }


    public void ProgressRecruiting()
    {
        if (!isRecruiting)
        {
            return;
        }

        CurrentRecruitmentLength--;

        if (CurrentRecruitmentLength <= 0)
        {
            CORE.Instance.GenerateCharacter(CurrentProperty.RecruitingGenderType, CurrentProperty.MinAge, CurrentProperty.MaxAge).StartWorkingFor(this);
            StopRecruiting();
        }
    }

    public void StopRecruiting()
    {
        isRecruiting = false;

        CurrentRecruitmentLength = 0;
    }

    public void SelectAction(Property.PropertyAction action)
    {
        if (!IsOwnedByPlayer)
        {
            GlobalMessagePrompterUI.Instance.Show("YOU DON'T OWN THIS PLACE!", 1f, Color.red);
            return;
        }

        CurrentAction = action;

        StateUpdated.Invoke();
    }

    public void JobActionComplete()
    {
        int totalRevenue = 0;
        for (int i = 0; i < EmployeesCharacters.Count; i++)
        {
            int sumEarned = 
                Mathf.CeilToInt(Random.Range(CurrentAction.GoldGeneratedMin, CurrentAction.GoldGeneratedMax)
                * this.RevneueMultiplier
                * CORE.Instance.Database.Stats.GlobalRevenueMultiplier);

            //Employee Skills Bonus to revenue
            foreach (BonusChallenge bonusChallenge in CurrentAction.ActionBonusChallenges)
            {
                float multi = EmployeesCharacters[i].GetBonus(bonusChallenge.Type).Value / bonusChallenge.ChallengeValue;
                sumEarned = Mathf.CeilToInt(sumEarned * multi);
            }

            totalRevenue += sumEarned;
        }

        //Management Bonus
        if (OwnerCharacter != null)
        {
            totalRevenue = Mathf.RoundToInt(totalRevenue * OwnerCharacter.GetBonus(CurrentProperty.ManagementBonus).Value / OwnerCharacter.PropertiesOwned.Count);
        }

        //Share revenue with employees.
        foreach (Character employee in EmployeesCharacters)
        {
            employee.Gold += totalRevenue / EmployeesCharacters.Count;
        }

        //Show Revenue Message
        if (totalRevenue > 0)
        { 
            CORE.Instance.ShowHoverMessage(string.Format("{0:n0}", totalRevenue.ToString()), ResourcesLoader.Instance.GetSprite("receive_money"), transform);
        }

        StateUpdated.Invoke();
    }

    public void Rebrand(Property newProperty)
    {
        if (!IsOwnedByPlayer)
        {
            GlobalMessagePrompterUI.Instance.Show("YOU DON'T OWN THIS PLACE!", 1f, Color.red);
            return;
        }

        SelectedPanelUI.Instance.Deselect();

        CancelUpgrade();
        StopRecruiting();
        Level = 1;

        while(EmployeesCharacters.Count > 0)
        {
            EmployeesCharacters[0].StopWorkingFor(this);
        }

        SetInfo(newProperty, this.RevneueMultiplier, this.RiskMultiplier);

        SelectedPanelUI.Instance.Select(this);
    }

}
