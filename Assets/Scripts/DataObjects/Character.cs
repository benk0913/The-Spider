using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Character", menuName = "DataObjects/Character", order = 2)]
public class Character : ScriptableObject, ISaveFileCompatible
{
    #region Stats

    public string ID;

    public bool IsAlwaysKnown = false;

    public bool IsDead = false;

    public bool IsAgent
    {
        get
        {
            return WorkLocation != null && WorkLocation.CurrentProperty.EmployeesAreAgents && !IsGuard;
        }
    }

    public bool IsGuard
    {
        get
        {
            return WorkLocation != null && WorkLocation.GuardsCharacters.Contains(this);
        }
    }

    public bool IsInTrouble
    {
        get
        {
            return PrisonLocation != null;
        }
    }

    public bool IsInHiding
    {
        get
        {
            return CurrentTaskEntity != null && CurrentTaskEntity.CurrentTask.name == "In Hiding";
        }
    }

    public bool isKnownOnStart
    {
        get
        {
            return _isknownonstart || this == CORE.PC || CORE.PC == this.TopEmployer;
        }
        set
        {
            _isknownonstart = value;
        }
    }
    private bool _isknownonstart;

    public bool isImportant
    {
        get
        {
            return (Pinned || Employer == CORE.PC);
        }
    }

    public Character Employer
    {
        get
        {
            if (WorkLocation == null)
            {
                return null;
            }

            if (WorkLocation.OwnerCharacter == this)
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

    public int Rank
    {
        get
        {
            if(Employer == null)
            {
                return 0;
            }

            if(Employer == this)
            {
                return 0;
            }

            int rankCount = 0;

            Character CurrentEmployer = Employer;
            while (CurrentEmployer != Employer.TopEmployer)
            {   
                rankCount++;
                CurrentEmployer = CurrentEmployer.Employer;
            }

            return rankCount;
        }
    }

    public int Reputation
    {
        get
        {
            return _reputation;
        }
        set
        {
            if (value < CORE.Instance.Database.ReputationMin)
            {
                _reputation = CORE.Instance.Database.ReputationMin;
            }
            else if (value > CORE.Instance.Database.ReputationMax)
            {
                _reputation = CORE.Instance.Database.ReputationMax;
            }
            else
            {
                _reputation = value;
            }

            if(this == CORE.PC)
            {
                StatsViewUI.Instance.RefreshReputation();
            }
        }
    }
    int _reputation;


    public int CGold
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;

            if (_gold < 0)
            {
                _gold = 0;
            }
        }
    }


    [FormerlySerializedAs("Gold")]
    public int _gold;

    public int CConnections
    {
        get
        {
            return _connections;
        }
        set
        {
            _connections = value;

            if (_connections < 0)
            {
                _connections = 0;
            }
        }
    }

    [FormerlySerializedAs("Connections")]
    public int _connections;

    public int CRumors
    {
        get
        {
            return _rumors;
        }
        set
        {
            _rumors = value;

            if (_rumors < 0)
            {
                _rumors = 0;
            }
        }
    }

    [FormerlySerializedAs("Rumors")]
    public int _rumors;


    public int CProgress
    {
        get
        {
            return _progress;
        }
        set
        {
            _progress = value;

            if (_progress < 0)
            {
                _progress = 0;
            }
        }
    }

    [FormerlySerializedAs("Progress")]
    public int _progress;

    public bool IsDisabled = false;

    public bool NeverDED = false;

