using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Character", menuName = "DataObjects/Character", order = 2)]
public class Character : ScriptableObject
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

    #region Traits & Bonuses

    /// <summary>
    /// DO NOT EVER SET (PUBLIC FOR SERIALIZATION)
    /// </summary>
    public List<Trait> Traits = new List<Trait>();

    public void AddTrait(Trait trait)
    {
        if(Traits.Contains(trait))
        {
            Trait nextTrait = trait.NextTrait;
            if (nextTrait != null)
            {
                Traits.Remove(trait);
                AddTrait(nextTrait);
            }
            return;
        }

        for (int i = 0; i < trait.OppositeTraits.Length; i++)
        {
            if (Traits.Contains(trait.OppositeTraits[i]))
            {
                Trait previousTrait = trait.OppositeTraits[i].PreviousTrait;

                if (previousTrait != null)
                {
                    Traits.Remove(trait.OppositeTraits[i]);
                    AddTrait(previousTrait);
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
            VisualChanged.Invoke();
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
            VisualChanged.Invoke();
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
            VisualChanged.Invoke();
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
            VisualChanged.Invoke();
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
            VisualChanged.Invoke();
        }
    }
    [SerializeField]
    VisualCharacteristic clothing;

    #endregion

    #endregion


    public UnityEvent VisualChanged = new UnityEvent();

    public Character()
    {
        this.ID = Util.GenerateUniqueID();
    }

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

        Clothing = VisualSet.Clothing.GetVCByName(Clothing.name);

        VisualChanged.Invoke();
    }

    public void Initialize()
    {
        RaceSet raceSet = CORE.Instance.Database.GetRace("Human");

        AgeSet ageSet = CORE.Instance.Database.GetRace("Human").GetAgeSet(AgeTypeEnum.Adult);

        if(ageSet == null)
        {
            Debug.LogError("NO AGE SET! "+AgeTypeEnum.Adult.ToString());
            return;
        }

        VisualSet = Gender == GenderType.Male? 
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

        RefreshVisualTree();

        if (CORE.PC != this)
        {
            GameClock.Instance.OnTurnPassed.AddListener(OnTurnPassedAI);
        }

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

        float charmModifier = otherCharacter.GetBonus(CORE.Instance.Database.GetBonusType("Charm")).Value;

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

        foreach (LocationEntity location in PropertiesOwned)
        {
            ManageProperty(location);
        }

        GoToRandomLocation();
    }

    void GoToRandomLocation()
    {
        
        //TODO Change later to a better algorythm
        if (WorkLocation != null && Random.Range(0, 2) != 0)//To work
        {
            GoToLocation(WorkLocation);
            return;
        }

        if (PropertiesOwned.Count > 0 && Random.Range(0, 4) != 0)//To one of my properties
        {
            GoToLocation(PropertiesOwned[Random.Range(0,PropertiesOwned.Count)]);
            return;
        }

        else // To random public location
        {
            GoToLocation(CORE.Instance.GetRandomLocationWithTrait(CORE.Instance.Database.PublicAreaTrait));
            return;
        }
    }

    void GoToLocation(LocationEntity targetLocation)
    {
        CurrentLocation = targetLocation;
    }

    void ManageProperty(LocationEntity location)
    {
        location.StartRecruiting();
    }

    public void StartWorkingFor(LocationEntity location)
    {
        location.EmployeesCharacters.Add(this);
        
        CORE.Instance.ShowHoverMessage("New Recruit", ResourcesLoader.Instance.GetSprite("three-friends"), location.transform);

        WorkLocation = location;
        WorkLocation.RefreshState();

        foreach (LocationEntity ownedLocation in PropertiesOwned)
        {
            ownedLocation.RefreshState();
        }

        GoToLocation(location);
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

        GoToRandomLocation();
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

        GoToLocation(location);
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

    #endregion

   
}

public class DynamicRelationsModifier
{
    public RelationsModifier Modifier;
    public int Turns;
    public Character ToCharacter;

    public DynamicRelationsModifier(RelationsModifier modifier, int turns, Character toCharacter)
    {
        this.Modifier = modifier;
        this.Turns = turns;
        this.ToCharacter = toCharacter;
    }
}


[System.Serializable]
public class RelationsModifier
{
    public string Message;
    public int Value;

    public RelationsModifier(string message, int value)
    {
        //this.Message = (value > 0 ? "<color=green>" : "<color=red>") + message + " (" + value + ")</color>";
        this.Message =  message;
        this.Value = value;
    }
}
