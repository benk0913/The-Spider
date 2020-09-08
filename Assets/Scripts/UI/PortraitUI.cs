using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;
using System;

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

    public bool NoClicking = false;
    

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
        if (CurrentCharacter == character)
        {
            RefreshState();
        }

        string tooltipString = "";
        CG.alpha = 1f;
        Unique.gameObject.SetActive(false);
        Face.gameObject.SetActive(true);

        QuestionMark.gameObject.SetActive(false);        

        if (character == null)
        {
            if(CurrentCharacter != null)
            {
                CurrentCharacter.StateChanged.RemoveListener(RefreshState);
            }

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

            TooltipTarget?.SetTooltip("Empty Slot");

            PrisonBars?.SetActive(false);

            CurrentCharacter = character;

            return;
        }

        if(character.Known != null && !character.IsKnown("Appearance", CORE.PC))
        {
            if (CurrentCharacter != null)
            {
                CurrentCharacter.StateChanged.RemoveListener(RefreshState);
            }

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

            tooltipString = "";

            if(character.IsKnown("Name",CORE.PC))
            {
                tooltipString += character.name + " - ";
            }
            else
            {
                tooltipString += "??? - ";
            }

            if (character.IsKnown("WorkLocation", CORE.PC))
            {
                tooltipString += "\n <color=yellow>"+character.CurrentRole + "</color>";

                if(character.IsAgent)
                {
                    tooltipString += "\n <color=green>Agent</color>";
                }
            }
            else
            {
                tooltipString += "\n <color=yellow>role unknown</color>";
            }

            if (character.TopEmployer == CORE.PC && ActionPortrait != null && ActionPortrait.CurrentEntity != null)
            {
                tooltipString += "\n CURRENT ACTION: ";
                tooltipString += ActionPortrait.TooltipTarget.Text;
            }


            tooltipString += "\n 'Right Click' for more options...";

            TooltipTarget?.SetTooltip(tooltipString);

            if (PrisonBars != null)
            {
                PrisonBars.SetActive(character.PrisonLocation != null);
            }

            CurrentCharacter = character;

            return;
        }

        if (PrisonBars != null)
        {
            PrisonBars.SetActive(character.PrisonLocation != null);
        }

        Face.color = Color.white;
        Hair.color = Color.white;
        Clothing.color = Color.white;


        if (CurrentCharacter != null)
        {
            CurrentCharacter.StateChanged.RemoveListener(RefreshState);
        }

        tooltipString = "";

        if (character.IsKnown("Name", CORE.PC))
        {
            tooltipString += character.name + " - ";
        }
        else
        {
            tooltipString += "??? - ";
        }


        if (character.IsKnown("Personality", CORE.PC))
        {
            foreach (BonusType bonus in CORE.Instance.Database.BonusTypes)
            {
                if (character.GetBonus(bonus).Value > 1)
                {
                    tooltipString += "\n <color=yellow>" + bonus.name + " " + character.GetBonus(bonus).Value + "</color>";
                }
            }
        }

        if (character.IsKnown("WorkLocation", CORE.PC))
        {
            tooltipString += "\n <color=yellow>Role: " + character.CurrentRole + "</color>";
                
            if (character.IsAgent && character.IsKnown("Faction", CORE.PC))
            {
                tooltipString += "\n <color=#"+ ColorUtility.ToHtmlStringRGB(character.CurrentFaction.FactionColor)+">"+character.CurrentFaction.name+"</color>";

                if (character.TopEmployer == CORE.PC)
                {
                    tooltipString += "\n <color=green>Your Agent</color>";
                }
                else
                {
                    tooltipString += "\n <color=green>An Agent</color>";
                }
            }
                
        }
        else
        {
            tooltipString += "\n <color=yellow>Role: unknown</color>";
        }

        
        if (character.TopEmployer == CORE.PC && ActionPortrait != null && ActionPortrait.CurrentEntity != null)
        {
            tooltipString += "\n <color=black>------------</color>";
            tooltipString += ActionPortrait.TooltipTarget.Text;
        }

        tooltipString += "\n 'Right Click' for more options...";

        TooltipTarget?.SetTooltip(tooltipString);

        if (character.UniquePortrait != null)
        {
            Unique.gameObject.SetActive(true);
            Face.gameObject.SetActive(false);
            Unique.sprite = character.UniquePortrait;
        }

        CurrentCharacter = character;

        RefreshState();

        CurrentCharacter.StateChanged.AddListener(RefreshState);

        
    }

    public virtual void SetDisabled()
    {
        CG.alpha = 0.5f;
    }

    public virtual void RefreshVisuals()
    {
        if(CurrentCharacter == null)
        {
            Debug.LogError("NO CHARACTER!?");
            return;
        }

        if (CurrentCharacter.Clothing == null)
        {
            Debug.LogError("NO CLOTHING!? - "+CurrentCharacter.name);
            return;
        }

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

        if (PrisonBars != null)
        {
            PrisonBars.SetActive(CurrentCharacter.PrisonLocation != null);
        }
    }

    public virtual void RefreshState()
    {
        if(CurrentCharacter == null)
        {
            return;
        }

        RefreshAction();
        RefreshVisuals();
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
        if(NoClicking)
        {
            return;
        }

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
