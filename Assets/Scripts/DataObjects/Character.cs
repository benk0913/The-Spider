using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Character", menuName = "DataObjects/Character", order = 2)]
public class Character : ScriptableObject, ISaveFileCompatible
{
    #region Stats

    public string ID;


    public Character Employer
    {
        get
        {
            if (WorkLocation == null)
            {
                return null;
            }

            if(WorkLocation.OwnerCharacter == this)
            {
                return null;
            }

            return WorkLocation.OwnerCharacter;
        }
    }

    public Character TopEmployer
    {
        get
        {
            if (Employer == null)
            {
                return this;
            }

            return Employer.TopEmployer;
        }
    }

    public int Gold
    {
        set
        {
            
            int earnings = value - _gold;
            
            if (Employer != null)
            {
                if (earnings < 2)
                {
                    Employer.Gold += earnings;
                    earnings = 0;
                }
                else
                {
                    Employer.Gold += earnings / 2;
                    earnings /= 2;
                }
            }


            _gold += earnings;
        }
        get
        {
            return _gold;
        }
    }
    [SerializeField]
    int _gold;


    public Faction CurrentFaction
    {
        get
        {
            if (Employer == null)
            {
                return _currentFaction != null? _currentFaction : CORE.Instance.Database.DefaultFaction;
            }

            return Employer.CurrentFaction;
        }
        set
        {
            _currentFaction = value;
        }
    }
    [SerializeField]
    Faction _currentFaction;

    public LocationEntity WorkLocation;

    public LocationEntity CurrentLocation;

    public List<LocationEntity> PropertiesOwned = new List<LocationEntity>();

    public LongTermTaskEntity CurrentTaskEntity
    {
        get
        {
            return _currentTaskEntity;
        }
        set
        {
            _currentTaskEntity = value;
            StateChanged.Invoke();
        }
    }
    LongTermTaskEntity _currentTaskEntity;

    public string CurrentRole
    {
        get
        {
            return WorkLocation != null ? WorkLocation.CurrentProperty.EmployeeRole : "";
        }
    }
    
    #region Traits & Bonuses

    /// <summary>
    /// DO NOT EVER SET (PUBLIC FOR SERIALIZATION)
    /// </summary>
    public List<Trait> Traits = new List<Trait>();

    public void AddTrait(Trait trait)
    {
        Trait nextTrait = trait.NextTrait; //-- Upgrade already upgraded traits.
        while(nextTrait != null)
        {
            if(Traits.Contains(nextTrait))
            {
                if(nextTrait.NextTrait != null)
                {
                    Trait tempTrait = nextTrait.NextTrait;
                    Traits.Remove(nextTrait);
                    Traits.Add(tempTrait);
                }

                return;
            }

            nextTrait = nextTrait.NextTrait;
        }

        Trait previousTrait = trait.PreviousTrait; //-- Remove traits which are in lower rank of this.
        while (previousTrait != null)
        {
            if (Traits.Contains(previousTrait))
            {
                Traits.Remove(previousTrait);
            }

            previousTrait = previousTrait.PreviousTrait;
        }

        if (Traits.Contains(trait)) // -- Upgrade if trait exists and has upgrade.
        {
            Trait traitsNextTrait = trait.NextTrait;
            if (traitsNextTrait != null)
            {
                Traits.Remove(trait);
                AddTrait(traitsNextTrait);
            }
            return;
        }

        for (int i = 0; i < trait.OppositeTraits.Length; i++) // -- If opposite traits exist, degrade them or dispose of them.
        {
            if (Traits.Contains(trait.OppositeTraits[i]))
            {
                Trait traitsPreviousTrait = trait.OppositeTraits[i].PreviousTrait;

                if (traitsPreviousTrait != null)
                {
                    Traits.Remove(trait.OppositeTraits[i]);
                    AddTrait(traitsPreviousTrait);
                }
                else
                {
                    Traits.Remove(trait.OppositeTraits[i]);
                }

                return;
            }
        }

        Traits.Add(trait);
        _bonuses = null;
    }

