using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Character", menuName = "DataObjects/Character", order = 2)]
public class Character : ScriptableObject
{
    #region Stats

    public string ID;

    [SerializeField]
    public GenderSet VisualSet;

    [SerializeField]
    public Character Employer;

    public Character TopEmployer
    {
        get
        {
            if(Employer == null)
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
                Employer.Gold += earnings / 2;
                earnings /= 2;
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
    public AgeTypeEnum AgeType { private set; get; }

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

    public UnityEvent VisualChanged = new UnityEvent();

    public Character()
    {
        this.ID = Util.GenerateUniqueID();
    }

    public void Randomize()
    {
        this.name = "Random Name";
        this.Gender = (GenderType) Random.Range(0, 2);
        this.Age = Random.Range(5, 50);
        this.HairColor = VisualSet.HairColor.Pool[Random.Range(0, VisualSet.HairColor.Pool.Count)];
        this.Hair = HairColor.Pool[Random.Range(0, HairColor.Pool.Count)];
        this.SkinColor = VisualSet.SkinColors.Pool[Random.Range(0, VisualSet.SkinColors.Pool.Count)];
        this.Clothing = VisualSet.Clothing.Pool[Random.Range(0, VisualSet.Clothing.Pool.Count)];
    }

    public void RefreshVisualTree()
    {
        //TODO Get rid of this when we have enough data.
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
        //TODO Get rid of this when we have enough data.
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
    }

    public void Join(LocationEntity location)
    {
        location.EmployeesCharacters.Add(this);
        Employer = location.OwnerCharacter;
    }

}
