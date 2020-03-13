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
        if (CurrentItem.IsHidden && !CurrentItem.IsResearched)
        {
            this.gameObject.SetActive(false);
            transform.parent.parent.GetComponent<UILineRenderer>().enabled = false;
        }
        else
        {
            this.gameObject.SetActive(true);
            transform.parent.parent.GetComponent<UILineRenderer>().enabled = true;
        }


        TechTitle.text = CurrentItem.name;

        TechIcon.sprite = CurrentItem.Icon;

        if (CurrentItem.Parent == null || CurrentItem.IsResearchable || CurrentItem.IsResearched)
        {
            NotDiscoveredPanel.SetActive(false);

            if (CurrentItem.IsResearched)
            {
                NotResearchedPanel.SetActive(false);

                Liner.color = ResearchedColor;

                Cost.color = Color.green;
                Cost.text = "DONE";
            }
            else
            {
                NotResearchedPanel.SetActive(true);

                Liner.color = UnresearchedColor;

                Cost.color = CORE.PC.Progress >= CurrentItem.Price ? Color.white : Color.red;
                Cost.text = CurrentItem.Price.ToString();
            }
        }
        else
        {
            NotDiscoveredPanel.SetActive(true);
            Liner.color = Color.grey;

            Cost.color = Color.white;
            Cost.text = "LOCKED";
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

        if (CurrentItem.IsResearched)
        {
            return;
        }

        if (CurrentItem.Price > CORE.PC.Progress)
        {
            return;
        }

        AudioControl.Instance.Play("tech_research");

        CORE.PC.Progress -= CurrentItem.Price;
        CurrentItem.IsResearched = true;
        TechNodeTreeUI.Instance.RefreshNodes();
        CORE.Instance.InvokeEvent("ResearchComplete");
    }
}
