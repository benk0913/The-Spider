using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechTreeItemUI : MonoBehaviour
{
    public TechTreeItem CurrentItem;

    [SerializeField]
    TextMeshProUGUI TechTitle;

    [SerializeField]
    Image TechIcon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    GameObject Lock;

    [SerializeField]
    GameObject NotResearchedPanel;

    public void SetItem(TechTreeItem item)
    {
        CurrentItem = item;

        RefreshUI();
    }

    void RefreshUI()
    {
        TechTitle.name = CurrentItem.name;
        TechIcon.sprite = CurrentItem.Icon;

        NotResearchedPanel.SetActive(!CurrentItem.IsResearched);

        List<TooltipBonus> tooltipBonuses = new List<TooltipBonus>();

        tooltipBonuses.Add(new TooltipBonus("Cost: " + CurrentItem.Price, ResourcesLoader.Instance.GetSprite("earIcon")));

        TooltipTarget.SetTooltip(CurrentItem.name + "\n" + CurrentItem.Description, tooltipBonuses);
    }

    public void OnClick()
    {
        
    }
}
