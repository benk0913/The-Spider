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

    public List<Character> FiredEmployeees = new List<Character>();

    public int Level = 1;

    public bool IsUpgrading;

    public int CurrentUpgradeLength;

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

    public bool IsBuyable
    {
        get
        {
            return OwnerCharacter == null && CurrentProperty.PlotType != CORE.Instance.Database.UniquePlotType;
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

            charactersToAdd.Clear();
            while (CharactersLivingInLocation.Count > 0)
            {
                Character tempChar = CORE.Instance.GetCharacter(CharactersLivingInLocation[0].name);

                CharactersLivingInLocation.RemoveAt(0);

                if (tempChar == null)
                {
                    continue;
                }

                charactersToAdd.Add(tempChar);
            }
            foreach (Character character in charactersToAdd)
            {
                character.StartLivingIn(this);
            }

            charactersToAdd.Clear();
            while (CharactersInLocation.Count > 0)
            {
                Character tempChar = CORE.Instance.GetCharacter(CharactersInLocation[0].name);

                CharactersInLocation.RemoveAt(0);

                if (tempChar == null)
                {
                    continue;
                }

                charactersToAdd.Add(tempChar);
            }
            foreach (Character character in charactersToAdd)
            {
                character.GoToLocation(this);
            }
        }
    }

    public IEnumerator TurnPassed()
    {
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

    public void OnClick()
    {
        if (VisibilityState == VisibilityStateEnum.Visible)
        {
            SelectedPanelUI.Instance.Select(this);
        }
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
        else if (!Traits.Contains(CORE.Instance.Database.PublicAreaTrait)) //If not a district
        {
            NearestDistrict = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.PublicAreaTrait, this);
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
            tempFigure = Instantiate(CurrentProperty.FigurePrefab);

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

            hoverModel = Instantiate(CurrentProperty.HoverPrefab);
        }
        else if (VisibilityState == VisibilityStateEnum.QuestionMark) //If player has scouted the nearest district
        {
            tempFigure = Instantiate(CORE.Instance.Database.UnknownFigurePrefab);
            //tempFigure.GetComponent<FigureController>().SetMaterial(CORE.Instance.Database.DefaultFaction.WaxMaterial);
            hoverModel = Instantiate(CORE.Instance.Database.UnknownFigurePrefab);
        }
        else //Player didn't scout nearest district.
        {
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

        if (Level >= CurrentProperty.PropertyLevels.Count)
        {
            return new FailReason("Level Already Maxed Out");
        }

        if (funder.Gold < CurrentProperty.PropertyLevels[Level].UpgradePrice)
        {
            GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH GOLD! " +
                "(You need more " + (CurrentProperty.PropertyLevels[Level].UpgradePrice - funder.Gold)+")", 1f, Color.red);

            return new FailReason("Not Enough Gold", (CurrentProperty.PropertyLevels[Level].UpgradePrice - funder.Gold));
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
        if (requester != OwnerCharacter.TopEmployer)
        {
            if (requester == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("YOU DON'T OWN THIS PLACE!", 1f, Color.red);
            }

            return new FailReason("Do Not Own Property");
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

        while(EmployeesCharacters.Count > 0)
        {
            EmployeesCharacters[0].StopWorkingForCurrentLocation();
        }

        SetInfo(Util.GenerateUniqueID(), newProperty, false);

        if (requester == CORE.PC)
        {
            SelectedPanelUI.Instance.Select(this);
        }

        return null;
    }

    public void CharacterEnteredLocation(Character character)
    {
        CharactersInLocation.Add(character);

        if(OwnerCharacter != null && OwnerCharacter.TopEmployer == CORE.PC)
        {
            character.Known.Know("Appearance", character.TopEmployer);
            character.Known.Know("CurrentLocation", character.TopEmployer);
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

        if (CharactersInLocation.Count == 0 && CharactersInLocationUIInstance != null)
        {
            CharactersInLocationUIInstance.gameObject.SetActive(false);
            CharactersInLocationUIInstance = null;
            return;
        }

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
            CharactersInLocationUIInstance.transform.SetParent(CORE.Instance.MainCanvas.transform);
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
            TaskDurationUI.transform.SetParent(CORE.Instance.MainCanvas.transform);
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

        foreach (KnowledgeInstance item in Known.Items)
        {
            for (int i = 0; i < item.KnownByCharacters.Count; i++)
            {
                node["Knowledge"][item.Key][i] = item.KnownByCharacters[i].ID;
            }
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        Level = int.Parse(node["Level"]);
        SetInfo(node["ID"], CORE.Instance.Database.GetPropertyByName(node["CurrentProperty"]), true);
        IsUpgrading = bool.Parse(node["IsUpgrading"]);
        CurrentUpgradeLength = int.Parse(node["CurrentUpgradeLength"]);
        CurrentAction = CurrentProperty.GetActionByName(node["CurrentAction"]);
        

        transform.position = new Vector3(float.Parse(node["PositionX"]), float.Parse(node["PositionY"]), float.Parse(node["PositionZ"]));
        transform.rotation = Quaternion.Euler(float.Parse(node["RotationX"]), float.Parse(node["RotationY"]), float.Parse(node["RotationZ"]));
        
        foreach (KnowledgeInstance item in Known.Items)
        {
            List<string> IDs = new List<string>();
            for (int i = 0; i < node["Knowledge"][item.Key].Count; i++)
            {
                IDs.Add(node["Knowledge"][item.Key][i]);
            }

            knowledgeCharacterIDs.Add(node["Knowledge"][item.Key], IDs);
        }
    }

    Dictionary<string, List<string>> knowledgeCharacterIDs = new Dictionary<string, List<string>>();

    public void ImplementIDs()
    {

        foreach (string key in knowledgeCharacterIDs.Keys)
        {
            for (int i = 0; i < knowledgeCharacterIDs[key].Count; i++)
            {
                Character character = CORE.Instance.GetCharacterByID(knowledgeCharacterIDs[key][i]);

                if (character == null)
                {
                    continue;
                }

                Known.Know(key, character);
            }
        }

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

        if(TaskDurationUI != null)
        {
            TaskDurationUI.Wipe();
        }

        if(OwnerCharacter != null)
        {
            OwnerCharacter.StopOwningLocation(this);
        }

        Destroy(this.gameObject);
    }

    public FailReason RecruitEmployee(Character requester)
    {
        if(requester != OwnerCharacter.TopEmployer)
        {
            if (requester == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("You do not own this proeprty!", 1f, Color.red);
            }

            return new FailReason("Do Not Own Property");
        }

        if (requester.Connections < 3) //TODO replace magic number...
        {
            if (requester == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("Need more connections (" + requester.Connections + "/" + 3.ToString(), 1f, Color.red);
            }

            return new FailReason("Not Enough Connections",3);
        }

        requester.Connections -= 3;

        Character randomNewEmployee = CORE.Instance.GenerateCharacter(
               CurrentProperty.RecruitingGenderType,
               CurrentProperty.MinAge,
               CurrentProperty.MaxAge);

        randomNewEmployee.Known.KnowEverything(OwnerCharacter.TopEmployer);

        randomNewEmployee.StartWorkingFor(this);

        if (OwnerCharacter != null && OwnerCharacter.TopEmployer == CORE.PC)
        {
            SelectedPanelUI.Instance.Select(this);
        }

        return null;
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
            CORE.Instance.ShowHoverMessage(string.Format("{0:n0}", LandValue.ToString()), ResourcesLoader.Instance.GetSprite("pay_money"), transform);
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
