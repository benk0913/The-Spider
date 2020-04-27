using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecruitmentPoolUI : MonoBehaviour, IPointerClickHandler
{
    public RecruitmentPool CurrentPool;

    public Action OnClick;

    [SerializeField]
    TextMeshProUGUI Label;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    Sprite DefaultSprite;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentPool != null && CORE.PC.Reputation < CurrentPool.MinReputation)
        {
            GlobalMessagePrompterUI.Instance.Show("NOT ENOUGH REPUTATION", 1f, Color.red);
            return;
        }

        OnClick?.Invoke();
    }

    public void SetInfo(RecruitmentPool pool, Action action = null)
    {
        CurrentPool = pool;
        OnClick = action;

        if(CurrentPool == null)
        {
            Label.text = "Random";
            IconImage.sprite = DefaultSprite;
            TooltipTarget.SetTooltip("<color=yellow>Random Character</color><color=green> - Cost: " + 3 + " Connections </color>");
            return;
        }

        Label.text = CurrentPool.name;
        IconImage.sprite = CurrentPool.Icon;

        if(CORE.PC.Reputation < pool.MinReputation)
        {
            TooltipTarget.SetTooltip("<color=yellow>"+pool.name + "</color> - <color=red>Minimum Reputation: " + CurrentPool.MinReputation + "</color> - <color=green>Cost: " + pool.Cost + " Connections</color> - " + CurrentPool.Description);
            IconImage.color = Color.red;
        }
        else
        {
            TooltipTarget.SetTooltip("<color=yellow>"+pool.name + "</color> - Minimum Reputation: " + CurrentPool.MinReputation + " - <color=green>Cost: " + pool.Cost + " Connections</color> - " + CurrentPool.Description);
            IconImage.color = Color.white;
        }
    }
}