    public void RemoveTrait(Trait trait)
    {
        Traits.Remove(trait);
        _bonuses = null;
    }


    public List<Bonus> Bonuses
    {
        get
        {
            if (_bonuses == null)
            {
                List<Bonus> bonuses = new List<Bonus>();

                foreach (Trait trait in Traits)
                {
                    foreach (Bonus traitBonus in trait.Bonuses)
                    {
                        bool bonusExists = false;

                        foreach (Bonus existingBonus in bonuses)
                        {
                            if (existingBonus.Type == traitBonus.Type)
                            {
                                existingBonus.Value += traitBonus.Value;

                                bonusExists = true;
                                break;
                            }
                        }

                        if (bonusExists)
                        {
                            continue;
                        }

                        if (traitBonus.Value < 1)
                        {
                            continue;
                        }

                        Bonus bonus = new Bonus();
                        bonus.Type = traitBonus.Type;
                        bonus.Value = traitBonus.Value;

                        bonuses.Add(bonus);
                        
                    }
                }

                foreach(Bonus bonus in bonuses)
                {
                    if (bonus.Value < 1)
                    {
                        bonus.Value = 1;
                        continue;
                    }

                    if (bonus.Value == 1)
                    {
                        bonus.Value = 2;
                        continue;
                    }
                }

                _bonuses = bonuses;
            }

            return _bonuses;
        }
    }
    List<Bonus> _bonuses = null;

    public Bonus GetBonus(BonusType type)
    {
        List<Bonus> bonuses;

        if (_bonuses != null)
        {
            bonuses = this._bonuses;
        }
        else
        {
            bonuses = this.Bonuses;
        }

        for(int i=0;i<bonuses.Count;i++)
        {
            if(bonuses[i].Type == type)
            {
                return bonuses[i];
            }
        }


        //No bonus.
        Bonus bonus = new Bonus();
        bonus.Type  = type;
        bonus.Value = 1;

        return bonus;
    }

    public List<DynamicRelationsModifier> DynamicRelationsModifiers = new List<DynamicRelationsModifier>();

    #endregion

    #region Visual

    [SerializeField]
    public GenderSet VisualSet;

    public GenderType Gender
    {
        get
        {
            return gender;
        }
        set
        {
            if(gender == value)
            {
                return;
            }

            gender = value;

            RefreshVisualTree();
        }
    }
    [SerializeField]
    GenderType gender = GenderType.Female;

    public int Age
    {
        set
        {
            if(age == value)
            {
                return;
            }

            age = value;

            if(age < 4)
            {
                this.AgeType = AgeTypeEnum.Baby;
                RefreshVisualTree();
            }
            else if (age < 15)
            {
                this.AgeType = AgeTypeEnum.Child;
                RefreshVisualTree();
            }
            else if (age < 55)
            {
                this.AgeType = AgeTypeEnum.Adult;
                RefreshVisualTree();
            }
            else
            {
                this.AgeType = AgeTypeEnum.Old;
                RefreshVisualTree();
            }
        }
        get
        {
            return age;
        }
    }
    [SerializeField]
    int age = 16;

    [SerializeField]
    public AgeTypeEnum AgeType;

    public VisualCharacteristic SkinColor
    {
        get
        {
            return skinColor;
        }
        set
        {
            if (skinColor == value)
                return;

            skinColor = value;

            Face = skinColor.GetVCByName(Face.name);
            StateChanged.Invoke();
        }
    }
    [SerializeField]
    VisualCharacteristic skinColor;

    public VisualCharacteristic HairColor
    {
        get
        {
            return hairColor;
        }
        set
        {
            if(hairColor == value)
                return;

            hairColor = value;

            Hair = hairColor.GetVCByName(Hair.name);
            StateChanged.Invoke();
        }
    }
    [SerializeField]
    VisualCharacteristic hairColor;

