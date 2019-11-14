using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManagementNotificationUI : MonoBehaviour
{
    protected List<Character> Characters = new List<Character>();

    protected int CurrentIndex;

    [SerializeField]
    protected TooltipTargetUI TooltipTarget;

    [SerializeField]
    public string ProblemText;

    public void OnStateChanged()
    {
        RefreshElements();
        RefreshUI();
    }

    protected virtual void RefreshUI()
    {
        if (Characters.Count <= 0)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);

        RefreshTooltip();
    }

    protected virtual void RefreshElements()
    {
        this.Characters = CORE.Instance.Characters.FindAll(CommonFilter);
    }

    public virtual bool CommonFilter(Character character)
    {
        return true;
    }

    protected virtual void RefreshTooltip()
    {
        List<TooltipBonus> Bonuses = new List<TooltipBonus>();

        for(int i=0;i<Characters.Count;i++)
        {
            if (CurrentIndex == i)
            {
                Bonuses.Add(new TooltipBonus("<color=green>"+Characters[i].name+"</color>", ResourcesLoader.Instance.GetSprite("pointing")));
            }
            else
            {
                Bonuses.Add(new TooltipBonus(Characters[i].name, ResourcesLoader.Instance.GetSprite("three-friends")));
            }
        }

        TooltipTarget.SetTooltip(ProblemText,Bonuses);
    }

    public virtual void ShowNextElement()
    {
        if(Characters.Count <= 0)
        {
            return;
        }

        CurrentIndex++;

        if(CurrentIndex >= Characters.Count)
        {
            CurrentIndex = 0;
        }

        RefreshUI();

        SelectedPanelUI.Instance.SetSelected(Characters[CurrentIndex]);
    }
}
