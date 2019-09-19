using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

public class PortraitUI : AgentInteractable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
    protected GameObject QuestionMark;

    [SerializeField]
    GameObject InfoButton;

    [SerializeField]
    protected CanvasGroup CG;

    [SerializeField]
    protected TooltipTargetUI TooltipTarget;

    [SerializeField]
    protected WorldPositionLerperUI Lerper;

    [SerializeField]
    protected ActionPortraitUI ActionPortrait;


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
        Lerper.SetPosition(position);

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

        QuestionMark.gameObject.SetActive(false);

        if (character == null)
        {
            Face.color = Color.black;
            Hair.color = Color.black;
            Clothing.color = Color.black;
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;

            if(ActionPortrait != null)
            {
                ActionPortrait.gameObject.SetActive(false);
            }

            return;
        }

        if(!character.IsKnown("Appearance"))
        {

            Face.color = Color.black;
            Hair.color = Color.black;
            Clothing.color = Color.black;
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;

            if (ActionPortrait != null)
            {
                ActionPortrait.gameObject.SetActive(false);
            }

            TooltipTarget.Text = "This character is unknown";

            QuestionMark.gameObject.SetActive(true);

            return;
        }
        
        Face.color = Color.white;
        Hair.color = Color.white;
        Clothing.color = Color.white;
        

        if(CurrentCharacter != null)
        {
            character.StateChanged.RemoveListener(RefreshState);

            TooltipTarget.Text = CurrentCharacter.name + " - 'Right Click' for more info...";
        }
        else
        {
            TooltipTarget.Text = "This character slot is unoccupied...";
        }

        CurrentCharacter = character;

        RefreshState();

        character.StateChanged.AddListener(RefreshState);

        
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
        Frame.color = CurrentCharacter.CurrentFaction.FactionColor;
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

    public void OnPointerEnter(PointerEventData eventData)
    { 
        if(InfoButton == null)
        {
            return;
        }

        InfoButton.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InfoButton == null)
        {
            return;
        }

        InfoButton.gameObject.SetActive(false);
    }
}