    public VisualCharacteristic Face
    {
        get
        {
            return face;
        }
        set
        {
            face = value;
            StateChanged.Invoke();
        }
    }
    [SerializeField]
    VisualCharacteristic face;

    public VisualCharacteristic Hair
    {
        get
        {
            return hair;
        }
        set
        {
            hair = value;
            StateChanged.Invoke();
        }
    }
    [SerializeField]
    VisualCharacteristic hair;

    public VisualCharacteristic Clothing
    {
        get
        {
            return clothing;
        }
        set
        {
            clothing = value;
            StateChanged.Invoke();
        }
    }
    [SerializeField]
    VisualCharacteristic clothing;


    #endregion

    #endregion


    public UnityEvent StateChanged = new UnityEvent();

    #region Randomize

    public void Randomize()
    {
        this.Gender = (GenderType) Random.Range(0, 2);
        this.name = Names.GetRandomName(Gender);
        this.Age = Random.Range(5, 50);

        RandomizeLook();

        RandomizeTraits();
    }

    void RandomizeLook()
    {
        this.HairColor = VisualSet.HairColor.Pool[Random.Range(0, VisualSet.HairColor.Pool.Count)];
        this.Hair = HairColor.Pool[Random.Range(0, HairColor.Pool.Count)];
        this.SkinColor = VisualSet.SkinColors.Pool[Random.Range(0, VisualSet.SkinColors.Pool.Count)];
        this.Face = SkinColor.Pool[Random.Range(0, SkinColor.Pool.Count)];
        this.Clothing = VisualSet.Clothing.Pool[Random.Range(0, VisualSet.Clothing.Pool.Count)];
    }

    void RandomizeTraits()
    {
        this.Traits.Clear();
        Trait[] newTraits = CORE.Instance.Database.GetRandomTraits();

        foreach (Trait trait in newTraits)
        {
            AddTrait(trait);
        }
    }

    #endregion

    #region Misc

    public void Wipe()
    {
        RemoveListeners();
        Destroy(this);
    }

    void AddListeners()
    {
        RemoveListeners();
        GameClock.Instance.OnTurnPassed.AddListener(OnTurnPassedAI);
    }

    void RemoveListeners()
    {
        GameClock.Instance.OnTurnPassed.RemoveListener(OnTurnPassedAI);
        StateChanged.RemoveAllListeners();
    }

    public void RefreshVisualTree()
    {
        RaceSet raceSet = CORE.Instance.Database.GetRace("Human");

        AgeSet ageSet = CORE.Instance.Database.GetRace("Human").GetAgeSet(AgeTypeEnum.Adult);

        if (ageSet == null)
        {
            Debug.LogError("NO AGE SET! " + AgeTypeEnum.Adult.ToString());
            return;
        }


        VisualSet = (gender == GenderType.Male) ?
            CORE.Instance.Database.GetRace("Human").GetAgeSet(AgeType).Male : CORE.Instance.Database.GetRace("Human").GetAgeSet(AgeType).Female;
        

        if (VisualSet == null)
        {
            Debug.LogError("NO VISUAL SET! " + age + " | "  + gender.ToString() + " | " + AgeTypeEnum.Adult.ToString());
            return;
        }


        SkinColor = VisualSet.SkinColors.GetVCByName(SkinColor.name);

        HairColor = VisualSet.HairColor.GetVCByName(HairColor.name);

        if (WorkLocation != null && WorkLocation.CurrentAction.EmployeeUniform != null)
        {
            Clothing = Gender == GenderType.Male ? WorkLocation.CurrentAction.EmployeeUniform.MaleClothing : WorkLocation.CurrentAction.EmployeeUniform.FemaleClothing;
        }
        else
        {
            Clothing = VisualSet.Clothing.GetVCByName(Clothing.name);
        }

        StateChanged.Invoke();
    }

