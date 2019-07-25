﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PortraitUI : AgentInteractable, IPointerClickHandler
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

    [SerializeField]
    TooltipTargetUI TooltipTarget;

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
        CurrentCharacter = character;

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

            TooltipTarget.Text = CurrentCharacter.name + " - 'Right Click' for more info...";
        }
        else
        {
            TooltipTarget.Text = "This character slot is unoccupied...";
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

        if (CurrentCharacter.Employer == null)
        {
            CharacterInfoUI.Instance.ShowInfo(CurrentCharacter);
            return;
        }

        if (CurrentCharacter.TopEmployer != CORE.PC)
        {
            CharacterInfoUI.Instance.ShowInfo(CurrentCharacter);
            return;
        }

        SelectedPanelUI.Instance.SetSelected(this.CurrentCharacter);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            CharacterInfoUI.Instance.ShowInfo(CurrentCharacter);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowActionMenu();
        }
    }

    public override List<AgentAction> GetPossibleAgentActions(Character forCharacter)
    {
        return base.GetPossibleAgentActions(forCharacter);
    }

    public override List<PlayerAction> GetPossiblePlayerActions()
    {
        return CORE.Instance.Database.PlayerActionsOnAgent;
    } 
}
