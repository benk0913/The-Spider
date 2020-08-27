using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuestioningItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public QuestioningItem CurrentItem;

    [SerializeField]
    TextMeshProUGUI Content;

    [SerializeField]
    Image SkillIcon;

    [SerializeField]
    Animator Anim;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    public bool IsOpponents;

    public void SetInfo(QuestioningItem item, bool isOpponents = false)
    {
        this.CurrentItem = item;
        this.Content.text = CurrentItem.Texts[Random.Range(0,CurrentItem.Texts.Count)];
        IsOpponents = isOpponents;

        if(IsOpponents)
        {
            this.SkillIcon.sprite = ResourcesLoader.Instance.GetSprite("uncertainty");
            TooltipTarget.SetTooltip("Skill Unknown");
        }
        else
        {
            this.SkillIcon.sprite = CurrentItem.RelevantSkill.icon;

            string tooltip = "";
            tooltip += "<color=yellow>";
            tooltip += CurrentItem.BeatsSkill != null ? "This beats answeres of type: " + CurrentItem.BeatsSkill.name : "This beats no skill.";
            tooltip += "</color>";
            tooltip += System.Environment.NewLine;
            tooltip += "<color=green>";
            tooltip += "Effect: "+ System.Text.RegularExpressions.Regex.Replace(CurrentItem.Type.ToString(), @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
            tooltip += "</color>";
            TooltipTarget.SetTooltip(tooltip);
        }
    }

    public void Spend()
    {
        Anim.SetTrigger("Spend");
        QuestioningWindowUI.Instance.SpendCard(this);
    }

    public void Use()
    {
        if(QuestioningWindowUI.Instance.UseCard(this))
        {
            UseRight();
        }
        else
        {
            UseWrong();
        }
    }

    public void UseWrong()
    {
        RevealIcon();

        Anim.SetTrigger("UseWrong");
    }

    public void UseRight()
    {
        RevealIcon();
        Anim.SetTrigger("UseRight");
    }

    public void RevealIcon()
    {
        if (IsOpponents)
        {
            this.SkillIcon.sprite = CurrentItem.RelevantSkill.icon;
        }

    }

    public void Dispose()
    {
        this.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsOpponents)
        {
            return;
        }

            if (eventData.button == PointerEventData.InputButton.Left)
        {
            Use();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Spend();
        }
    }
}
