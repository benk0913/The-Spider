using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeItemUnlockableUI : MonoBehaviour
{
    protected KnowledgeInstance CurrentKnowledge;
    protected Character CurrentCharacter;

    [SerializeField]
    protected TextMeshProUGUI Title;

    [SerializeField]
    protected TextMeshProUGUI Score;

    [SerializeField]
    protected GameObject KnownPanel;

    [SerializeField]
    protected Image Icon;

    [SerializeField]
    protected TooltipTargetUI TooltipTarget;

    public virtual void SetInfo(Character character, KnowledgeInstance knowledge)
    {
        this.CurrentCharacter = character;
        this.CurrentKnowledge = knowledge;

        RefreshUI();
    }

    public virtual void RefreshUI()
    {
        this.Title.text = CurrentKnowledge.Key;
        this.TooltipTarget.SetTooltip("<color=green>Drag a relevant rumor here to unlock this knowledge </color>- " + CurrentKnowledge.Description);
        this.Score.text = this.CurrentKnowledge.Score+"/"+ this.CurrentKnowledge.ScoreMax;

        if (CurrentKnowledge.Icon != null)
        {
            Icon.sprite = CurrentKnowledge.Icon;
        }

        if (CurrentKnowledge.IsKnownByCharacter(CORE.PC))
        {
            this.Score.gameObject.SetActive(false);
            this.KnownPanel.gameObject.SetActive(true);
        }
        else
        {
            this.Score.gameObject.SetActive(true);
            this.KnownPanel.gameObject.SetActive(false);
        }
    }

    public void OnHover()
    {
        ResearchCharacterWindowUI.Instance.CurrentHovered = this;
    }

    public void OnUnhover()
    {
        if(ResearchCharacterWindowUI.Instance.CurrentHovered == this)
        {
            ResearchCharacterWindowUI.Instance.CurrentHovered = null;
        }
    }

    public virtual void Consume(DragDroppableRumorUI item)
    {

        CurrentCharacter.KnowledgeRumors.Remove(item.CurrentRumor);

        if (CurrentKnowledge.Key == item.CurrentRumor.RelevantKey)
        {
            item.Consume();

            CurrentKnowledge.Score++;

            RefreshUI();

            ResearchCharacterWindowUI.Instance.Portrait.SetCharacter(ResearchCharacterWindowUI.Instance.CurrentCharacter);
        }
        else
        {
            item.ConsumeIncorrect();
        }
    }
}
