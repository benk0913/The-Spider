using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraitUI : MonoBehaviour
{
    protected Trait CurrentTrait;

    [SerializeField]
    protected Image Icon;

    [SerializeField]
    protected TooltipTargetUI TooltipTarget;

    public void SetInfo(Trait trait)
    {
        CurrentTrait = trait;

        Icon.sprite = CurrentTrait.icon;

        TooltipTarget.SetTooltip("<size=20>"+CurrentTrait.name+"</size>");

        TooltipTarget.Text += "\n  <size=15>" + CurrentTrait.Description+"</size>";

        for(int i=0;i<CurrentTrait.Bonuses.Count;i++)
        {
            TooltipTarget.Text +=  "\n <size=12>" 
                + (CurrentTrait.Bonuses[i].Value > 0? "<color=green>" : "<color=red>") 
                +  CurrentTrait.Bonuses[i].Type.name 
                + " : " 
                + CurrentTrait.Bonuses[i].Value 
                + "</color></size>";
        }
    }
}