    public void Initialize(bool presetCharacter = false)
    {
        if (presetCharacter)
        {
            ID = this.name;
        }
        else
        {
            ID = Util.GenerateUniqueID();
        }

        RaceSet raceSet = CORE.Instance.Database.GetRace("Human");

        AgeSet ageSet = CORE.Instance.Database.GetRace("Human").GetAgeSet(AgeTypeEnum.Adult);

        if(ageSet == null)
        {
            Debug.LogError("NO AGE SET! "+AgeTypeEnum.Adult.ToString());
            return;
        }

        if (VisualSet == null)
        {
            VisualSet = Gender == GenderType.Male ?
                CORE.Instance.Database.GetRace("Human").GetAgeSet(AgeTypeEnum.Adult).Male :
                CORE.Instance.Database.GetRace("Human").GetAgeSet(AgeTypeEnum.Adult).Female;

            if (VisualSet == null)
            {
                Debug.LogError("NO VISUAL SET! " + AgeTypeEnum.Adult.ToString());
                return;
            }

            skinColor = VisualSet.SkinColors.Pool[0];
            hairColor = VisualSet.HairColor.Pool[0];

            Face = skinColor.Pool[0];
            Hair = hairColor.Pool[0];

            //TODO Add item support
            Clothing = VisualSet.Clothing.Pool[0];
        }

        RefreshVisualTree();

        AddListeners();

        GoToLocation(CORE.Instance.GetLocationOfProperty(CORE.Instance.Database.DefaultLocationProperty));
    }

    public int GetRelationsWith(Character otherCharacter)
    {
        int result = 0;

        RelationsModifier[] modifiers = GetRelationModifiers(otherCharacter);

        foreach(RelationsModifier modifier in modifiers)
        {
            result += modifier.Value;
        }

        return result;
    }

    public RelationsModifier[] GetRelationModifiers(Character otherCharacter)
    {
        List<RelationsModifier> modifiers = new List<RelationsModifier>();

        foreach(Trait trait in otherCharacter.Traits)
        {
            foreach(RelationsModifier traitModifier in trait.RelationModifiers)
            {
                modifiers.Add(traitModifier);
            }

            if(Traits.Contains(trait))
            {
                modifiers.Add(new RelationsModifier("Likes \"" + trait.name + "\"", 2));
                continue;
            }

            foreach (Trait oppositeTrait in trait.OppositeTraits)
            {
                if (Traits.Contains(oppositeTrait))
                {
                    modifiers.Add(new RelationsModifier("Hates \"" + trait.name +"\"", -2));
                    continue;
                }
            }
        }

        foreach(DynamicRelationsModifier dynamicMod in DynamicRelationsModifiers)
        {
            if(dynamicMod.ToCharacter == otherCharacter)
            {
                modifiers.Add(dynamicMod.Modifier);
            }
        }

        if(otherCharacter.Employer == this)
        {
            modifiers.Add(new RelationsModifier("My Employer", 1));
        }

        if (otherCharacter.Employer == this)
        {
            modifiers.Add(new RelationsModifier("My Employee", 1));
        }

        if (otherCharacter.Gender != this.Gender && otherCharacter.Age > 15 && this.Age > 15)
        {
            modifiers.Add(new RelationsModifier("Opposite Sex", 1));
        }

        if (otherCharacter.AgeType == this.AgeType)
        {
            modifiers.Add(new RelationsModifier("Same Age Range", 1));
        }

        float charmModifier = otherCharacter.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value;

        if (charmModifier> 1)
        {
            modifiers.Add(new RelationsModifier("Charming", Mathf.RoundToInt(charmModifier - 1)));
        }

        return modifiers.ToArray();
    }

    #endregion

    #region AI

