using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitUI : MonoBehaviour
{
    [SerializeField]
    public Character CurrentCharacter = null;

    [SerializeField]
    public bool Random = false;

    [SerializeField]
    Image Face;

    [SerializeField]
    Image Hair;

    [SerializeField]
    Image Clothing;

    [SerializeField]
    GameObject QuestionMark;

    [SerializeField]
    CanvasGroup CG;

    private void Start()
    {
        if(Random)
        {
            CurrentCharacter = (Character) Character.CreateInstance("Character");
            CurrentCharacter.Initialize();
            CurrentCharacter.Randomize();
        }

        SetCharacter(CurrentCharacter);
    }

    public void SetCharacter(Character character)
    {
        CG.alpha = 1f;

        if (character == null)
        {
            Face.color = Color.black;
            Hair.color = Color.black;
            Clothing.color = Color.black;
            QuestionMark.SetActive(true);
            return;
        }
        
        Face.color = Color.white;
        Hair.color = Color.white;
        Clothing.color = Color.white;
        QuestionMark.SetActive(false);
        

        if(CurrentCharacter != null)
        {
            character.VisualChanged.RemoveListener(RefreshVisuals);
        }

        CurrentCharacter = character;
        RefreshVisuals();

        character.VisualChanged.AddListener(RefreshVisuals);
    }

    public void SetDisabled()
    {
        CG.alpha = 0.5f;
    }

    public void RefreshVisuals()
    {
        Face.sprite = CurrentCharacter.Face.Sprite;
        Hair.sprite = CurrentCharacter.Hair.Sprite;
        Clothing.sprite = CurrentCharacter.Clothing.Sprite;
    }

    public void SelectCharacter()
    {
        if(this.CurrentCharacter == null)
        {
            return;
        }

        MapViewManager.Instance.SelectCharacter(this.CurrentCharacter);
    }
}