    public Faction CurrentFaction
    {
        get
        {
            if (Employer == null)
            {
                return _currentFaction != null? _currentFaction : CORE.Instance.Factions.Find(x=>x.name == CORE.Instance.Database.DefaultFaction.name);
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

    public FactionAI AI;

    public LocationEntity WorkLocation;

    public LocationEntity PrisonLocation;

    public LocationEntity HomeLocation;

    public LocationEntity CurrentLocation;

    public Faction PuppetOf = null;
    public int PupperTurnsLeft;

    public bool Pinned
    {
        get
        {
            return _pinned;
        }
        set
        {
            _pinned = value;
        }
    }
    bool _pinned;

    public List<LocationEntity> PropertiesInCommand
    {
        get
        {
            List<LocationEntity> properties = new List<LocationEntity>();

            properties.AddRange(PropertiesOwned);

            foreach(LocationEntity property in PropertiesOwned)
            {
                foreach(Character character in property.EmployeesCharacters)
                {
                    properties.AddRange(character.PropertiesInCommand);
                }
            }

            return properties;
        }
    }

    public List<Character> CharactersInCommand
    {
        get
        {
            List<Character> characters = new List<Character>();

            foreach (LocationEntity property in PropertiesOwned)
            {
                foreach (Character character in property.EmployeesCharacters)
                {
                    characters.Add(character);
                    characters.AddRange(character.CharactersInCommand);
                }

                foreach (Character character in property.GuardsCharacters)
                {
                    characters.Add(character);
                    characters.AddRange(character.CharactersInCommand);
                }

            }

            return characters;
        }
    }

    public List<Character> GuardsInCommand
    {
        get
        {
            return CharactersInCommand.FindAll(x => x.IsGuard);
        }
    }

    public Character RandomAgent
    {
        get
        {
            List<Character> characters = CharactersInCommand.FindAll(x => x.IsAgent);

            if (characters == null || characters.Count == 0)
            {
                return null;
            }

            return characters[Random.Range(0, characters.Count)];
        }
    }

    public List<LocationEntity> PropertiesOwned = new List<LocationEntity>();

    public List<ForgeryCaseElement> CaseElements = new List<ForgeryCaseElement>();

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

    public QuestioningInstance CurrentQuestioningInstance = null;
    
    #region Traits & Bonuses

    public int TotalBonusScore
    {
        get
        {
            return Mathf.RoundToInt(GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value
            + GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value
            + GetBonus(CORE.Instance.Database.GetBonusType("Intelligent")).Value
            + GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value
            + GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value
            + GetBonus(CORE.Instance.Database.GetBonusType("Menacing")).Value
            + GetBonus(CORE.Instance.Database.GetBonusType("Stealthy")).Value);
        }
    }

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

                        Bonus bonus = new Bonus();
                        bonus.Type = traitBonus.Type;
                        bonus.Value = 1+traitBonus.Value;

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

    [SerializeField]
    public Sprite UniquePortrait;

    #endregion

    #region Items

    public List<Item> Belogings = new List<Item>();

    public Item GetItem(string itemKey)
    {
        foreach (Item item in Belogings)
        {
            if (item.name == itemKey)
            {
                return item;
            }
        }

        return null;
    }

    #endregion

    #region Favor 

    public List<FavorPointsPair> FavorPoints = new List<FavorPointsPair> ();

    public int GetFavorPoints(Character character)
    {
        FavorPointsPair Pair = null;

        Pair = FavorPoints.Find(x => x.Key == character);

        if(Pair == null)
        {
            Pair = new FavorPointsPair(character);
            FavorPoints.Add(Pair);
        }

        return Pair.Value;
    }

    public void AddFavorPoints(Character character, int value)
    {
        FavorPointsPair Pair = null;

        Pair = FavorPoints.Find(x => x.Key == character);

        if (Pair.Key == null)
        {
            Pair = new FavorPointsPair(character, 0);
            FavorPoints.Add(Pair);
        }

        Pair.Value += value;
    }

    public int FavorPointGoldPrice(Character withCharacter)
    {
        return Mathf.RoundToInt(Mathf.Max(
            10
            ,
            (100) - (GetRelationsWith(withCharacter) * 5f)));
    }

    public int FavorPointRumorsPrice(Character withCharacter)
    {
        return 100;
    }

    public int FavorPointConnectionsPrice(Character withCharacter)
    {
        return 100;
    }


    #endregion

    #region Known / Information


    public Knowledge Known;

    public List<KnowledgeRumor> KnowledgeRumors = new List<KnowledgeRumor>();

    public List<KnowledgeRumor> InformationSold = new List<KnowledgeRumor>();

    public bool IsKnown(string itemKey, Character byCharacter)
    {
        if(Known == null)
        {
            return true;
        }

        foreach(KnowledgeInstance item in Known.Items)
        {
            if(item.Key == itemKey)
            {
                return item.IsKnownByCharacter(byCharacter);
            }
        }

        return false;
    }

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
        Destroy(this);
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

        if (WorkLocation != null && (WorkLocation.EmployeesCharacters.Contains(this) && WorkLocation.CurrentAction.EmployeeUniform != null))
        {
            Clothing = Gender == GenderType.Male ? WorkLocation.CurrentAction.EmployeeUniform.MaleClothing : WorkLocation.CurrentAction.EmployeeUniform.FemaleClothing;
        }
        else if (WorkLocation != null && (WorkLocation.GuardsCharacters.Contains(this) && WorkLocation.CurrentAction.GuardUniform != null))
        {
            Clothing = Gender == GenderType.Male ? WorkLocation.CurrentAction.GuardUniform.MaleClothing : WorkLocation.CurrentAction.GuardUniform.FemaleClothing;
        }
        else
        {
            if (Clothing == null)
            {
                Clothing = VisualSet.Clothing.Pool[0];
            }
            else
            {
                Clothing = VisualSet.Clothing.GetVCByName(Clothing.name);
            }
        }

        StateChanged.Invoke();
    }

    public void Initialize(bool presetCharacter = false)
    {
        if (presetCharacter)
        {
            ID = this.name;

            List<Item> newBelogings = new List<Item>();
            foreach(Item item in Belogings)
            {
                newBelogings.Add(item.Clone());
            }

            this.Belogings = newBelogings;
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


        Known = new Knowledge(this);

        if (isKnownOnStart)
        {
            Known.KnowEverything(TopEmployer);
        }

        if(CurrentFaction.FactionHead != null && CurrentFaction.FactionHead.name == name)
        {
            AI = Instantiate(CurrentFaction.AI);
        }
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

        ReputationInstance otherCharacterReputation = CORE.Instance.Database.GetReputationType(otherCharacter.Reputation);
        modifiers.Add(new RelationsModifier(otherCharacterReputation.name,otherCharacterReputation.AgentRelationModifier));

        foreach (Trait trait in otherCharacter.Traits)
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

        if(this.Employer == otherCharacter)
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

    public bool IsPuppetOf(Faction faction)
    {
        faction = CORE.Instance.Factions.Find(x => x.name == faction.name);

        if (PuppetOf != null || Employer == this || Employer == null)
        {
            return (PuppetOf == faction);
        }

        return Employer.IsPuppetOf(faction);
    }

    public void DisableCharacter()
    {
        IsDisabled = true;
    }

    public void EnableCharacter()
    {
        IsDisabled = false;
    }

    #endregion

    #region AI

    public void OnTurnPassedAI()
    {
        if(IsDisabled)
        {
            return;
        }

        if (PuppetOf != null)
        {
            PupperTurnsLeft--;
            if (PupperTurnsLeft <= 0)
            {
                PuppetOf = null;
            }
        }

        for(int i=0;i<DynamicRelationsModifiers.Count;i++)
        {
            DynamicRelationsModifiers[i].Turns--;

            if(DynamicRelationsModifiers[i].Turns <= 0)
            {
                DynamicRelationsModifiers.RemoveAt(i);
                i--;
            }
        }

        if (CurrentFaction.FactionHead != null && this.name == CurrentFaction.FactionHead.name)//Faction Head
        {
            return;
        }

        if (PrisonLocation != null) // Imprisoned
        {
            return;
        }

        if (CurrentTaskEntity == null)
        {
            bool hasSomethingToDo = TryToDoSomething();
            
            if(!hasSomethingToDo)
            {
                GoToLocation(HomeLocation);
            }
        }

        AttemptPersonalMotives();
    }

    void AttemptPersonalMotives()
    {
        if(TopEmployer != CORE.PC)
        {
            return;
        }

        if (CurrentFaction.HasPromotionSystem)
        {
            if(IsGuard)
            {
                return;
            }

            int total = 0;
            Bonuses.ForEach((x) => 
            {
                total += Mathf.RoundToInt(x.Value);
                total--;
            });

            total /= 5;
            total++;

            CProgress += total;

            if(CProgress >= 50)
            {
                PopupData popup = new PopupData(CORE.Instance.Database.AllPopupPresets.Find(x => x.name == "Promotion"), new List<Character> { this }, new List<Character> { this.Employer });
                PopupWindowUI.Instance.AddPopup(popup);

                TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": has been promoted!", ResourcesLoader.Instance.GetSprite("thumb-up"), this));

                if (Employer == CORE.PC)
                {
                    WarningWindowUI.Instance.Show(this.name + " has been promoted to replace YOU. Keep your agent's in check next time.", () => { LoseWindowUI.Instance.Show(); }, true);
                    return;
                }
                else if(this == CORE.PC || Employer == this)
                {
                    return;
                }
                else
                {
                    LocationEntity previousWorkLocation = WorkLocation;
                    Character previousEmployer = Employer;
                    StopWorkingForCurrentLocation();
                    StartWorkingFor(previousEmployer.WorkLocation);

                    previousWorkLocation.RecruitEmployee(TopEmployer);

                    if (previousEmployer == null)
                    {
                        return;
                    }

                    List<LocationEntity> Inheritence = new List<LocationEntity>();
                    Inheritence.AddRange(previousEmployer.PropertiesOwned);
                    foreach(LocationEntity location in Inheritence)
                    {
                        StartOwningLocation(location);
                    }

                    previousEmployer.StopDoingCurrentTask();
                    previousEmployer.StopWorkingForCurrentLocation();
                }
            }

            if(GetRelationsWith(TopEmployer) < -20)
            {
                if(Random.Range(0,5) == 0)
                {
                    LeaveFaction();
                }
            }
            else if(GetRelationsWith(TopEmployer) < -5)
            {
                if (Random.Range(0,3) == 0)
                {
                    
                    StopDoingCurrentTask();
                    CORE.Instance.Database.SlackOfAction.Execute(TopEmployer, this, this.CurrentLocation);
                    
                }
            }
        }
        else
        {
            AttemptBetrayEmployer();
        }
    }

    void LeaveFaction()
    {
        WarningWindowUI.Instance.Show(this.name+" has left your faction.",null);

        while(PropertiesOwned.Count > 0)
        {
            StopOwningLocation(PropertiesOwned[0], true);
        }

        StopWorkingForCurrentLocation();
    }

    void AttemptBetrayEmployer()
    {
        if (TopEmployer == this)
        {
            return;
        }

        if(TopEmployer != CORE.PC)
        {
            return;
        }

        if(Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")))
        {
            return;
        }

        if(Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))
        {
            return;
        }

        if(Random.Range(0, 5) == 0)
        {
            return;
        }

        if(PropertiesInCommand.Count < 3)
        {
            return;
        }

        if (PropertiesInCommand.Count < (TopEmployer.PropertiesInCommand.Count / 2))
        {
            return;
        }

        int minRelationToBetray = 5;
        if (Traits.Contains(CORE.Instance.Database.GetTrait("Bad Moral Standards")))
        {
            minRelationToBetray = 10;
        }
        else if (Traits.Contains(CORE.Instance.Database.GetTrait("Evil")))
        {
            minRelationToBetray = 9999;
        }
        
        if(GetRelationsWith(TopEmployer) >= minRelationToBetray)
        {
            return;
        }

        BetrayEmployer();
    }

    public void BetrayEmployer()
    {
        Faction previousFaction = this.TopEmployer.CurrentFaction;

        if (this.TopEmployer == CORE.PC && PropertiesOwned.Count > 0)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": has stabbed you in the back!", ResourcesLoader.Instance.GetSprite("thumb-down"), this));

            string deathString = this.name + " has grown too strong and decided to betray you! Lost ownership on:";

            PropertiesOwned.ForEach((x) => { deathString += x.Name + " - "; });

            PopupData popup = new PopupData(CORE.Instance.Database.AllPopupPresets.Find(x => x.name == "Betrayal"), new List<Character> { this }, new List<Character> { this.TopEmployer },
                ()=> 
                {
                    WarningWindowUI.Instance.Show(deathString, () => 
                    {
                        TutorialScreenUI.Instance.Show("FirstBetray");
                    });
                });

            PopupWindowUI.Instance.AddPopup(popup);
            
        }

