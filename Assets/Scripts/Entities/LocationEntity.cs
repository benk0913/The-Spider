﻿using System.Collections;
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
    GameObject IdleStateObject;

    public int Level = 1;

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
        SelectedMarkerObject = ResourcesLoader.Instance.GetRecycledObject(DEF.LOCATION_MARKER_PREFAB);
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

    public void SetInfo(Property property)
    {
        CurrentProperty = property;
        CurrentAction = CurrentProperty.Actions[0];
        RefreshState();
    }

    public void RefreshState()
    {
        if (FigurePoint.transform.childCount > 0)
        {
            FigurePoint.transform.GetChild(0).gameObject.SetActive(false);
            FigurePoint.transform.GetChild(0).SetParent(transform);
        }

        GameObject tempFigure = ResourcesLoader.Instance.GetRecycledObject(CurrentProperty.FigurePrefab);
        tempFigure.transform.SetParent(FigurePoint);
        tempFigure.transform.position = FigurePoint.position;
        tempFigure.transform.rotation = FigurePoint.rotation;


        //TODO Replace getchilds with script
        if (OwnerCharacter == null)
        {
            tempFigure.transform.GetChild(0).GetComponent<MeshRenderer>().material = CORE.Instance.Database.DefaultFaction.WaxMaterial;
        }
        else
        {
            tempFigure.transform.GetChild(0).GetComponent<MeshRenderer>().material = OwnerCharacter.CurrentFaction.WaxMaterial;
        }


        StateUpdated.Invoke();
    }

    public void PurchaseUpgrade()
    {
        if (Level >= CurrentProperty.PropertyLevels.Count)
        {
            return;
        }

        if (OwnerCharacter.TopEmployer.Gold < CurrentProperty.PropertyLevels[Level].UpgradePrice)
        {
            GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                "(You need more " + (CurrentProperty.PropertyLevels[Level].UpgradePrice - OwnerCharacter.TopEmployer.Gold)+")", 1f, Color.red);
            //TODO "NO MONEY ALERT"
            return;
        }

        OwnerCharacter.TopEmployer.Gold -= CurrentProperty.PropertyLevels[Level].UpgradePrice;
        IsUpgrading = true;
        CurrentUpgradeLength = CurrentProperty.PropertyLevels[Level].UpgradeLength;
        StateUpdated.Invoke();
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

            HoverPanelUI hoverPanel = ResourcesLoader.Instance.GetRecycledObject(DEF.HOVER_PANEL_PREFAB).GetComponent<HoverPanelUI>();
            hoverPanel.transform.SetParent(CORE.Instance.MainCanvas.transform);
            hoverPanel.Show(Camera.main.WorldToScreenPoint(transform.position), "Upgrade Complete", ResourcesLoader.Instance.GetSprite("thumb-up"));
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
        CurrentAction = action;

        StateUpdated.Invoke();
    }

    public void JobActionComplete()
    {
        int totalRevenue = 0;
        for (int i = 0; i < EmployeesCharacters.Count; i++)
        {
            int sumEarned = Random.Range(CurrentAction.GoldGeneratedMin, CurrentAction.GoldGeneratedMax);

            totalRevenue += sumEarned;
            EmployeesCharacters[i].Gold += sumEarned;
        }

        HoverPanelUI hoverPanel = ResourcesLoader.Instance.GetRecycledObject(DEF.HOVER_PANEL_PREFAB).GetComponent<HoverPanelUI>();
        hoverPanel.transform.SetParent(CORE.Instance.MainCanvas.transform);
        hoverPanel.Show(Camera.main.WorldToScreenPoint(transform.position), string.Format("{0:n0}",totalRevenue.ToString()), ResourcesLoader.Instance.GetSprite("receive_money"));

        StateUpdated.Invoke();
    }

    public void Rebrand(Property newProperty)
    {
        SelectedPanelUI.Instance.Deselect();

        CancelUpgrade();
        StopRecruiting();
        Level = 1;

        while(EmployeesCharacters.Count > 0)
        {
            EmployeesCharacters[0].StopWorkingFor(this);
        }

        SetInfo(newProperty);

        SelectedPanelUI.Instance.Select(this);
    }

}
