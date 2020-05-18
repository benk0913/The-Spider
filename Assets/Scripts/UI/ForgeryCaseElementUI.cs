using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ForgeryCaseElementUI : MonoBehaviour, IPointerClickHandler
{
    public ForgeryCaseElement CurrentElement;
    public Character CurrentCharacter;
    public LocationEntity CurrentLocation;

    public bool Owned = false;

    public Action OnClick;

    [SerializeField]
    TextMeshProUGUI Label;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    Sprite DefaultSprite;

    public bool IsVSCharacter
    {
        get
        {
            return CurrentCharacter != null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioControl.Instance.Play("click");

        if (Owned)
        {
            GlobalMessagePrompterUI.Instance.Show("You Already Own This Element", 1f, Color.green);
            return;
        }

        FailReason reason = null;
        if (IsVSCharacter)
        {
            if (!CurrentElement.IsAvailable(CurrentCharacter, out reason))
            {
                GlobalMessagePrompterUI.Instance.Show("Unavailable: " + reason, 1f, Color.red);
                return;
            }
            CurrentCharacter.CaseElements.Add(CurrentElement);
        }
        else
        {
            if (!CurrentElement.IsAvailable(CurrentLocation, out reason))
            {
                GlobalMessagePrompterUI.Instance.Show("Unavailable: " + reason, 1f, Color.red);
                return;
            }
            CurrentLocation.CaseElements.Add(CurrentElement);
        }

        AudioControl.Instance.Play("page_turn");

        CurrentElement.SpendCost();
   
        OnClick?.Invoke();
    }

    public void SetInfo(ForgeryCaseElement element, LocationEntity location, Action onClick = null)
    {
        CurrentElement = element;
        CurrentCharacter = null;
        CurrentLocation = location;

        Owned = location.CaseElements.Find(x => x.name == element.name) != null;

        OnClick = onClick;

        if (CurrentElement == null)
        {
            Label.text = "Random";
            IconImage.sprite = DefaultSprite;
            TooltipTarget.SetTooltip("<color=yellow>Random Character</color><color=green>" + System.Environment.NewLine + "- Cost: " + 3 + " Connections </color>");
            return;
        }

        Label.text = CurrentElement.name;
        IconImage.sprite = CurrentElement.Icon;

        if (Owned)
        {
            TooltipTarget.SetTooltip("<color=green>" + element.name + System.Environment.NewLine + "</color> - " + CurrentElement.Description);
            IconImage.color = Color.green;
        }
        else
        {
            FailReason reason = null;
            if (element.IsAvailable(location, out reason))
            {
                TooltipTarget.SetTooltip("<color=yellow>" + element.name + System.Environment.NewLine + "</color> - " + System.Environment.NewLine + " - " + System.Environment.NewLine + CurrentElement.Description);
                IconImage.color = Color.white;
            }
            else
            {
                TooltipTarget.SetTooltip("<color=red>" + element.name + System.Environment.NewLine + "- Unavailable - " + reason.Key + " </color> - " + System.Environment.NewLine + CurrentElement.CostDescription + " - " + System.Environment.NewLine + CurrentElement.Description);
                IconImage.color = Color.red;
            }
        }
    }

    public void SetInfo(ForgeryCaseElement element, Character character, Action onClick = null)
    {
        CurrentElement = element;
        CurrentCharacter = character;
        CurrentLocation = null;
        
        Owned = character.CaseElements.Find(x => x.name == element.name) != null;

        OnClick = onClick;

        if(CurrentElement == null)
        {
            Label.text = "Random";
            IconImage.sprite = DefaultSprite;
            TooltipTarget.SetTooltip("<color=yellow>Random Character</color><color=green>" + System.Environment.NewLine +"- Cost: " + 3 + " Connections </color>");
            return;
        }

        Label.text = CurrentElement.name;
        IconImage.sprite = CurrentElement.Icon;

        if(Owned)
        {
            TooltipTarget.SetTooltip("<color=green>"+element.name + System.Environment.NewLine + "</color> - " + CurrentElement.Description);
            IconImage.color = Color.green;
        }
        else
        {
            FailReason reason = null;
            if(element.IsAvailable(character, out reason))
            {
                TooltipTarget.SetTooltip("<color=yellow>" + element.name + System.Environment.NewLine + "</color> - " +System.Environment.NewLine +" - " + System.Environment.NewLine + CurrentElement.Description);
                IconImage.color = Color.white;
            }
            else
            {
                TooltipTarget.SetTooltip("<color=red>" + element.name + System.Environment.NewLine + "- Unavailable - " + reason.Key + " </color> - " + System.Environment.NewLine + CurrentElement.CostDescription + " - " + System.Environment.NewLine + CurrentElement.Description);
                IconImage.color = Color.red;
            }
        }
    }
}