    void OnTurnPassedAI()
    {
        for(int i=0;i<DynamicRelationsModifiers.Count;i++)
        {
            DynamicRelationsModifiers[i].Turns--;

            if(DynamicRelationsModifiers[i].Turns <= 0)
            {
                DynamicRelationsModifiers.RemoveAt(i);
                i--;
            }
        }

        if (this == CORE.PC)
        {
            return;
        }

        if (CurrentTaskEntity == null)
        {
            bool hasSomethingToDo = TryToDoSomething();
            
            if(!hasSomethingToDo)
            {
                GoToRandomLocation();
            }
        }
    }

    bool TryToDoSomething()
    {
        if ((int)GameClock.Instance.CurrentTimeOfDay < 3) //(Before evening?) - WORK
        {
            foreach (LocationEntity location in PropertiesOwned)
            {
                if (AttemptManagePropertyAI(location))
                {
                    return true;
                }
            }

            if (WorkLocation != null && WorkLocation.CurrentAction.WorkAction != null)
            {
                WorkLocation.CurrentAction.WorkAction.Execute(TopEmployer, this, WorkLocation);
                
                return true;
            }
        }

        // After evening? - Pass Time / Sleep
        return false;
    }

    void GoToRandomLocation()
    {
        if(CORE.Instance.Locations.Count == 0)
        {
            return;
        }

        GoToLocation(CORE.Instance.Locations[Random.Range(0,CORE.Instance.Locations.Count)]);
        return;
    }

    void GoToLocation(LocationEntity targetLocation)
    {
        CurrentLocation = targetLocation;
    }

    bool AttemptManagePropertyAI(LocationEntity location)
    {
        if(location.AttemptRecruiting())
        {
            return true;
        }

        return false;
    }

    public void StartWorkingFor(LocationEntity location)
    {
        location.EmployeesCharacters.Add(this);

        WorkLocation = location;
        WorkLocation.RefreshState();

        foreach (LocationEntity ownedLocation in PropertiesOwned)
        {
            ownedLocation.RefreshState();
        }

        RefreshVisualTree();
    }

    public void StopWorkingFor(LocationEntity location)
    {
        if (!location.EmployeesCharacters.Contains(this))
        {
            return;
        }

        location.EmployeesCharacters.Remove(this);

        LocationEntity tempLocation = WorkLocation;
        WorkLocation = null;
        tempLocation.RefreshState();

        foreach(LocationEntity ownedLocation in PropertiesOwned)
        {
            ownedLocation.RefreshState();
        }

        RefreshVisualTree();
    }

    public void StartOwningLocation(LocationEntity location)
    {
        if(location.OwnerCharacter != null)
        {
            location.OwnerCharacter.StopOwningLocation(location);
        }

        location.OwnerCharacter = this;
        location.RefreshState();

        if(!PropertiesOwned.Contains(location))
        {
            PropertiesOwned.Add(location);
        }

    }

    public void StopOwningLocation(LocationEntity location)
    {
        location.OwnerCharacter = null;
        location.RefreshState();

        if (PropertiesOwned.Contains(location))
        {
            PropertiesOwned.Remove(location);
        }

        GoToRandomLocation();        
    }

    public bool StartDoingTask(LongTermTaskEntity task)
    {
        if(task == CurrentTaskEntity)
        {
            return true;
        }

        if(CurrentTaskEntity != null)
        {
            if (!CurrentTaskEntity.Cancel())
            {
                return false;
            }
        }
       
        CurrentTaskEntity = task;
        return true;
    }

    public bool StopDoingCurrentTask(bool forced = false)
    {
        if (CurrentTaskEntity == null)
        {
            return true;
        }

        if (!forced && !CurrentTaskEntity.CurrentTask.Cancelable)
        {
            GlobalMessagePrompterUI.Instance.Show(this.name + " Cannot Stop " + CurrentTaskEntity.CurrentTask.name, 1f, Color.red);
            return false;
        }

        CurrentTaskEntity = null;

        return true;
    }

    public void ForceTask(LongTermTaskEntity task)
    {
        CurrentTaskEntity.Cancel();
        StartDoingTask(task);
    }

