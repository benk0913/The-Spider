using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FavorDecisionUI : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TooltipTargetUI TooltipTarget;
    public Image Icon;
    public Button ImgButton;

    FavorDecision CurrentDecision;
    Character CurrentCharacter;

    public void SetInfo(FavorDecision favorDecision, Character ofCharacter)
    {
        this.CurrentDecision = favorDecision;
        this.CurrentCharacter = ofCharacter;

        TitleText.text = CurrentDecision.name + "\n <size=15>"+CurrentDecision.FavorCost+"</size>";
        Icon.sprite = CurrentDecision.Icon;

        if(favorDecision.LockedToFaction.Find(x=>x.name == ofCharacter.CurrentFaction.name) != null)
        {
            ImgButton.interactable = false;
            TooltipTarget.SetTooltip("<color=red>LOCKED FOR - "+ofCharacter.CurrentFaction.name+" - </color>"+CurrentDecision.Description);
        }
        else if (favorDecision.UnavailableForFactionleader && ofCharacter.TopEmployer == ofCharacter)
        {
            ImgButton.interactable = false;
            TooltipTarget.SetTooltip("<color=red> "+ofCharacter.name+" has no one to betray. - </color>" + CurrentDecision.Description);
        }
        else
        {
            ImgButton.interactable = true;
            TooltipTarget.SetTooltip(CurrentDecision.Description);
        }
    }

    public void BuyDecision()
    {
        int favorPoints = CurrentCharacter.GetFavorPoints(CORE.PC);


        if (CurrentDecision.RequiresTech != null)
        {
            TechTreeItem techInstance = CORE.Instance.TechTree.Find(x => x.name == CurrentDecision.RequiresTech.name);

            if (techInstance != null && !techInstance.IsResearched)
            {
                GlobalMessagePrompterUI.Instance.Show("Requires Tech: "+CurrentDecision.RequiresTech.name, 1f, Color.red);
                return;
            }

        }

        if (favorPoints < CurrentDecision.FavorCost)
        {
            GlobalMessagePrompterUI.Instance.Show("Not Enough Favor Points! (" + favorPoints + "/" + CurrentDecision.FavorCost + ")", 1f, Color.red);
            return;
        }

        if (CurrentDecision.ActionToExecute != null)
        {
            FailReason reason = null;
            if (!CurrentDecision.ActionToExecute.CanDoAction(CORE.PC, CurrentCharacter, CurrentCharacter.CurrentLocation, out reason))
            {
                GlobalMessagePrompterUI.Instance.Show("Can not demand this. " + reason?.Key, 1f, Color.red);

                return;
            }

            CurrentDecision.ActionToExecute.Execute(CORE.PC, CurrentCharacter, CurrentCharacter.CurrentLocation);
        }


        CurrentCharacter.AddFavorPoints(CORE.PC, -CurrentDecision.FavorCost);

        SelectedPanelUI.Instance.LocationPanel.RefreshUI();

        BribeFavorWindowUI.Instance.gameObject.SetActive(false);
    }
}
