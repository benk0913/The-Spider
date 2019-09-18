﻿using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocationEntity : AgentInteractable, ISaveFileCompatible
{
    public string ID;

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

    [SerializeField]
    public bool PresetLocation = false;

    public int Level = 1;

    public float RevneueMultiplier;
    public float RiskMultiplier;

    public bool IsUpgrading;

    public int CurrentUpgradeLength;

    public UnityEvent StateUpdated;

    public Property.PropertyAction CurrentAction;

    public List<Character> CharactersInLocation = new List<Character>();

    GameObject SelectedMarkerObject;

    CharactersInLocationUI CharactersInLocationUIInstance;

    [SerializeField]
    List<AgentAction> PossibleAgentActions = new List<AgentAction>();

    [SerializeField]
    List<PlayerAction> PossiblePlayerActions = new List<PlayerAction>();

    public LongTermTaskDurationUI TaskDurationUI;

    void Awake()
    {
        if (PresetLocation)
        {
            CORE.Instance.PresetLocations.Add(this);
        }
    }

    void Start()
    {
        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);
        GameClock.Instance.OnDayPassed.AddListener(DayPassed);
    }

    public bool IsOwnedByPlayer
    {
        get
        {
            return OwnerCharacter != null && (OwnerCharacter == CORE.PC || OwnerCharacter.TopEmployer == CORE.PC);
        }
    }

    public PropertyTrait[] Traits
    {
        get
        {
            List<PropertyTrait> traits = new List<PropertyTrait>();
            traits.InsertRange(0, TemporaryTraits);
            traits.InsertRange(0, CurrentProperty.Traits);

            return traits.ToArray();
        }
    }

    public List<PropertyTrait> TemporaryTraits = new List<PropertyTrait>();

    public override List<AgentAction> GetPossibleAgentActions(Character forCharacter)
    {
        List<AgentAction> actions = new List<AgentAction>();
        actions.InsertRange(0, PossibleAgentActions);
        actions.InsertRange(0, CurrentProperty.UniqueAgentActions);

        return actions;
    }

    public override List<PlayerAction> GetPossiblePlayerActions()
    {
        List<PlayerAction> actions = new List<PlayerAction>();
        actions.InsertRange(0, PossiblePlayerActions);
        actions.InsertRange(0, CurrentProperty.UniquePlayerActions);

        return actions;
    }

    public void OnRightClick()
    {
        ShowActionMenu();
    }

    public void InitializePreset()
    {
        if (CurrentProperty != null)
        {
            SetInfo(this.name, CurrentProperty, 1f, 1f, false);

            if (OwnerCharacter != null)
            {
                Character ownerGameInstance = CORE.Instance.GetCharacter(OwnerCharacter.name);
                if (ownerGameInstance != null)
                {
                    ownerGameInstance.StartOwningLocation(this);
                }
            }

            List<Character> charactersToAdd = new List<Character>();
            while (EmployeesCharacters.Count > 0)
            {
                Character tempChar = CORE.Instance.GetCharacter(EmployeesCharacters[0].name);

                EmployeesCharacters.RemoveAt(0);

                if (tempChar == null)
                {
                    continue;
                }

                charactersToAdd.Add(tempChar);
            }
            foreach (Character character in charactersToAdd)
            {
                character.StartWorkingFor(this);
            }

        }
    }

    void TurnPassed()
    {
        ProgressUpgrade();
 
        RefreshState();
    }

    void DayPassed()
    {
        AttemptRandomEvent();
    }

    void AttemptRandomEvent()
    {
        foreach (GameEvent gEvent in CurrentAction.PossibleEvents)
        {
            if(gEvent.RollChance())
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("Location", this);
                parameters.Add("From", OwnerCharacter);
                parameters.Add("To", OwnerCharacter.TopEmployer);
                gEvent.Execute(
                    parameters, 
                    OwnerCharacter != null && OwnerCharacter.TopEmployer == CORE.PC);
                return;
            }
        }
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

    public void SetInfo(string id, Property property, float revenueMultiplier = 1f, float riskMultiplier = 1f, bool Clear = false)
    {
        if(Clear)
        {
            EmployeesCharacters.Clear();
            OwnerCharacter = null;
        }

        this.ID = id;
        this.RevneueMultiplier = revenueMultiplier;
        this.RiskMultiplier = riskMultiplier;

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

        GameObject tempFigure = Instantiate(CurrentProperty.FigurePrefab);
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

    public bool AttemptRecruiting()
    {
        if (EmployeesCharacters.Count >= CurrentProperty.PropertyLevels[Level - 1].MaxEmployees)
        {
            return false;
        }

        CORE.Instance.Database.GetEventAction("Recruit Employees").Execute(OwnerCharacter, OwnerCharacter, this);

        return true;
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

    public void Rebrand(Property newProperty)
    {
        if (!IsOwnedByPlayer)
        {
            GlobalMessagePrompterUI.Instance.Show("YOU DON'T OWN THIS PLACE!", 1f, Color.red);
            return;
        }

        SelectedPanelUI.Instance.Deselect();

        CancelUpgrade();
        //TODO - Stop recruitments...
        Level = 1;

        while(EmployeesCharacters.Count > 0)
        {
            EmployeesCharacters[0].StopWorkingFor(this);
        }

        SetInfo(Util.GenerateUniqueID(), newProperty, this.RevneueMultiplier, this.RiskMultiplier, false);

        SelectedPanelUI.Instance.Select(this);
    }

    public void CharacterEnteredLocation(Character character)
    {
        CharactersInLocation.Add(character);

        if(CharactersInLocationUIInstance == null)
        {
            CharactersInLocationUIInstance = ResourcesLoader.Instance.GetRecycledObject("CharactersInLocationUI").GetComponent<CharactersInLocationUI>();
            CharactersInLocationUIInstance.transform.SetParent(CORE.Instance.MainCanvas.transform);
            CharactersInLocationUIInstance.transform.localScale = Vector3.one;
            CharactersInLocationUIInstance.transform.SetAsFirstSibling();
        }

        CharactersInLocationUIInstance.SetInfo(this);
    }

    public void CharacterLeftLocation(Character character)
    {
        CharactersInLocation.Remove(character);

        if (CharactersInLocation.Count == 0 && CharactersInLocationUIInstance != null)
        {
            CharactersInLocationUIInstance.gameObject.SetActive(false);
            CharactersInLocationUIInstance = null;
            return;
        }

        CharactersInLocationUIInstance.SetInfo(this);
    }


    public void AddLongTermTask(LongTermTaskEntity entity)
    {
        if(TaskDurationUI == null)
        {
            TaskDurationUI = Instantiate(ResourcesLoader.Instance.GetObject("LongTermTaskWorld")).GetComponent<LongTermTaskDurationUI>();
            TaskDurationUI.transform.SetParent(CORE.Instance.MainCanvas.transform);
            TaskDurationUI.transform.SetAsFirstSibling();
        }
        else
        {
            if (TaskDurationUI.Instances.ContainsKey(entity.CurrentTask) && TaskDurationUI.Instances[entity.CurrentTask].Contains(entity))
            {
                return;
            }
        }

        foreach(PropertyTrait trait in entity.CurrentTask.TraitsToTargetDuringAction)
        {
            TemporaryTraits.Add(trait);
        }

        if (entity.CurrentCharacter.CurrentFaction == CORE.PC.CurrentFaction)
        {
            TaskDurationUI.AddEntity(entity);
        }
    }

    public void RemoveLongTermTask(LongTermTaskEntity entity)
    {
        if(TaskDurationUI == null)
        {
            return;
        }

        if(!(TaskDurationUI.Instances.ContainsKey(entity.CurrentTask) && TaskDurationUI.Instances[entity.CurrentTask].Contains(entity)))
        {
            return;
        }
        
        foreach (PropertyTrait trait in entity.CurrentTask.TraitsToTargetDuringAction)
        {
            TemporaryTraits.Remove(trait);
        }

        TaskDurationUI.RemoveEntity(entity);

        if (TaskDurationUI.Instances.Count > 0)
        {
            return;
        }

        TaskDurationUI.gameObject.SetActive(false);
        TaskDurationUI = null;
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["ID"] = ID;
        node["CurrentProperty"] = CurrentProperty.name;
        node["Level"] = Level.ToString();
        node["RevneueMultiplier"] = RevneueMultiplier.ToString();
        node["RiskMultiplier"] = RiskMultiplier.ToString();
        node["IsUpgrading"] = IsUpgrading.ToString();
        node["CurrentUpgradeLength"] = CurrentUpgradeLength.ToString();

        if (CurrentAction != null)
        {
            node["CurrentAction"] = CurrentAction.Name;
        }


        node["PositionX"] = transform.position.x.ToString();
        node["PositionY"] = transform.position.y.ToString();
        node["PositionZ"] = transform.position.z.ToString();

        node["RotationX"] = transform.rotation.eulerAngles.x.ToString();
        node["RotationY"] = transform.rotation.eulerAngles.y.ToString();
        node["RotationZ"] = transform.rotation.eulerAngles.z.ToString();

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        Level = int.Parse(node["Level"]);
        SetInfo(node["ID"], CORE.Instance.Database.GetPropertyByName(node["CurrentProperty"]), float.Parse(node["RevneueMultiplier"]), float.Parse(node["RiskMultiplier"]), true);
        IsUpgrading = bool.Parse(node["IsUpgrading"]);
        CurrentUpgradeLength = int.Parse(node["CurrentUpgradeLength"]);
        CurrentAction = CurrentProperty.GetActionByName(node["CurrentAction"]);
        

        transform.position = new Vector3(float.Parse(node["PositionX"]), float.Parse(node["PositionY"]), float.Parse(node["PositionZ"]));
        transform.rotation = Quaternion.Euler(float.Parse(node["RotationX"]), float.Parse(node["RotationY"]), float.Parse(node["RotationZ"]));
    }

    public void ImplementIDs()
    {
    }
}
