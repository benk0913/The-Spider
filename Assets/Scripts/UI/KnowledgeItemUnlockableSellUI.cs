using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeItemUnlockableSellUI : KnowledgeItemUnlockableUI
{

    public override void SetInfo(Character character, KnowledgeInstance knowledge)
    {
        this.CurrentCharacter = character;
        this.CurrentKnowledge = knowledge;

        RefreshUI();
    }

    public override void RefreshUI()
    {
        this.Title.text = "Sell Information";
        this.TooltipTarget.SetTooltip("<color=green>Drag a relevant rumor here to sell it for gold!</color>- " + CurrentKnowledge.Description);
        this.Score.text = Mathf.Max(0f, (float)20 - (5 * CurrentCharacter.Rank)).ToString(); //Gold amount (30c - Penalty on lower rank (5c per rank))    }
    }

    public override void Consume(DragDroppableRumorUI item)
    {
        CurrentCharacter.KnowledgeRumors.Remove(item.CurrentRumor);

        item.ConsumeIncorrect();

        int goldRevenue = Mathf.RoundToInt(Mathf.Max(0f, (float)20 - (5 * CurrentCharacter.Rank)));
        CORE.PC.Gold += goldRevenue;
        StatsViewUI.Instance.RefreshGold();
        GlobalMessagePrompterUI.Instance.Show("Sold information for " + goldRevenue + " gold.", 1f, Color.yellow);

        RefreshUI();
    }
}
