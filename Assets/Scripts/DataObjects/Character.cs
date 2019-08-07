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

    public void Randomize()
    {
        this.Gender = (GenderType) Random.Range(0, 2);
        this.name = Names.GetRandomName(Gender);
        this.Age = Random.Range(5, 50);
        this.HairColor = VisualSet.HairColor.Pool[Random.Range(0, VisualSet.HairColor.Pool.Count)];
        this.Hair = HairColor.Pool[Random.Range(0, HairColor.Pool.Count)];
        this.SkinColor = VisualSet.SkinColors.Pool[Random.Range(0, VisualSet.SkinColors.Pool.Count)];
        this.Clothing = VisualSet.Clothing.Pool[Random.Range(0, VisualSet.Clothing.Pool.Count)];
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

    #region AI

    void OnTurnPassedAI()
    {
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

    #endregion


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
   
}