        LetterPreset letter = CORE.Instance.Database.BetrayalLetter.CreateClone();
        Dictionary<string, object> letterParameters = new Dictionary<string, object>();

        letterParameters.Add("Letter_From", this);
        letterParameters.Add("Letter_To", TopEmployer);

        LetterDispenserEntity.Instance.DispenseLetter(new Letter(letter, letterParameters));

        StopWorkingForCurrentLocation();

        //Branch to a new faction
        this.CurrentFaction = previousFaction.Clone();
        this.CurrentFaction.FactionHead = this;
        this.CurrentFaction.name = this.name + "'s - Gang";
        this.CurrentFaction.Icon = CORE.Instance.Database.NoFaction.Icon;
        this.AI = this.CurrentFaction.AI;
        this.CurrentFaction.FactionColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        this.CurrentFaction.WaxMaterial = new Material(this.CurrentFaction.WaxMaterial);
        this.CurrentFaction.WaxMaterial.color = this.CurrentFaction.FactionColor;
        CORE.Instance.Factions.Add(this.CurrentFaction);
    }

    public bool TryToDoSomething()
    {
        if(IsDisabled)
        {
            return false;
        }

        if (CurrentFaction.FactionHead != null && this.name == CurrentFaction.FactionHead.name)//Faction Head
        {
            return false;
        }

        if (PrisonLocation != null) // Imprisoned
        {
            return false;
        }

        if (CurrentTaskEntity != null)
        {
        }


        if (IsGuard && Random.Range(0,2) == 0) // Guards keep working
        {
            CORE.Instance.Database.GetAgentAction("Guard Location").Execute(TopEmployer, this, WorkLocation);
            return true;
        }

        //if (GameClock.Instance.CurrentTimeOfDay == GameClock.GameTime.Morning 
        //    || GameClock.Instance.CurrentTimeOfDay == GameClock.GameTime.Noon 
        //    || GameClock.Instance.CurrentTimeOfDay == GameClock.GameTime.Afternoon)
        //{

            if (WorkLocation != null && WorkLocation.CurrentAction.WorkAction != null)
            {
                if (WorkLocation.EmployeesCharacters.Contains(this))
                {
                    WorkLocation.CurrentAction.WorkAction.Execute(TopEmployer, this, WorkLocation);
                }

                return true;
            }
        //}
        //else if (GameClock.Instance.CurrentTimeOfDay == GameClock.GameTime.Night)
        //{
        //    if (HomeLocation != null)
        //    {
        //        CORE.Instance.Database.SleepAction.Execute(this, this, HomeLocation);

        //        return true;
        //    }
        //}

        //Hangoutime!
        LocationEntity hangoutLocation = null;

        if(Traits.Contains(CORE.Instance.Database.GetTrait("Drunkard")) )
        {
            hangoutLocation = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.RumorsHubTrait, HomeLocation);   
        }
        else if (Traits.Contains(CORE.Instance.Database.GetTrait("Religious")))
        {
            hangoutLocation = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.HouseOfWorshipTrait, HomeLocation);
        }
        else if (Traits.Contains(CORE.Instance.Database.GetTrait("Lustful")))
        {
            hangoutLocation = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.HouseOfPleasureTrait, HomeLocation);
        }

        if(hangoutLocation != null)
        {
            CORE.Instance.Database.GetAgentAction("Hang Out").Execute(TopEmployer, this, hangoutLocation);
            return true;
        }

        // After evening? - Pass Time
        return false;
    }

    public void GoToRandomLocation()
    {
        if(CORE.Instance.Locations.Count == 0)
        {
            return;
        }

        GoToLocation(CORE.Instance.Locations[Random.Range(0,CORE.Instance.Locations.Count)]);
        return;
    }

    public void GoToLocation(LocationEntity targetLocation)
    {

        if(targetLocation == CurrentLocation)
        {
            return;
        }

        if (CurrentLocation != null)
        {
            CurrentLocation.CharacterLeftLocation(this);
        }

        if(targetLocation == null)
        {
            return;
        }

        //If new location doesnt have agents and the character is not players agent
        if(TopEmployer.name != CORE.PC.name
            && targetLocation.CharactersInLocation.FindAll((Character charInLocation) => 
            { return charInLocation.TopEmployer == CORE.PC; }).Count == 0)
        {
            Known.ForgetAll("CurrentLocation");
            Known.Know("CurrentLocation", TopEmployer);
        }

        CurrentLocation = targetLocation;

        CurrentLocation.CharacterEnteredLocation(this);
    }

    public void StartWorkingFor(LocationEntity location, bool isGuard = false)
    {
        if (location.OwnerCharacter != null && location.OwnerCharacter.CurrentFaction.isAlwaysKnown)
        {
            Known.KnowAll("Faction");
        }
        else
        {
            if (location.OwnerCharacter != null && location.OwnerCharacter.CurrentFaction != CurrentFaction)
            {
                Known.ForgetAll("Faction");
            }
        }

        if (isGuard)
        {
            location.GuardsCharacters.Add(this);
        }
        else
        {
            location.EmployeesCharacters.Add(this);
        }

        WorkLocation = location;
        WorkLocation.RefreshState();

        if(WorkLocation.CurrentProperty.isHomeOfEmployees)
        {
            StartLivingIn(WorkLocation);
        }

        foreach (LocationEntity ownedLocation in PropertiesInCommand)
        {
            ownedLocation.Known.KnowEverything(TopEmployer);
            ownedLocation.RefreshState();
        }

        foreach (Character employee in CharactersInCommand)
        {
            employee.Known.KnowEverything(TopEmployer);
        }

        if (isKnownOnStart)
        {
            Known.KnowEverything(TopEmployer);
        }

        RefreshVisualTree();

        GoToLocation(WorkLocation);
        
        TryToDoSomething();

        if (TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": has began working for " + WorkLocation.Name, WorkLocation.CurrentProperty.Icon, this));
        }
    }

    public void EnterPrison(LocationEntity location)
    {
        if(PrisonLocation == location)
        {
            return;
        }

        if(location.OwnerCharacter == null)
        {
            return;
        }

        StopDoingCurrentTask(true);
        GoToLocation(location);

        Known.Know("CurrentLocation", location.OwnerCharacter.TopEmployer);

        location.PrisonersCharacters.Add(this);
        PrisonLocation = location;
        location.RefreshState();
        RefreshVisualTree();

        if (TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": has been imprisoned in " + location.Name, location.CurrentProperty.Icon, this));
        }

    }

    public void ExitPrison()
    {
        if(PrisonLocation == null)
        {
            return;
        }

        StopDoingCurrentTask(true);
        PrisonLocation.PrisonersCharacters.Remove(this);
        PrisonLocation.RefreshState();
        PrisonLocation = null;
        RefreshVisualTree();

        if (TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": is no longer imprisoned.", null, this));
        }
    }

    public void StopWorkingForCurrentLocation()
    {
        LocationEntity location = WorkLocation;

        if (location.EmployeesCharacters.Contains(this))
        {
            location.EmployeesCharacters.Remove(this);
        }
        else if (location.GuardsCharacters.Contains(this))
        {
            location.GuardsCharacters.Remove(this);
        }
        else
        {
            return;
        }

        if (TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": has stopped working for "+WorkLocation.Name, WorkLocation.CurrentProperty.Icon, this));
        }

        if (location.CharactersLivingInLocation.Contains(this))
        {
            StartLivingIn(CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.PublicAreaTrait, CurrentLocation));
        }

        LocationEntity tempLocation = WorkLocation;
        WorkLocation = null;
        tempLocation.RefreshState();

        PropertiesInCommand.ForEach(x => x.RefreshState());
        
        RefreshVisualTree();
    }

    public void StartOwningLocation(LocationEntity location)
    {
        if(location.OwnerCharacter != null && CORE.Instance.Characters.Contains(location.OwnerCharacter))
        {
            location.OwnerCharacter.StopOwningLocation(location);
        }

        location.OwnerCharacter = this;

        location.Known.KnowEverything(TopEmployer);
        

        location.RefreshState();

        if(!PropertiesOwned.Contains(location))
        {
            PropertiesOwned.Add(location);
        }

        if (TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": now owns " + location.Name, location.CurrentProperty.Icon, this));
        }
    }

    public void StopOwningLocation(LocationEntity location, bool lookForReplacement = false)
    {
        if (TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": no longer owns " + location.Name, location.CurrentProperty.Icon, this));
        }

        if (lookForReplacement)
        {
            Character possibleReplacement = location.EmployeesCharacters.Find((Character employee) => { return employee.age > 15; });

            if(possibleReplacement == null && TopEmployer != this)
            {
                possibleReplacement = this.TopEmployer;
            }

            if (possibleReplacement == null)
            {
                location.Dispose();
                return;
            }
            else
            {
                possibleReplacement.StopWorkingForCurrentLocation();
                possibleReplacement.CurrentFaction = CORE.Instance.Database.DefaultFaction;//TEST - THIS MAY BE SHIT.
                possibleReplacement.StartOwningLocation(location);
                
                return;
            }
        }

        location.OwnerCharacter = null;
        location.RefreshState();

        if (PropertiesOwned.Contains(location))
        {
            PropertiesOwned.Remove(location);
        }

        GoToRandomLocation();        
    }

    public void StartLivingIn(LocationEntity location)
    {
        StopLivingInCurrentLocation();

        location.CharactersLivingInLocation.Add(this);
        HomeLocation = location;
    }

    public void StopLivingInCurrentLocation(bool findReplacement = false)
    {
        if (HomeLocation == null)
        {
            return;
        }

        HomeLocation.CharactersLivingInLocation.Remove(this);

        if (findReplacement)
        {
            StartLivingIn(CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.PublicAreaTrait, HomeLocation));
        }
        else
        {
            HomeLocation = null;
        }
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
        GoToLocation(task.CurrentTargetLocation);

        if (TopEmployer == CORE.PC)
        {
            CORE.Instance.InvokeEvent("AgentRefreshedAction");
        }

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


        LongTermTaskEntity entity = CurrentTaskEntity;
        CurrentTaskEntity = null;
        entity.Dispose();

        if (TopEmployer == CORE.PC)
        {
            CORE.Instance.InvokeEvent("AgentRefreshedAction");
        }

        return true;
    }

    public void ForceTask(LongTermTaskEntity task)
    {
        CurrentTaskEntity.Cancel();
        StartDoingTask(task);
    }

    public void Death(bool notify = true, bool forced = false)
    {
        if(NeverDED && !forced)
        {
            return; 
        }

        if (this.TopEmployer == CORE.PC && PropertiesOwned.Count > 0)
        {
            string deathString = this.name + " has died! Lost ownership on:";

            PropertiesOwned.ForEach((x) => { deathString += x.Name + " - "; });

            WarningWindowUI.Instance.Show(deathString, ()=> { });
        }

        IsDead = true;

        SelectedPanelUI.Instance.Deselect();

        GoToLocation(CORE.Instance.GetRandomLocationWithTrait(CORE.Instance.Database.BurialGroundTrait));

        if(PrisonLocation != null)
        {
            ExitPrison();
        }

        if (WorkLocation != null)
        {
            StopWorkingForCurrentLocation();
        }

        if(HomeLocation != null)
        {
            StopLivingInCurrentLocation();
        }

        while(PropertiesOwned.Count > 0)
        {
            StopOwningLocation(PropertiesOwned[0], true);
        }


        StopDoingCurrentTask();

        //TODO Causes issues in main turn turner (Removes from iteration container while iterating)
        CORE.Instance.Characters.Remove(this);

        if (notify && TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": <color=red> Has Died </color>", ResourcesLoader.Instance.GetSprite("DeceasedIcon"), this));
            //WarningWindowUI.Instance.Show(this.name + " has died!", () => { });
        }

        if (this == CORE.PC)
        {
            WarningWindowUI.Instance.Show(this.name + " has died! GAME OVER.", () => { LoseWindowUI.Instance.Show(); });
            //WarningWindowUI.Instance.Show(this.name + " has died! GAME OVER.", () => { CORE.Instance.RestartGame(); });
        }

        Faction factionWhichIAmLeaderOf = CORE.Instance.Factions.Find(x => x.FactionHead != null && x.FactionHead.name == this.name);

        if(factionWhichIAmLeaderOf != null)
        {
            CurrentFaction.DissolveFaction();
            CurrentFaction.FactionHead = null;
        }
    }
    

    #endregion

    #region Save & Load

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["ID"] = ID;

        node["name"] = name;

        node["Gender"] = ((int)Gender).ToString();

        node["Gold"] = CGold.ToString();
        node["Connections"] = CConnections.ToString();
        node["Rumors"] = CRumors.ToString();
        node["Progress"] = CProgress.ToString();
        node["Reputation"] = Reputation.ToString();

        node["NeverDED"] = NeverDED.ToString();

        node["PrisonLocation"] = PrisonLocation == null ? "" : PrisonLocation.ID;

        if (PuppetOf != null)
        {
            node["PuppetOf"] = PuppetOf.name;
            node["PupperTurnsLeft"] = PupperTurnsLeft.ToString();
        }

        node["WorkLocation"] = WorkLocation == null ? "" : WorkLocation.ID;
        node["WorkLocationGuard"] = WorkLocation == null ? "" : WorkLocation.GuardsCharacters.Contains(this).ToString();
        node["HomeLocation"] = HomeLocation == null ? "" : HomeLocation.ID;

        node["CurrentLocation"] = CurrentLocation == null ? "" : CurrentLocation.ID;

        node["CurrentFaction"] = CurrentFaction == null  ? "" : CurrentFaction.name;

        node["Pinned"] = Pinned.ToString();

        node["IsDead"] = IsDead.ToString();

        foreach (KnowledgeInstance item in Known.Items)
        {
            for(int i=0;i<item.KnownByCharacters.Count;i++)
            {
                if(item.KnownByCharacters[i] == null)
                {
                    continue;
                }

                node["Knowledge"][item.Key][i] = item.KnownByCharacters[i].ID;
                node["Knowledge"]["Score"][item.Key] = item.Score.ToString();
            }
        }

        for(int i=0;i<KnowledgeRumors.Count;i++)
        {
            node["KnowledgeRumors"][i] = KnowledgeRumors[i].ToJSON();
        }

        for (int i = 0; i < InformationSold.Count; i++)
        {
            node["InformationSold"][i] = InformationSold[i].ToJSON();
        }

        for (int i=0;i<FavorPoints.Count;i++)
        {
            node["Favors"][i]["CharacterID"] = FavorPoints[i].Key.ID.ToString();
            node["Favors"][i]["Favor"] = FavorPoints[i].Value.ToString();
        }

        for (int i=0;i<PropertiesOwned.Count;i++)
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

        for(int i=0;i<Belogings.Count;i++)
        {
            node["Inventory"][i] = Belogings[i].name;
        }

        for(int i=0;i<CaseElements.Count;i++)
        {
            node["CaseElements"][i] = CaseElements[i].name;
        }

        node["age"] = Age.ToString();
        node["skinColor"] = SkinColor.name;
        node["hairColor"] = HairColor.name;
        node["face"] = Face.name;
        node["hair"] = Hair.name;
        node["clothing"] = Clothing.name;
        node["UniquePortrait"] = UniquePortrait == null? "" : UniquePortrait.name;

        if (CurrentQuestioningInstance != null)
        {
            node["CurrentQuestioningInstance"] = CurrentQuestioningInstance.name;
        }

        if (CurrentTaskEntity != null)
        {
            node["CurrentTaskEntityCurrentTask"] = CurrentTaskEntity.CurrentTask.name;
            node["CurrentTaskEntityCurrentRequester"] = CurrentTaskEntity.CurrentRequester.ID;
            node["CurrentTaskEntityCurrentCharacter"] = CurrentTaskEntity.CurrentCharacter.ID;
            node["CurrentTaskEntityCurrentTarget"] = ((LocationEntity)CurrentTaskEntity.CurrentTargetLocation).ID;
            node["CurrentTaskEntityCurrentTargetCharacter"] = CurrentTaskEntity.TargetCharacter != null ? CurrentTaskEntity.TargetCharacter.ID : "";
            node["CurrentTaskEntityTurnsLeft"] = CurrentTaskEntity.TurnsLeft.ToString();
            node["CurrentTaskOriginalAction"] = CurrentTaskEntity.OriginAction != null? CurrentTaskEntity.OriginAction.name : "";
            node["CurrentTaskPerTurnAction"] = CurrentTaskEntity.ActionPerTurn != null ? CurrentTaskEntity.ActionPerTurn.name : "";
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        Initialize();

        ID = node["ID"].Value;

        name = node["name"].Value;

        Gender = (GenderType)int.Parse(node["Gender"].Value);

        CGold = int.Parse(node["Gold"].Value);
        CConnections = int.Parse(node["Connections"].Value);
        CRumors = int.Parse(node["Rumors"].Value);
        CProgress = int.Parse(node["Progress"].Value);
        Reputation = int.Parse(node["Reputation"].Value);

        NeverDED = bool.Parse(node["NeverDED"].Value);

        _prisonLocationID = node["PrisonLocation"].Value;
        _puppetOfFaction = node["PuppetOf"].Value;

        if (!string.IsNullOrEmpty(node["PupperTurnsLeft"].Value))
        {
            PupperTurnsLeft = int.Parse(node["PupperTurnsLeft"].Value);
        }

        _workLocationID = node["WorkLocation"].Value;

        if (!string.IsNullOrEmpty(node["WorkLocationGuard"].Value))
        {
            _isGuard = bool.Parse(node["WorkLocationGuard"].Value);
        }
        else
        {
            _isGuard = false;
        }

        _homeLocationID = node["HomeLocation"].Value;

        _currentLocationID = node["CurrentLocation"].Value;

        Pinned = bool.Parse(node["Pinned"].Value);

        IsDead = bool.Parse(node["IsDead"].Value);

        foreach (KnowledgeInstance item in Known.Items)
        {
            if (string.IsNullOrEmpty(node["Knowledge"][item.Key].ToString()))
            {
                continue;
            }

            List<string> IDs = new List<string>();
            for(int i=0;i< node["Knowledge"][item.Key].Count;i++)
            {
                IDs.Add(node["Knowledge"][item.Key][i].Value);
            }

            knowledgeCharacterIDs.Add(item.Key, IDs);

            

            int score = 0;
            if (!string.IsNullOrEmpty(node["Knowledge"]["Score"][item.Key]))
            {
                score = int.Parse(node["Knowledge"]["Score"][item.Key]);
            }

            knowledgeScore.Add(item.Key, score);

        }

        KnowledgeRumors.Clear();
        for (int i = 0; i < node["KnowledgeRumors"].Count; i++)
        {
            KnowledgeRumors.Add(new KnowledgeRumor());
            KnowledgeRumors[i].FromJSON(node["KnowledgeRumors"][i]);
        }

        InformationSold.Clear();
        for (int i = 0; i < node["InformationSold"].Count; i++)
        {
            InformationSold.Add(new KnowledgeRumor());
            InformationSold[i].FromJSON(node["InformationSold"][i]);
        }

        favorPointsIDs.Clear();

        for (int i=0; i < node["Favors"].Count;i++)
        {
            favorPointsIDs.Add(new FavorPointsPair(null, int.Parse(node["Favors"][i]["Favor"].Value), node["Favors"][i]["CharacterID"].Value));
        }

        _currentFactionName = node["CurrentFaction"];

        _propertiesOwnedIDs = new string[node["PropertiesOwned"].Count];
        for (int i = 0; i < node["PropertiesOwned"].Count; i++)
        {
            _propertiesOwnedIDs[i] = node["PropertiesOwned"][i];
        }

        for (int i = 0; i < node["Traits"].Count; i++)
        {
            AddTrait(CORE.Instance.Database.GetTrait(node["Traits"][i]));
        }

        CaseElements.Clear();
        for (int i = 0; i < node["CaseElements"].Count; i++)
        {
            CaseElements.Add(CORE.Instance.Database.CaseElements.Find(x => x.name == node["CaseElements"].Value));
        }

        for (int i = 0; i < node["DynamicRelationsModifiers"].Count; i++)
        {
            DynamicRelationsModifier modifier = new DynamicRelationsModifier();

            modifier.FromJSON(node["DynamicRelationsModifiers"][i]);

            DynamicRelationsModifiers.Add(modifier);
        }

        for (int i = 0; i < node["Inventory"].Count; i++)
        {
            Belogings.Add(CORE.Instance.Database.GetItem(node["Inventory"][i]).Clone());
        }

        Age = int.Parse(node["age"].Value);
        SkinColor = VisualSet.GetVCByName(node["skinColor"].Value);
        HairColor = VisualSet.GetVCByName(node["hairColor"].Value);
        Face = SkinColor.GetVCByName(node["face"].Value);
        Hair = HairColor.GetVCByName(node["hair"].Value);
        clothing = VisualSet.GetVCByName(node["clothing"].Value);

        if (!string.IsNullOrEmpty(node["UniquePortrait"].Value))
        {
            UniquePortrait = ResourcesLoader.Instance.GetSprite(node["UniquePortrait"].Value);
        }

        if (!string.IsNullOrEmpty(node["CurrentTaskEntityCurrentTask"].Value))
        {
            _currentTaskTurnsLeft = int.Parse(node["CurrentTaskEntityTurnsLeft"].Value);
            _currentTaskName = node["CurrentTaskEntityCurrentTask"].Value;
            _currentTaskRequesterID = node["CurrentTaskEntityCurrentRequester"].Value;
            _currentTaskCharacterID = node["CurrentTaskEntityCurrentCharacter"].Value;
            _currentTaskTargetCharacterID = node["CurrentTaskEntityCurrentTargetCharacter"].Value;
            _currentTaskTargetID = node["CurrentTaskEntityCurrentTarget"].Value;
            _currentTaskOriginalAction = node["CurrentTaskOriginalAction"].Value;
            _currentTaskPerTurnAction = node["CurrentTaskPerTurnAction"].Value;
        }

        if (!string.IsNullOrEmpty(node["CurrentQuestioningInstance"].Value))
        {
            CurrentQuestioningInstance = CORE.Instance.Database.QuestioningInstances.Find(x => x.name == node["CurrentQuestioningInstance"].Value);
        }
        else
        {
            CurrentQuestioningInstance = null;
        }
    }



    string _prisonLocationID;
    string _puppetOfFaction;
    string _workLocationID;
    public bool _isGuard;
    string _homeLocationID;
    string[] _propertiesOwnedIDs;
    string _currentLocationID;

    string _currentTaskName;
    int    _currentTaskTurnsLeft;
    string _currentTaskRequesterID;
    string _currentTaskCharacterID;
    string _currentTaskTargetCharacterID;
    string _currentTaskTargetID;
    string _currentTaskPerTurnAction;
    string _currentTaskOriginalAction;

    string _currentFactionName;

    List<FavorPointsPair> favorPointsIDs = new List<FavorPointsPair>();

    Dictionary<string, List<string>> knowledgeCharacterIDs = new Dictionary<string, List<string>>();
    Dictionary<string, int> knowledgeScore = new Dictionary<string, int>();

    public void ImplementIDs()
    {

        if (!string.IsNullOrEmpty(_homeLocationID))
        {
            StartLivingIn(CORE.Instance.GetLocationByID(_homeLocationID));
        }

        if (!string.IsNullOrEmpty(_workLocationID))
        {
            StartWorkingFor(CORE.Instance.GetLocationByID(_workLocationID),_isGuard);
        }

        if (!string.IsNullOrEmpty(_prisonLocationID))
        {
            EnterPrison(CORE.Instance.GetLocationByID(_prisonLocationID));
        }

        if(!string.IsNullOrEmpty(_puppetOfFaction))
        {
            PuppetOf = CORE.Instance.Factions.Find(x => x.name == _puppetOfFaction);
        }

        if (!string.IsNullOrEmpty(_currentLocationID))
        {
            GoToLocation(CORE.Instance.GetLocationByID(_currentLocationID));
        }

        if(!string.IsNullOrEmpty(_currentFactionName))
        {
            CurrentFaction = CORE.Instance.Factions.Find(x => x.name == _currentFactionName);
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
            LongTermTask task = CORE.Instance.Database.GetLongTermTaskByName(_currentTaskName);

            CORE.Instance.GenerateLongTermTask(
                task,
                CORE.Instance.GetCharacterByID(_currentTaskRequesterID),
                CORE.Instance.GetCharacterByID(_currentTaskCharacterID),
                CORE.Instance.GetLocationByID(_currentTaskTargetID),
                CORE.Instance.GetCharacterByID(_currentTaskTargetCharacterID),
                _currentTaskTurnsLeft,
                !string.IsNullOrEmpty(_currentTaskPerTurnAction) ? CORE.Instance.Database.GetAgentAction(_currentTaskPerTurnAction) : null,
                !string.IsNullOrEmpty(_currentTaskOriginalAction)? CORE.Instance.Database.GetAgentAction(_currentTaskOriginalAction) : null);
        }

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

        foreach (string key in knowledgeScore.Keys)
        {
            Known.GetKnowledgeInstance(key).Score = knowledgeScore[key];
        }

        FavorPoints.Clear();
        foreach (FavorPointsPair favorInstance in favorPointsIDs)
        {
            FavorPoints.Add(new FavorPointsPair(CORE.Instance.Characters.Find(x => x.ID == favorInstance.CharacterID), favorInstance.Value));
        }

        if (CurrentFaction.FactionHead != null && CurrentFaction.FactionHead.name == name)
        {
            AI = Instantiate(CurrentFaction.AI);
        }

        //if (Clothing == null)
        //{
        //    if (IsGuard)
        //    {
        //        if (WorkLocation != null && WorkLocation.CurrentAction.GuardUniform != null)
        //        {
        //            Clothing = Gender == GenderType.Male ?
        //                WorkLocation.CurrentAction.GuardUniform.MaleClothing : WorkLocation.CurrentAction.GuardUniform.FemaleClothing;
        //        }
        //    }
        //    else
        //    {
        //        if (WorkLocation != null && WorkLocation.CurrentAction.EmployeeUniform != null)
        //        {
        //            Clothing = Gender == GenderType.Male ?
        //                WorkLocation.CurrentAction.EmployeeUniform.MaleClothing : WorkLocation.CurrentAction.EmployeeUniform.FemaleClothing;
        //        }
        //    }
        //}


        if (IsDead)
        {
            Death(false);
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

        if (ToCharacter != null) //TODO Potentially wrong, we may have to halt the saving of this quest if no ToChar
        {
            node["ToCharacter"] = ToCharacter.ID.ToString();
        }

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

public class FavorPointsPair
{
    public Character Key;
    public string CharacterID;
    public int Value;

    public FavorPointsPair(Character character, int value = 0, string charID = "")
    {
        this.Key = character;
        this.Value = value;
        this.CharacterID = charID;
    }
}