    public void Death()
    {
        if (WorkLocation != null)
        {
            StopWorkingFor(WorkLocation);
        }

        while(PropertiesOwned.Count > 0)
        {
            StopOwningLocation(PropertiesOwned[0]);
        }

        StopDoingCurrentTask();
        
        RemoveListeners();

        CORE.Instance.Characters.Remove(this);
    }

    #endregion

    #region Save & Load

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["ID"] = ID;

        node["name"] = name;

        node["Gender"] = ((int)Gender).ToString();

        node["Gold"] = _gold.ToString();

        node["WorkLocation"] = WorkLocation == null ? "" : WorkLocation.ID;

        node["CurrentLocation"] = CurrentLocation == null ? "" : CurrentLocation.ID;

        node["CurrentFaction"] = CurrentFaction.name;

        for(int i=0;i<PropertiesOwned.Count;i++)
        {
            node["PropertiesOwned"][i] = PropertiesOwned[i].ID;
        }

        for(int i=0;i<Traits.Count;i++)
        {
            node["Traits"][i] = Traits[i].name;
        }

        for(int i=0;i<DynamicRelationsModifiers.Count;i++)
        {
            node["DynamicRelationsModifiers"][i] = DynamicRelationsModifiers[i].ToJSON();
        }

        node["age"] = Age.ToString();
        node["skinColor"] = SkinColor.name;
        node["hairColor"] = HairColor.name;
        node["face"] = Face.name;
        node["hair"] = Hair.name;
        node["clothing"] = Clothing.name;

        if(CurrentTaskEntity != null)
        {
            node["CurrentTaskEntityCurrentTask"] = CurrentTaskEntity.CurrentTask.name;
            node["CurrentTaskEntityCurrentRequester"] = CurrentTaskEntity.CurrentRequester.ID;
            node["CurrentTaskEntityCurrentCharacter"] = CurrentTaskEntity.CurrentCharacter.ID;
            node["CurrentTaskEntityCurrentTarget"] = ((LocationEntity)CurrentTaskEntity.CurrentTargetLocation).ID;
            node["CurrentTaskEntityCurrentTargetCharacter"] = CurrentTaskEntity.TargetCharacter != null ? CurrentTaskEntity.TargetCharacter.ID : "";
            node["CurrentTaskEntityTurnsLeft"] = CurrentTaskEntity.TurnsLeft.ToString();
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        Initialize();

        ID = node["ID"];

        name = node["name"];

        Gender = (GenderType)int.Parse(node["Gender"]);

        _gold = int.Parse(node["Gold"]);

        _workLocationID = node["WorkLocation"];

        _currentLocationID = node["CurrentLocation"];

        CurrentFaction = CORE.Instance.Database.GetFactionByName(node["CurrentFaction"]);

        _propertiesOwnedIDs = new string[node["PropertiesOwned"].Count];
        for (int i = 0; i < node["PropertiesOwned"].Count; i++)
        {
            _propertiesOwnedIDs[i] = node["PropertiesOwned"][i];
        }

        for (int i = 0; i < node["Traits"].Count; i++)
        {
            AddTrait(CORE.Instance.Database.GetTrait(node["Traits"][i]));
        }

        for (int i = 0; i < node["DynamicRelationsModifiers"].Count; i++)
        {
            DynamicRelationsModifier modifier = new DynamicRelationsModifier();

            modifier.FromJSON(node["DynamicRelationsModifiers"][i]);

            DynamicRelationsModifiers.Add(modifier);
        }

        Age = int.Parse(node["age"]);
        SkinColor = VisualSet.GetVCByName(node["skinColor"]);
        HairColor = VisualSet.GetVCByName(node["hairColor"]);
        Face = SkinColor.GetVCByName(node["face"]);
        Hair = HairColor.GetVCByName(node["hair"]);
        clothing = VisualSet.GetVCByName(node["clothing"]);

        if (!string.IsNullOrEmpty(node["CurrentTaskEntityCurrentTask"]))
        {
            _currentTaskTurnsLeft = int.Parse(node["CurrentTaskEntityTurnsLeft"]);
            _currentTaskName = node["CurrentTaskEntityCurrentTask"];
            _currentTaskRequesterID = node["CurrentTaskEntityCurrentRequester"];
            _currentTaskCharacterID = node["CurrentTaskEntityCurrentCharacter"];
            _currentTaskTargetID = node["CurrentTaskEntityCurrentTarget"];
            _currentTaskTargetCharacterID = node["CurrentTaskEntityCurrentTargetCharacter"];
        }
    }



