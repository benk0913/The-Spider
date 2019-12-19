using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TechTreeItemUI : MonoBehaviour
{
    public TechTreeItem CurrentItem;

    [SerializeField]
    TextMeshProUGUI TechTitle;

    [SerializeField]
    TextMeshProUGUI Cost;

    [SerializeField]
    Image TechIcon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    GameObject Lock;

    [SerializeField]
    GameObject NotResearchedPanel;

    [SerializeField]
    GameObject NotDiscoveredPanel;

    [SerializeField]
    UILineRenderer Liner;

    [SerializeField]
    Color UnresearchedColor;

    [SerializeField]
    Color ResearchedColor;

    public void SetItem(TechTreeItem item)
    {
        CurrentItem = item;

        RefreshUI();
    }

    public void RefreshUI()
    {
        TechTitle.text = CurrentItem.name;

        if (CurrentItem.FactionsLocked.Contains(CORE.PC.CurrentFaction))
        {
            Cost.color = Color.white;
            Cost.text = "LOCKED";
        }
        else
        {
            Cost.color = CORE.PC.Progress >= CurrentItem.Price ? Color.white : Color.red;
            Cost.text = CurrentItem.Price.ToString();
        }

        TechIcon.sprite = CurrentItem.Icon;

        if (CurrentItem.Parent == null || CurrentItem.IsResearchable)
        {
            NotDiscoveredPanel.SetActive(false);

            if (CurrentItem.IsResearched)
            {
                NotResearchedPanel.SetActive(false);

                Liner.color = ResearchedColor;
            }
            else
            {
                NotResearchedPanel.SetActive(true);

                Liner.color = UnresearchedColor;
            }
        }
        else
        {
            NotDiscoveredPanel.SetActive(true);
            Liner.color = Color.black;
        }

        List<TooltipBonus> tooltipBonuses = new List<TooltipBonus>();

        tooltipBonuses.Add(new TooltipBonus("Cost: " + CurrentItem.Price, ResourcesLoader.Instance.GetSprite("scroll-unfurled")));

        TooltipTarget.SetTooltip("<u>"+CurrentItem.name + "</u>\n" + CurrentItem.Description, tooltipBonuses);
    }

    public void OnClick()
    {
        if (!CurrentItem.IsResearchable)
        {
            return;
        }

        if(CurrentItem.Price > CORE.PC.Progress)
        {
            return;
        }

        CORE.PC.Progress -= CurrentItem.Price;
        CurrentItem.IsResearched = true;
        TechNodeTreeUI.Instance.RefreshNodes();
    }
}
