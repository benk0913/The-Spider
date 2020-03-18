using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocationEntity : AgentInteractable, ISaveFileCompatible
{
    public string ID;

    public string Name
    {
        get
        {
            name = CurrentProperty.PropertyLevels[Level - 1].LevelName;
            if (string.IsNullOrEmpty(name))
            {
                name = CurrentProperty.name;
            }

            return name;
        }
    }

    public const float PORTRAITS_SPACING = 5f;
    public const int PORTRAITS_MAX_IN_ROW = 5;

    [SerializeField]
    public int LandValue;

    [SerializeField]
    public Property CurrentProperty;

    [SerializeField]
    public Character OwnerCharacter;

    [SerializeField]
    public List<Character> EmployeesCharacters = new List<Character>();

    [SerializeField]
    public List<Character> GuardsCharacters = new List<Character>();

    [SerializeField]
    public List<Character> PrisonersCharacters = new List<Character>();

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

    [SerializeField]
    public bool StartsDisabled = false;

    [SerializeField]
    GameObject WhenHiddenObject;

    public List<Character> FiredEmployeees = new List<Character>();

    public int Level = 1;

    public bool IsUpgrading;

    public bool IsRuined;

    public int CurrentUpgradeLength;

    public bool IsDisabled = false;

    public UnityEvent StateUpdated;

    public Property.PropertyAction CurrentAction;

    public List<Character> CharactersInLocation = new List<Character>();
    public List<Character> CharactersLivingInLocation = new List<Character>();

    public LocationKnowledge Known;


    public LocationEntity NearestDistrict;

    GameObject SelectedMarkerObject;

    CharactersInLocationUI CharactersInLocationUIInstance;

    [SerializeField]
    protected List<AgentAction> PossibleAgentActions = new List<AgentAction>();

    [SerializeField]
    protected List<PlayerAction> PossiblePlayerActions = new List<PlayerAction>();

    public LongTermTaskDurationUI TaskDurationUI;

    public List<LongTermTaskEntity> LongTermTasks = new List<LongTermTaskEntity>();

    public Faction FactionInControl
    {
        get
        {
            if (Traits.Contains(CORE.Instance.Database.CentralAreaTrait))
            {
                List<LocationEntity> locationsInDistrict = CORE.Instance.Locations.FindAll(x => 
                x.NearestDistrict == this 
                && !x.Traits.Contains(CORE.Instance.Database.PublicAreaTrait)
                && x.CurrentProperty.PlotType != CORE.Instance.Database.UniquePlotType
                && x.gameObject.activeInHierarchy);
                
                if(locationsInDistrict.Count > 0)
                {
                    Faction Potential = locationsInDistrict[0].FactionInControl;
                    if(locationsInDistrict.Find(x=>x.FactionInControl != Potential))
                    {
                        return CORE.Instance.Database.NoFaction;
                    }

                    return Potential;
                }
            }

            if(OwnerCharacter != null)
            {
                return OwnerCharacter.CurrentFaction;
            }

            return CORE.Instance.Database.NoFaction;
        }
    }

    public bool IsBuyable
    {
        get
        {
            return OwnerCharacter == null && CurrentProperty.PlotType != CORE.Instance.Database.UniquePlotType;
        }
    }

    public bool HasFreePrisonCell
    {
        get
        {

            return PrisonersCharacters.Count < CurrentProperty.PropertyLevels[Level - 1].MaxPrisoners;
        }
    }

    public VisibilityStateEnum VisibilityState
    {
        get
        {
            if (Known.GetKnowledgeInstance("Existance").IsKnownByCharacter(CORE.PC)) //If player has scouted this location.
            {
                return VisibilityStateEnum.Visible;
            }
            else if (NearestDistrict == null || NearestDistrict.Known.GetKnowledgeInstance("Existance").IsKnownByCharacter(CORE.PC)) //If player has scouted the nearest district
            {
                return VisibilityStateEnum.QuestionMark;
            }

            //Player didn't scout nearest district.
            return VisibilityStateEnum.Hidden;
        }
    }

    void Awake()
    {
        if (PresetLocation)
        {
            CORE.Instance.PresetLocations.Add(this);
        }
    }

    void Start()
    {
        GameClock.Instance.OnDayPassed.AddListener(DayPassed);
    }

    public bool IsOwnedByPlayer
    {
        get
        {
            return OwnerCharacter != null && (OwnerCharacter == CORE.PC || OwnerCharacter.TopEmployer == CORE.PC);
        }
    }

    public List<Trait> Traits
    {
        get
        {
            List<Trait> traits = new List<Trait>();
            traits.InsertRange(0, TemporaryTraits);
            traits.InsertRange(0, CurrentProperty.Traits); 

            return traits;
        }
    }

    public List<Item> Inventory = new List<Item>();

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
        if(VisibilityState == VisibilityStateEnum.Hidden)
        {
            return;
        }

        ShowActionMenu();
    }

    public void InitializePreset()
    {
        if (CurrentProperty != null)
        {
            SetInfo(this.name, CurrentProperty, false);

            if (OwnerCharacter != null)
            {
                Character ownerGameInstance = CORE.Instance.GetCharacter(OwnerCharacter.name);
                if (ownerGameInstance != null)
                {
                    ownerGameInstance.StartOwningLocation(this);
                }
            }

            List<Character> employeesToAdd = new List<Character>();
            while (EmployeesCharacters.Count > 0)
            {
                Character tempChar = CORE.Instance.GetCharacter(EmployeesCharacters[0].name);

                EmployeesCharacters.RemoveAt(0);

                if (tempChar == null)
                {
                    continue;
                }

                employeesToAdd.Add(tempChar);
            }

            List<Character> guardsToAdd = new List<Character>();
            while (GuardsCharacters.Count > 0)
            {
                Character tempChar = CORE.Instance.GetCharacter(GuardsCharacters[0].name);

                GuardsCharacters.RemoveAt(0);

                if (tempChar == null)
                {
                    continue;
                }

                guardsToAdd.Add(tempChar);
            }
            foreach (Character character in employeesToAdd)
            {
                character.StartWorkingFor(this);
            }
            foreach (Character character in guardsToAdd)
            {
                character.StartWorkingFor(this,true);
            }

            employeesToAdd.Clear();
            guardsToAdd.Clear();
            while (CharactersLivingInLocation.Count > 0)
            {
                Character tempChar = CORE.Instance.GetCharacter(CharactersLivingInLocation[0].name);

                CharactersLivingInLocation.RemoveAt(0);

                if (tempChar == null)
                {
                    continue;
                }

                employeesToAdd.Add(tempChar);
            }
            foreach (Character character in employeesToAdd)
            {
                character.StartLivingIn(this);
            }

            employeesToAdd.Clear();
            while (CharactersInLocation.Count > 0)
            {
                Character tempChar = CORE.Instance.GetCharacter(CharactersInLocation[0].name);

                CharactersInLocation.RemoveAt(0);

                if (tempChar == null)
                {
                    continue;
                }

                employeesToAdd.Add(tempChar);
            }
            foreach (Character character in employeesToAdd)
            {
                character.GoToLocation(this);
            }
        }

        if(StartsDisabled)
        {
            DisableProperty();
        }
    }

    public void EnableProperty()
    {
        IsDisabled = false;

        this.gameObject.SetActive(true);
        foreach (Character character in CharactersInLocation)
        {
            character.EnableCharacter();
        }
    }

    public void DisableProperty()
    {
        IsDisabled = true;

        this.gameObject.SetActive(false);
        foreach(Character character in CharactersInLocation)
        {
            character.DisableCharacter();
        }
    }

    public IEnumerator TurnPassed()
    {
        if(IsDisabled)
        {
            yield break;
        }

        ProgressUpgrade();

        List<LongTermTaskEntity> CompleteEarly = new List<LongTermTaskEntity>();
        List<LongTermTaskEntity> CompleteLate = new List<LongTermTaskEntity>();
        List<LongTermTaskEntity> CompleteNormal = new List<LongTermTaskEntity>();

        foreach (LongTermTaskEntity task in LongTermTasks)
        {
            if (task.CurrentTask.CompleteLate)
            {
                CompleteLate.Add(task);
            }
            else if (task.CurrentTask.CompleteEarly)
            {
                CompleteEarly.Add(task);
            }
            else
            {
                CompleteNormal.Add(task);
            }
        }

        foreach (LongTermTaskEntity task in CompleteEarly)
        {
            task.TurnPassed();
            yield return 0;
        }

        foreach (LongTermTaskEntity task in CompleteNormal)
        {
            task.TurnPassed();
            yield return 0;
        }

        foreach (LongTermTaskEntity task in CompleteLate)
        {
            task.TurnPassed();
            yield return 0;
        }

        if(CurrentProperty.AutoRestock)
        {
            if(Inventory.Count > 0)
            {
                Inventory.RemoveAt(0);
            }

            while(Inventory.Count < CurrentProperty.PropertyLevels[Level - 1].InventoryCap)
            {
               int rnd = Random.Range(0, CurrentProperty.PropertyLevels[Level - 1].PossibleMerchantise.Length);
               Inventory.Add(CurrentProperty.PropertyLevels[Level - 1].PossibleMerchantise[rnd].Clone());
            }
        }

        DistrictBonuses();

        RefreshState();

        yield return 0;
    }

    void DayPassed() //TODO move to mechanical flow
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

    public void DistrictBonuses()
    {
        Faction faction = FactionInControl;
        if(!Traits.Contains(CORE.Instance.Database.CentralAreaTrait))
        {
            return;
        }

        if(faction == null)
        {
            return;
        }

        if(faction == CORE.Instance.Database.NoFaction)
        {
            return;
        }


        if(faction.FactionHead == null)
        {
            return;
        }

        Character factionHead = CORE.Instance.Characters.Find(x => faction.FactionHead.name == x.name);

        if(factionHead == null)
        {
            return;
        }

        if (CurrentProperty.DistrictBonus.ProgressionPerTurn != 0)
        {
            factionHead.Progress += CurrentProperty.DistrictBonus.ProgressionPerTurn;

            if (factionHead == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject(
                 "ScrollCollectedWorld",
                 transform,
                 StatsViewUI.Instance.ProgressText.transform,
                 () => { StatsViewUI.Instance.RefreshProgress(); },
                 false);

                AudioControl.Instance.PlayInPosition("resource_progression", transform.position);
            }
        }

        if (CurrentProperty.DistrictBonus.GoldPerTurn != 0)
        {
            factionHead.Gold += CurrentProperty.DistrictBonus.GoldPerTurn;

            if (factionHead == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject(
             "CoinCollectedWorld",
             transform,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshGold(); },
             false);

                AudioControl.Instance.PlayInPosition("resource_gold", transform.position);
            }
        }

        if (CurrentProperty.DistrictBonus.RumorsPerTurn != 0)
        {
            factionHead.Rumors += CurrentProperty.DistrictBonus.RumorsPerTurn;

            if (factionHead == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject(
             "EarCollectedWorld",
             transform,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshRumors(); },
             false);

                AudioControl.Instance.PlayInPosition("resource_rumors", transform.position);
            }
        }

        if (CurrentProperty.DistrictBonus.ConnectionsPerTurn != 0)
        {
            factionHead.Connections += CurrentProperty.DistrictBonus.ConnectionsPerTurn;

            if (factionHead == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject(
             "ConnectionCollectedWorld",
             transform,
             StatsViewUI.Instance.ConnectionsText.transform,
             () => { StatsViewUI.Instance.RefreshConnections(); },
             false);

                AudioControl.Instance.PlayInPosition("resource_connections", transform.position);
            }
        }
    }

    public void OnClick()
    {
        if (VisibilityState == VisibilityStateEnum.Visible)
        {
            SelectedPanelUI.Instance.Select(this);
        }
    }

    public void OnHover()
    {
        if (VisibilityState == VisibilityStateEnum.Visible || VisibilityState == VisibilityStateEnum.QuestionMark)
        {
            LocationHoverUI.Instance.Show(this, VisibilityState == VisibilityStateEnum.QuestionMark);
        }
    }

    public void OnUnhover()
    {
        LocationHoverUI.Instance.Hide();
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

    public void SetInfo(string id, Property property,bool Clear = false)
    {
        if(Clear)
        {
            EmployeesCharacters.Clear();
            OwnerCharacter = null;
        }

        Known = new LocationKnowledge(this);

        if (OwnerCharacter != null)
        {
            Known.KnowEverything(OwnerCharacter.TopEmployer);
        }

        this.ID = id;

        CurrentProperty = property;

        CurrentAction = CurrentProperty.Actions[0];

        if (CurrentProperty.AlwaysKnown)
        {
            NearestDistrict = null;
            Known.KnowAll("Existance");
        }
        else if (!Traits.Contains(CORE.Instance.Database.CentralAreaTrait)) //If not a district
        {
            NearestDistrict = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.CentralAreaTrait, this);
        }

        if(CurrentProperty.HiddenObject != null)
        {
            this.WhenHiddenObject = ResourcesLoader.Instance.GetRecycledObject(CurrentProperty.HiddenObject);
            this.WhenHiddenObject.transform.SetParent(transform);
            this.WhenHiddenObject.transform.localPosition = CurrentProperty.HiddenObject.transform.position;
            this.WhenHiddenObject.transform.localRotation = CurrentProperty.HiddenObject.transform.rotation;
        }

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

        GameObject tempFigure;
        GameObject hoverModel;
        if (VisibilityState == VisibilityStateEnum.Visible) //If player has scouted this location.
        {
            if (IsRuined)
            {
                tempFigure = Instantiate(CORE.Instance.Database.RuinsFigurePrefab);
                hoverModel = Instantiate(CORE.Instance.Database.RuinsHoverPrefab);

            }
            else
            {
                tempFigure = Instantiate(CurrentProperty.FigurePrefab);
                hoverModel = Instantiate(CurrentProperty.HoverPrefab);
            }

            if (CurrentProperty.MaterialOverride != null)
            {
                tempFigure.GetComponent<FigureController>().SetMaterial(CurrentProperty.MaterialOverride);
            }
            else
            {
                if (OwnerCharacter == null)
                {
                    tempFigure.GetComponent<FigureController>().SetMaterial(FactionInControl.WaxMaterial);
                }
                else
                {
                    if (FactionInControl.Known != null && FactionInControl.Known.IsKnown("Existance", CORE.PC))
                    {
                        tempFigure.GetComponent<FigureController>().SetMaterial(FactionInControl.WaxMaterial);
                    }
                    else
                    {
                        tempFigure.GetComponent<FigureController>().SetMaterial(CORE.Instance.Database.DefaultFaction.WaxMaterial);
                    }
                }
            }

            if (WhenHiddenObject != null)
            {
                WhenHiddenObject.gameObject.SetActive(false);
            }
        }
        else if (VisibilityState == VisibilityStateEnum.QuestionMark) //If player has scouted the nearest district
        {
            if (NearestDistrict == null)
            {
                tempFigure = Instantiate(CORE.Instance.Database.UnknownFigurePrefabBIG);
                //tempFigure.GetComponent<FigureController>().SetMaterial(CORE.Instance.Database.DefaultFaction.WaxMaterial);
            }
            else
            {
                tempFigure = Instantiate(CORE.Instance.Database.UnknownFigurePrefab);
                //tempFigure.GetComponent<FigureController>().SetMaterial(CORE.Instance.Database.DefaultFaction.WaxMaterial);
            }

            hoverModel = Instantiate(CORE.Instance.Database.UnknownFigureHover);

            if (WhenHiddenObject != null)
            {
                WhenHiddenObject.gameObject.SetActive(true);
            }
        }
        else //Player didn't scout nearest district.
        {
            if (WhenHiddenObject != null)
            {
                WhenHiddenObject.gameObject.SetActive(true);
            }

            tempFigure = null;
            hoverModel = null;
        }

        if (tempFigure != null)
        {
            tempFigure.transform.SetParent(FigurePoint);
            tempFigure.transform.position = FigurePoint.position;
            tempFigure.transform.rotation = FigurePoint.rotation;
        }

        if (hoverModel != null)
        {
            hoverModel.transform.SetParent(HoverPoint);
            hoverModel.transform.position = HoverPoint.position;
            hoverModel.transform.rotation = HoverPoint.rotation;
        }

        StateUpdated.Invoke();
    }

    public void PurchaseUpgrade()
    {
        PurchaseUpgrade(OwnerCharacter.TopEmployer);
    }

    public FailReason PurchaseUpgrade(Character funder)
    { 
        if(funder != OwnerCharacter.TopEmployer)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("YOU DON'T OWN THIS PLACE!", 1f, Color.red);
            }

            return new FailReason("Do Not Own Property");
        }

        if (IsRuined)
        {
            return new FailReason("Location Is Ruined");
        }

        if (Level >= CurrentProperty.PropertyLevels.Count)
        {
            return new FailReason("Level Already Maxed Out");
        }

        if (funder.Gold < CurrentProperty.PropertyLevels[Level].UpgradePrice)
        {
            GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                "(" +funder.Gold+"/"+ CurrentProperty.PropertyLevels[Level].UpgradePrice + ")", 1f, Color.red);

            return new FailReason("Not Enough Gold", (CurrentProperty.PropertyLevels[Level].UpgradePrice - funder.Gold));
        }
        
        if(funder == CORE.PC)
        {
            AudioControl.Instance.PlayInPosition("property_upgrade",transform.position);
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

        return null;
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

            if (OwnerCharacter != null && OwnerCharacter.TopEmployer == CORE.PC)
            {
                CORE.Instance.InvokeEvent("Upgrade Complete");
            }

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

    public FailReason SelectAction(Character requester, Property.PropertyAction action)
    {
        if (OwnerCharacter == null || requester != OwnerCharacter.TopEmployer)
        {
            if (requester == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("YOU DON'T OWN THIS PLACE!", 1f, Color.red);
            }

            return new FailReason("Do Not Own Property");
        }

        if (requester == CORE.PC)
        {
            AudioControl.Instance.Play("property_select");
        }

        CurrentAction = action;

        StateUpdated.Invoke();

        return null;
    }

    public FailReason Rebrand(Character requester, Property newProperty)
    {
        if (requester != OwnerCharacter.TopEmployer)
        {
            if (requester == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("YOU DON'T OWN THIS PLACE!", 1f, Color.red);
            }

            return new FailReason("Do Not Own Property");
        }

        SelectedPanelUI.Instance.Deselect();

        CancelUpgrade();
        //TODO - Stop recruitments...
        Level = 1;

        while (EmployeesCharacters.Count > newProperty.PropertyLevels[Level-1].MaxEmployees)
        {
            EmployeesCharacters[EmployeesCharacters.Count-1].StopWorkingForCurrentLocation();
        }

        EmployeesCharacters.FindAll(x =>
            (newProperty.RecruitingGenderType != -1 && (int)x.Gender != newProperty.RecruitingGenderType)
            || newProperty.MinAge > x.Age
            || newProperty.MaxAge < x.Age
        ).ForEach((x) =>
        {
            x.StopWorkingForCurrentLocation();
        });

        SetInfo(Util.GenerateUniqueID(), newProperty, false);

        if (requester == CORE.PC)
        {
            SelectedPanelUI.Instance.Select(this);
        }

        if (EmployeesCharacters.Count == 0)
        {
            RecruitEmployeeForce();
        }

        return null;
    }

    public void CharacterEnteredLocation(Character character)
    {
        CharactersInLocation.Add(character);

        if(OwnerCharacter != null)
        {
            character.Known.Know("Appearance", OwnerCharacter.TopEmployer);
            character.Known.Know("CurrentLocation", OwnerCharacter.TopEmployer);
        }


        if (CharactersInLocationUIInstance != null)
        {
            CharactersInLocationUIInstance.AddCharacter(character);
        }
        else
        {
            RefreshCharactersInLocationUI();
        }
    }

    public void CharacterLeftLocation(Character character)
    {
        CharactersInLocation.Remove(character);

        //if (CharactersInLocation.Count == 0 && CharactersInLocationUIInstance != null)
        //{
        //    CharactersInLocationUIInstance.gameObject.SetActive(false);
        //    CharactersInLocationUIInstance = null;
        //    return;
        //}

        if(CharactersInLocationUIInstance != null)
        {
            CharactersInLocationUIInstance.RemoveCharacter(character);
        }
    }
    
    public void RefreshCharactersInLocationUI()
    {
        if (CharactersInLocationUIInstance == null)
        {
            CharactersInLocationUIInstance = ResourcesLoader.Instance.GetRecycledObject("CharactersInLocationUI").GetComponent<CharactersInLocationUI>();
            CharactersInLocationUIInstance.transform.SetParent(CORE.Instance.DisposableContainer);
            CharactersInLocationUIInstance.transform.localScale = Vector3.one;
            CharactersInLocationUIInstance.transform.SetAsFirstSibling();
        }

        CharactersInLocationUIInstance.SetInfo(this);
    }

    public void RefreshTasks()
    {


        if (TaskDurationUI == null)
        {
            TaskDurationUI = Instantiate(ResourcesLoader.Instance.GetObject("LongTermTaskWorld")).GetComponent<LongTermTaskDurationUI>();
            TaskDurationUI.transform.SetParent(CORE.Instance.DisposableContainer);
            TaskDurationUI.transform.SetAsFirstSibling();
        }

        foreach(LongTermTaskEntity task in LongTermTasks)
        {
            task.RefreshKnownTaskState();
            if (task.isKnownTask && !TaskDurationUI.Contains(task))
            {
                TaskDurationUI.AddEntity(task);
            }
        }

        TaskDurationUI.Refresh();
    }

    public void AddLongTermTask(LongTermTaskEntity entity)
    {
        LongTermTasks.Add(entity);

        if(TaskDurationUI == null)
        {
            TaskDurationUI = Instantiate(ResourcesLoader.Instance.GetObject("LongTermTaskWorld")).GetComponent<LongTermTaskDurationUI>();
            TaskDurationUI.transform.SetParent(CORE.Instance.DisposableContainer);
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

        if (entity.isKnownTask)
        {
            TaskDurationUI.AddEntity(entity);
        }
    }

    public void RemoveLongTermTask(LongTermTaskEntity entity)
    {
        LongTermTasks.Remove(entity);

        if (TaskDurationUI == null)
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

        if (entity.isKnownTask)
        {
            TaskDurationUI.RemoveEntity(entity);

            if (TaskDurationUI.Instances.Keys.Count == 0)
            {
                Destroy(TaskDurationUI.gameObject);
                TaskDurationUI = null;
            }
        }
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["ID"] = ID;
        node["CurrentProperty"] = CurrentProperty.name;
        node["Level"] = Level.ToString();
        node["IsUpgrading"] = IsUpgrading.ToString();
        node["IsRuined"] = IsRuined.ToString();
        node["CurrentUpgradeLength"] = CurrentUpgradeLength.ToString();
        node["NearestDistrict"] = NearestDistrict == null? "" : NearestDistrict.ID;
        node["LandValue"] = LandValue.ToString();
        node["IsDisabled"] = IsDisabled.ToString();

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

        foreach (KnowledgeInstance item in Known.Items)
        {
            for (int i = 0; i < item.KnownByCharacters.Count; i++)
            {
                node["Knowledge"][item.Key][i] = item.KnownByCharacters[i].ID;
            }
        }

        for (int i = 0; i < Traits.Count; i++)
        {
            node["Traits"][i] = Traits[i].name;
        }

        for(int i=0;i<Inventory.Count;i++)
        {
            node["Inventory"][i] = Inventory[i].name;
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        Level = int.Parse(node["Level"]);
        SetInfo(node["ID"], CORE.Instance.Database.GetPropertyByName(node["CurrentProperty"]), true);
        IsUpgrading = bool.Parse(node["IsUpgrading"]);
        IsRuined = bool.Parse(node["IsRuined"]);
        CurrentUpgradeLength = int.Parse(node["CurrentUpgradeLength"]);
        CurrentAction = CurrentProperty.GetActionByName(node["CurrentAction"]);
        _nearestDistrictID = node["NearestDistrict"];
        LandValue = int.Parse(node["LandValue"]);
        IsDisabled = bool.Parse(node["IsDisabled"]);


        transform.position = new Vector3(float.Parse(node["PositionX"]), float.Parse(node["PositionY"]), float.Parse(node["PositionZ"]));
        transform.rotation = Quaternion.Euler(float.Parse(node["RotationX"]), float.Parse(node["RotationY"]), float.Parse(node["RotationZ"]));

        knowledgeCharacterIDs.Clear();
        foreach (KnowledgeInstance item in Known.Items)
        {
            if(node["Knowledge"][item.Key].Count == 0)
            {
                continue;
            }

            List<string> IDs = new List<string>();
            for (int i = 0; i < node["Knowledge"][item.Key].Count; i++)
            {
                IDs.Add(node["Knowledge"][item.Key][i]);
            }

            knowledgeCharacterIDs.Add(item.Key, IDs);
        }

        Traits.Clear();
        for (int i = 0; i < node["Traits"].Count; i++)
        {
            Traits.Add(CORE.Instance.Database.GetTrait(node["Traits"][i]));
        }

        Inventory.Clear();
        for (int i = 0; i < node["Inventory"].Count; i++)
        {
            Item itemPreset = CORE.Instance.Database.AllItems.Find(x => x.name == node["Inventory"][i].Value);
            
            if(itemPreset == null)
            {
                Debug.LogError("COULDN'T LOAD " + node["Inventory"]);
                continue;
            }

            Inventory.Add(itemPreset.Clone());
        }
    }

    Dictionary<string, List<string>> knowledgeCharacterIDs = new Dictionary<string, List<string>>();
    string _nearestDistrictID;

    public void ImplementIDs()
    {

        foreach (string key in knowledgeCharacterIDs.Keys)
        {
            for (int i = 0; i < knowledgeCharacterIDs[key].Count; i++)
            {
                if(string.IsNullOrEmpty(key))
                {
                    Debug.LogError("Location - " + this.Name + " owned by " + OwnerCharacter + " has a null key ");
                    continue;
                }

                Character character = CORE.Instance.GetCharacterByID(knowledgeCharacterIDs[key][i]);

                if (character == null)
                {
                    continue;
                }

                Known.Know(key, character);
            }
        }

        if(!string.IsNullOrEmpty(_nearestDistrictID))
        {
            NearestDistrict = CORE.Instance.Locations.Find(x => x.ID == _nearestDistrictID);
        }

        if(IsDisabled)
        {
            DisableProperty();
            return;
        }

        RefreshState();
    }

    public void Dispose()
    {
        while(CharactersInLocation.Count > 0)
        {
            CharactersInLocation[0].GoToRandomLocation();
        }

        while (CharactersLivingInLocation.Count > 0)
        {
            CharactersLivingInLocation[0].StopLivingInCurrentLocation();
        }

        while (EmployeesCharacters.Count > 0)
        {
            EmployeesCharacters[0].StopWorkingForCurrentLocation();
        }

        while (GuardsCharacters.Count > 0)
        {
            GuardsCharacters[0].StopWorkingForCurrentLocation();
        }

        for(int i=0;i<PrisonersCharacters.Count;i++)
        {
            PrisonersCharacters[i].Death();
        }

        if (TaskDurationUI != null)
        {
            TaskDurationUI.Wipe();
        }

        if(OwnerCharacter != null)
        {
            if (OwnerCharacter.PropertiesOwned.Count == 1 && OwnerCharacter.TopEmployer == OwnerCharacter) //If head of faction and this is the last property.
            {
                if (OwnerCharacter == CORE.PC)
                {
                    WarningWindowUI.Instance.Show(this.name + " has lost the manor! GAME OVER.", () => { LoseWindowUI.Instance.Show(); });
                }
                else
                {
                    //TODO FACTION DED
                }
            }

            OwnerCharacter.StopOwningLocation(this);
        }
    }

    public void BecomeRuins()
    {
        Dispose();
        IsRuined = true;
        RefreshState();
    }

    public FailReason RepairRuins(Character funder)
    {
        if(OwnerCharacter != null && OwnerCharacter.TopEmployer != funder)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("This location is owned by someone else! ", 1f, Color.red);
            }

            return new FailReason("Location Unavailable");
        }

        if (funder.Gold < 40)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("Need More Gold: (" + CORE.PC.Gold + "/" + 40 + ")", 1f, Color.red);
            }

            return new FailReason("Not Enough Gold");
        }

        funder.Gold -= 40;

        IsRuined = false;
        RefreshState();

        return null;
    }

    public FailReason RecruitEmployee(Character requester, bool isGuard = false)
    {
        int recruitmentCost = CORE.Instance.Database.BaseRecruitmentCost;
        recruitmentCost += CORE.Instance.Database.GetReputationType(OwnerCharacter.TopEmployer.Reputation).RecruitExtraCost;

        if (requester != OwnerCharacter.TopEmployer)
        {
            if (requester == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("You do not own this proeprty!", 1f, Color.red);
            }

            return new FailReason("Do Not Own Property");
        }

        if (requester.Connections < recruitmentCost) //TODO replace magic number...
        {
            if (requester == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("Need more connections (" + requester.Connections + "/" + recruitmentCost.ToString(), 1f, Color.red);
            }

            return new FailReason("Not Enough Connections",3);
        }

        if (isGuard)
        {
            if (GuardsCharacters.Count >= CurrentProperty.PropertyLevels[Level - 1].MaxGuards) //TODO replace magic number...
            {
                if (requester == CORE.PC)
                {
                    GlobalMessagePrompterUI.Instance.Show("Location Is Full Of Guards", 1f, Color.red);
                }

                return new FailReason("Location Is Full Of Guards", 3);
            }
        }
        else
        {
            if (EmployeesCharacters.Count >= CurrentProperty.PropertyLevels[Level - 1].MaxEmployees) //TODO replace magic number...
            {
                if (requester == CORE.PC)
                {
                    GlobalMessagePrompterUI.Instance.Show("Location Is Full Of Employees", 1f, Color.red);
                }

                if (CurrentProperty.EmployeesAreAgents)
                {
                    return new FailReason("Location Is Full Of Agents", 3);
                }
                else
                {
                    return new FailReason("Location Is Full Of Employees", 3);
                }
            }
        }

        requester.Connections -= recruitmentCost;

        Character randomNewEmployee = CORE.Instance.GenerateCharacter(
               CurrentProperty.RecruitingGenderType,
               CurrentProperty.MinAge,
               CurrentProperty.MaxAge,
               this);

        randomNewEmployee.StartWorkingFor(this, isGuard);

        randomNewEmployee.Known.KnowEverything(OwnerCharacter.TopEmployer);

        if (OwnerCharacter != null && OwnerCharacter.TopEmployer == CORE.PC)
        {
            AudioControl.Instance.PlayInPosition("property_recruit",transform.position);

            SelectedPanelUI.Instance.Select(this);

            if (isGuard)
            {
                CORE.Instance.InvokeEvent("GuardRecruited");
            }
            else
            {
                CORE.Instance.InvokeEvent("AgentRecruited");
            }
        }

        return null;
    }


    public void RecruitEmployeeForce(bool isGuard = false)
    {
        
        Character randomNewEmployee = CORE.Instance.GenerateCharacter(
               CurrentProperty.RecruitingGenderType,
               CurrentProperty.MinAge,
               CurrentProperty.MaxAge,
               this);

        randomNewEmployee.StartWorkingFor(this, isGuard);

        randomNewEmployee.Known.KnowEverything(OwnerCharacter.TopEmployer);

        if (OwnerCharacter != null && OwnerCharacter.TopEmployer == CORE.PC)
        {
            SelectedPanelUI.Instance.Select(this);

            if (isGuard)
            {
                CORE.Instance.InvokeEvent("GuardRecruited");
            }
            else
            {
                CORE.Instance.InvokeEvent("AgentRecruited");
            }
        }
        
    }

    public FailReason PurchasePlot(Character funder, Character forCharacter)
    {



        if (funder.Gold < LandValue)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                    "(You need more " + (LandValue - funder.Gold) + ")", 1f, Color.red);
            }

            return new FailReason("Not Enough Gold");
        }

        if (OwnerCharacter != null)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("LOCATION UNAVAILABLE ", 1f, Color.red);
            }

            return new FailReason("Location Unavailable");
        }

        this.SetInfo(Util.GenerateUniqueID(), CurrentProperty, true);

        forCharacter.StartOwningLocation(this);

        funder.Gold -= LandValue;

        if (funder.CurrentFaction == CORE.PC.CurrentFaction)
        {
            AudioControl.Instance.PlayInPosition("purchase",transform.position);

            CORE.Instance.ShowHoverMessage(string.Format("{0:n0}", LandValue.ToString()), ResourcesLoader.Instance.GetSprite("pay_money"), transform);

            RebrandWindowUI.Instance.Show(this);
        }



        forCharacter.DynamicRelationsModifiers.Add
        (
        new DynamicRelationsModifier(
        new RelationsModifier("Purchased a property for me!", 5)
        , 10
        , funder)
        );


        return null;
    }

    public FailReason BuyoutPlot(Character funder, Character forCharacter)
    {

        if (funder.Gold < LandValue*2)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                    "(You need more " + (LandValue - funder.Gold) + ")", 1f, Color.red);
            }

            return new FailReason("Not Enough Gold");
        }

        if (CurrentProperty.PlotType == CORE.Instance.Database.UniquePlotType)
        {
            if (funder == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("LOCATION UNAVAILABLE ", 1f, Color.red);
            }

            return new FailReason("Location Unavailable");
        }

        this.SetInfo(Util.GenerateUniqueID(), CurrentProperty, true);

        forCharacter.StartOwningLocation(this);

        funder.Gold -= LandValue*2;

        if (funder.CurrentFaction == CORE.PC.CurrentFaction)
        {
            CORE.Instance.ShowHoverMessage(string.Format("{0:n0}", (LandValue*2).ToString()), ResourcesLoader.Instance.GetSprite("pay_money"), transform);
        }



        forCharacter.DynamicRelationsModifiers.Add
        (
        new DynamicRelationsModifier(
        new RelationsModifier("Purchased a property for me!", 5)
        , 10
        , funder)
        );

        return null;
    }

    public enum VisibilityStateEnum
    {
        Hidden,QuestionMark,Visible
    }
}
