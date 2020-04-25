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
            TooltipTarget.SetTooltip("Random Character - Cost: " + 3 + " Connections");
            return;
        }

        Label.text = CurrentPool.name;
        IconImage.sprite = CurrentPool.Icon;

        if(CORE.PC.Reputation < pool.MinReputation)
        {
            TooltipTarget.SetTooltip(pool.name + " - <color=red>Minimum Reputation: " + CurrentPool.MinReputation + "</color> - Cost: " + pool.Cost + " Connections - " + CurrentPool.Description);
            IconImage.color = Color.red;
        }
        else
        {
            TooltipTarget.SetTooltip(pool.name + " - Minimum Reputation: " + CurrentPool.MinReputation + " - Cost: " + pool.Cost + " Connections - " + CurrentPool.Description);
            IconImage.color = Color.white;
        }
    }
}
