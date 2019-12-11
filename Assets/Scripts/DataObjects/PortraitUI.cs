using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

public class PortraitUI : AgentInteractable, IPointerClickHandler
{
    [SerializeField]
    public Character CurrentCharacter = null;

    [SerializeField]
    public bool Random = false;

    [SerializeField]
    protected Image Face;

    [SerializeField]
    protected Image Hair;

    [SerializeField]
    protected Image Clothing;

    [SerializeField]
    protected Image Frame;

    [SerializeField]
    protected Image FrameBG;

    [SerializeField]
    protected GameObject QuestionMark;

    [SerializeField]
    protected CanvasGroup CG;

    [SerializeField]
    protected TooltipTargetUI TooltipTarget;

    [SerializeField]
    protected WorldPositionLerperUI Lerper;

    [SerializeField]
    protected ActionPortraitUI ActionPortrait;

    [SerializeField]
    protected Image Unique;

    [SerializeField]
    protected GameObject PrisonBars;

    [SerializeField]
    protected Image AgentRing;

    protected void Start()
    {
        if(Random)
        {
            CurrentCharacter = (Character) Character.CreateInstance("Character");
            CurrentCharacter.Initialize();
            CurrentCharacter.Randomize();
        }

        SetCharacter(CurrentCharacter);
    }

    public virtual void SetCharacter(Character character, Vector3 position)
    {
        Lerper.SetTransform(transform);

        SetCharacter(character);
    }

    public virtual void SetCharacter(Character character, bool ShowActionPortrait = true)
    {
        SetCharacter(character);
        
        if(ActionPortrait == null)
        {
            return;
        }

        ActionPortrait.gameObject.SetActive(false);
        ActionPortrait = null;
    }

    public virtual void SetCharacter(Character character)
    {
        CurrentCharacter = character;

        CG.alpha = 1f;
        Unique.gameObject.SetActive(false);
        Face.gameObject.SetActive(true);

        QuestionMark.gameObject.SetActive(false);

        if (character == null)
        {
            Face.color = Color.black;
            Hair.color = Color.black;
            Clothing.color = Color.black;
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
            FrameBG.color = CORE.Instance.Database.DefaultFaction.FactionColor;

            if (ActionPortrait != null)
            {
                ActionPortrait.gameObject.SetActive(false);
            }

            return;
        }

        if (PrisonBars != null)
        {
            PrisonBars.SetActive(character.PrisonLocation != null);
        }

        if(character.Known != null && !character.IsKnown("Appearance", CORE.PC))
        {

            Face.color = Color.black;
            Hair.color = Color.black;
            Clothing.color = Color.black;
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
            FrameBG.color = CORE.Instance.Database.DefaultFaction.FactionColor;

            if (AgentRing != null)
            {
                AgentRing.gameObject.SetActive(false);
            }


            if (ActionPortrait != null)
            {
                ActionPortrait.gameObject.SetActive(false);
            }

            QuestionMark.gameObject.SetActive(true);

            if (!character.IsKnown("Name", CORE.PC))
            {
                TooltipTarget?.SetTooltip("??? - 'Right Click' for more options...");
            }
            else
            {
                TooltipTarget?.SetTooltip(CurrentCharacter.name + " - 'Right Click' for more options...");
            }

            return;
        }

        Face.color = Color.white;
        Hair.color = Color.white;
        Clothing.color = Color.white;
        

        if(CurrentCharacter != null)
        {
            character.StateChanged.RemoveListener(RefreshState);
            
            if (character.Known != null && !character.IsKnown("Name", CORE.PC))
            {
                TooltipTarget?.SetTooltip("??? - 'Right Click' for more options...");
            }
            else
            {
                TooltipTarget?.SetTooltip(CurrentCharacter.name + " - 'Right Click' for more options...");
            }
        }
        else
        {
            TooltipTarget?.SetTooltip("This character slot is unoccupied...");
        }

        CurrentCharacter = character;

        RefreshState();

        character.StateChanged.AddListener(RefreshState);

        if(CurrentCharacter.UniquePortrait != null)
        {
            Unique.gameObject.SetActive(true);
            Face.gameObject.SetActive(false);
            Unique.sprite = CurrentCharacter.UniquePortrait;
        }
        
    }

    public virtual void SetDisabled()
    {
        CG.alpha = 0.5f;
    }

    public virtual void RefreshVisuals()
    {
        Face.sprite = CurrentCharacter.Face.Sprite;
        Hair.sprite = CurrentCharacter.Hair.Sprite;
        Clothing.sprite = CurrentCharacter.Clothing.Sprite;

        if (CurrentCharacter.Known != null && CurrentCharacter.IsKnown("Faction", CORE.PC))
        {
            Frame.color = CurrentCharacter.CurrentFaction.FactionColor;
            FrameBG.color = CurrentCharacter.CurrentFaction.FactionColor;

            if (AgentRing != null)
            {
                AgentRing.gameObject.SetActive(CurrentCharacter.IsAgent);
                AgentRing.color = CurrentCharacter.CurrentFaction.FactionColor;
            }
        }
        else
        {
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
            FrameBG.color = CORE.Instance.Database.DefaultFaction.FactionColor;

            if (AgentRing != null)
            {
                AgentRing.gameObject.SetActive(false);
            }
        }
    }

    public virtual void RefreshState()
    {
        if(CurrentCharacter == null)
        {
            return;
        }

        RefreshVisuals();
        RefreshAction();
    }

    public virtual void RefreshAction()
    {
        if (ActionPortrait != null)
        {
            if (CurrentCharacter.CurrentTaskEntity != null)
            {
                ActionPortrait.gameObject.SetActive(true);
                ActionPortrait.SetAction(CurrentCharacter.CurrentTaskEntity);
            }
            else
            {
                ActionPortrait.gameObject.SetActive(false);
            }
        }
    }

    public virtual void SelectCharacter()
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

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(CurrentCharacter == null)
        {
            return;
        }

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (CurrentCharacter.TopEmployer == CORE.PC)
            {
                SelectedPanelUI.Instance.SetSelected(this.CurrentCharacter);
            }
            else
            {
                CharacterInfoUI.Instance.ShowInfo(CurrentCharacter);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowActionMenu();
        }
    }

    public override List<AgentAction> GetPossibleAgentActions(Character forCharacter)
    {
        return CORE.Instance.Database.AgentActionsOnAgent;
    }

    public override List<PlayerAction> GetPossiblePlayerActions()
    {
        return CORE.Instance.Database.PlayerActionsOnAgent;
    } 

    public void ShowCharacterInfo()
    {
        CharacterInfoUI.Instance.ShowInfo(this.CurrentCharacter);
    }
}
