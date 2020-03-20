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
        this.TooltipTarget.SetTooltip("<color=green>Drag a relevant rumor here to sell it for gold!</color>- ");
        this.Score.text = Mathf.Max(0f, (float)20 - (5 * CurrentCharacter.Rank)).ToString(); //Gold amount (30c - Penalty on lower rank (5c per rank))    }
    }

    DragDroppableRumorUI LastDrag;
    private void Update()
    {
        if(ResearchCharacterWindowUI.Instance.CurrentDragged != null && ResearchCharacterWindowUI.Instance.CurrentDragged != LastDrag)
        {
            if(ResearchCharacterWindowUI.Instance.CurrentCharacter.InformationSold.Contains(ResearchCharacterWindowUI.Instance.CurrentDragged.CurrentRumor))
            {
                this.Title.text = "<color=red>ALREADY SOLD</color>";
                this.TooltipTarget.SetTooltip("<color=green>Drag a relevant rumor here to dispose. (YOU HAVE ALREADY SOLD THIS INFORMATION)</color>- ");
                this.Score.text = "<color=red>DISPOSE</color>";
            }
            else
            {
                RefreshUI();
            }
        }

        LastDrag = ResearchCharacterWindowUI.Instance.CurrentDragged;
    }

    public override void Consume(DragDroppableRumorUI item)
    {
        if (CurrentCharacter.InformationSold.Find(x => x.Description == item.CurrentRumor.Description) != null)
        {
            AudioControl.Instance.Play("paper_crumple");

            GlobalMessagePrompterUI.Instance.Show("Disposed of information.", 1f, Color.red);
        }
        else
        {
            AudioControl.Instance.Play("resource_gold");

            CurrentCharacter.InformationSold.Add(item.CurrentRumor);
            CurrentCharacter.KnowledgeRumors.Remove(item.CurrentRumor);

            int goldRevenue = Mathf.RoundToInt(Mathf.Max(0f, (float)20 - (5 * CurrentCharacter.Rank)));
            CORE.PC.CGold += goldRevenue;
            StatsViewUI.Instance.RefreshGold();
            GlobalMessagePrompterUI.Instance.Show("Sold information for " + goldRevenue + " gold.", 1f, Color.yellow);
        }

        item.ConsumeIncorrect();

       

        RefreshUI();
    }
}
