using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizeCharacterWindowUI : MonoBehaviour
{
    public static  CustomizeCharacterWindowUI Instance;

    public TMP_InputField NameField;
    public TMP_InputField AgeField;
    public Button MaleBTN;
    public Button FemaleBTN;

    Character CurrentCharacter;

    public PortraitUI CharacterPortrait;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void SetActor(Character character)
    {
        CurrentCharacter = character;
        this.gameObject.SetActive(true);

        RefreshUI();
    }

    void RefreshUI()
    {
        if(CurrentCharacter == null)
        {
            return;
        }

        NameField.text = CurrentCharacter.name;
        AgeField.text = CurrentCharacter.Age.ToString();
        MaleBTN.interactable = CurrentCharacter.Gender == GenderType.Female;
        FemaleBTN.interactable = CurrentCharacter.Gender == GenderType.Male;

        CharacterPortrait.SetCharacter(CurrentCharacter);

        CharacterInfoUI.Instance.ShowInfo(CurrentCharacter);
    }

    public void SetName()
    {
        CurrentCharacter.name = NameField.text;
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void SetAge()
    {
        int finalAge = CurrentCharacter.Age;
        if(int.TryParse(AgeField.text, out finalAge))
        {
            CurrentCharacter.Age = finalAge;
            CurrentCharacter.RefreshVisualTree();
            RefreshUI();
        }
    }

    public void SetMale()
    {
        CurrentCharacter.Gender = GenderType.Male;
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void SetFemale()
    {
        CurrentCharacter.Gender = GenderType.Female;
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void NextSkinColor()
    {
        int poolIndex = CurrentCharacter.VisualSet.SkinColors.Pool.IndexOf(CurrentCharacter.SkinColor);
        
        poolIndex++;
        if(poolIndex >= CurrentCharacter.VisualSet.SkinColors.Pool.Count)
        {
            poolIndex = 0;
        }

        CurrentCharacter.SkinColor = CurrentCharacter.VisualSet.SkinColors.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void PreviousHairColor()
    {
        int poolIndex = CurrentCharacter.VisualSet.HairColor.Pool.IndexOf(CurrentCharacter.HairColor);
        
        poolIndex--;
        if(poolIndex < 0)
        {
            poolIndex = CurrentCharacter.VisualSet.HairColor.Pool.Count-1;
        }

        CurrentCharacter.HairColor = CurrentCharacter.VisualSet.HairColor.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void NextHairColor()
    {
        int poolIndex = CurrentCharacter.VisualSet.HairColor.Pool.IndexOf(CurrentCharacter.HairColor);
        
        poolIndex++;
        if(poolIndex >= CurrentCharacter.VisualSet.HairColor.Pool.Count)
        {
            poolIndex = 0;
        }

        CurrentCharacter.HairColor = CurrentCharacter.VisualSet.HairColor.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    

    public void PreviousSkinColor()
    {
        int poolIndex = CurrentCharacter.VisualSet.SkinColors.Pool.IndexOf(CurrentCharacter.SkinColor);
        
        poolIndex--;
        if(poolIndex < 0)
        {
            poolIndex = CurrentCharacter.VisualSet.SkinColors.Pool.Count-1;
        }

        CurrentCharacter.SkinColor = CurrentCharacter.VisualSet.SkinColors.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void NextFace()
    {
        int poolIndex = CurrentCharacter.SkinColor.Pool.IndexOf(CurrentCharacter.Face);
        
        poolIndex++;
        if(poolIndex >=  CurrentCharacter.SkinColor.Pool.Count)
        {
            poolIndex = 0;
        }

        CurrentCharacter.Face =  CurrentCharacter.SkinColor.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void PreviousFace()
    {
        int poolIndex =  CurrentCharacter.SkinColor.Pool.IndexOf(CurrentCharacter.Face);
        
        poolIndex--;
        if(poolIndex < 0)
        {
            poolIndex =  CurrentCharacter.SkinColor.Pool.Count-1;
        }

        CurrentCharacter.Face =  CurrentCharacter.SkinColor.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void NextHairStyle()
    {
        int poolIndex = CurrentCharacter.HairColor.Pool.IndexOf(CurrentCharacter.Hair);
        
        poolIndex++;
        if(poolIndex >=  CurrentCharacter.HairColor.Pool.Count)
        {
            poolIndex = 0;
        }

        CurrentCharacter.Hair =  CurrentCharacter.HairColor.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void PreviousHairStyle()
    {
        int poolIndex =  CurrentCharacter.HairColor.Pool.IndexOf(CurrentCharacter.Hair);
        
        poolIndex--;
        if(poolIndex < 0)
        {
            poolIndex =  CurrentCharacter.HairColor.Pool.Count-1;
        }

        CurrentCharacter.Hair =  CurrentCharacter.HairColor.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void PreviousClothing()
    {
        int poolIndex = CurrentCharacter.VisualSet.Clothing.Pool.IndexOf(CurrentCharacter.Clothing);
        
        poolIndex--;
        if(poolIndex < 0)
        {
            poolIndex = CurrentCharacter.VisualSet.Clothing.Pool.Count-1;
        }

        CurrentCharacter.Clothing = CurrentCharacter.VisualSet.Clothing.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    public void NextClothing()
    {
        int poolIndex = CurrentCharacter.VisualSet.Clothing.Pool.IndexOf(CurrentCharacter.Clothing);
        
        poolIndex++;
        if(poolIndex >= CurrentCharacter.VisualSet.Clothing.Pool.Count)
        {
            poolIndex = 0;
        }

        CurrentCharacter.Clothing = CurrentCharacter.VisualSet.Clothing.Pool[poolIndex];
        CurrentCharacter.RefreshVisualTree();
        RefreshUI();
    }

    
    


}