    string _workLocationID;
    string[] _propertiesOwnedIDs;
    string _currentLocationID;

    string _currentTaskName;
    int    _currentTaskTurnsLeft;
    string _currentTaskRequesterID;
    string _currentTaskCharacterID;
    string _currentTaskTargetCharacterID;
    string _currentTaskTargetID;

    public void ImplementIDs()
    {
        if (!string.IsNullOrEmpty(_workLocationID))
        {
            StartWorkingFor(CORE.Instance.GetLocationByID(_workLocationID));
        }

        if (!string.IsNullOrEmpty(_currentLocationID))
        {
            GoToLocation(CORE.Instance.GetLocationByID(_currentLocationID));
        }

        if (_propertiesOwnedIDs != null)
        {
            foreach (string proeprtyID in _propertiesOwnedIDs)
            {
                StartOwningLocation(CORE.Instance.GetLocationByID(proeprtyID));
            }
        }

        if (!string.IsNullOrEmpty(_currentTaskName))
        {
            CORE.Instance.GenerateLongTermTask(
                CORE.Instance.Database.GetLongTermTaskByName(_currentTaskName),
                CORE.Instance.GetCharacterByID(_currentTaskRequesterID),
                CORE.Instance.GetCharacterByID(_currentTaskCharacterID),
                CORE.Instance.GetLocationByID(_currentTaskTargetID),
                CORE.Instance.GetCharacterByID(_currentTaskTargetCharacterID),
                _currentTaskTurnsLeft);
        }
    }

    #endregion

}

public class DynamicRelationsModifier : ISaveFileCompatible
{
    public RelationsModifier Modifier;
    public int Turns;
    public Character ToCharacter;

    public DynamicRelationsModifier()
    {
    }

    public DynamicRelationsModifier(RelationsModifier modifier, int turns, Character toCharacter)
    {
        this.Modifier = modifier;
        this.Turns = turns;
        this.ToCharacter = toCharacter;
    }

    string toCharacterID;

    public void FromJSON(JSONNode node)
    {
        RelationsModifier modifier = new RelationsModifier();
        modifier.FromJSON(node["Modifier"]);
        Modifier = modifier;

        Turns = int.Parse(node["Turns"]);

        toCharacterID = node["ToCharacter"];
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["Modifier"] = Modifier.ToJSON();
        node["Turns"] = Turns.ToString();
        node["ToCharacter"] = ToCharacter.ID.ToString();

        return node;
    }

    public void ImplementIDs()
    {
        ToCharacter = CORE.Instance.GetCharacterByID(toCharacterID);
    }
}


[System.Serializable]
public class RelationsModifier : ISaveFileCompatible
{
    public string Message;
    public int Value;

    public RelationsModifier()
    {
    }

    public RelationsModifier(string message, int value)
    {
        //this.Message = (value > 0 ? "<color=green>" : "<color=red>") + message + " (" + value + ")</color>";
        this.Message =  message;
        this.Value = value;
    }

    public void FromJSON(JSONNode node)
    {
        Message = node["Message"];
        Value = int.Parse(node["Value"]);
    }

    public void ImplementIDs()
    {
       
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();
        node["Message"] = Message;
        node["Value"] = Value.ToString();

        return node;
    }
}
