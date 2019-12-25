using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityControlUI : MonoBehaviour
{
    public static CityControlUI Instance;

    [SerializeField]
    RectTransform ContainerRect;

    [SerializeField]
    TextMeshProUGUI TextLabel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PassTimeComplete", Refresh);
    }


    private void OnDestroy()
    {
        CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", Refresh);
    }


    public void Refresh()
    {
        ClearContainer();

        float accumPrecent = 0f;
        //faction.FactionHead.PropertiesInCommand.Count 
        foreach (Faction faction in CORE.Instance.Factions)
        {
            if(faction.FactionHead == null)
            {
                continue;
            }

            GameObject tempPanel = ResourcesLoader.Instance.GetRecycledObject("FactionControlElement");
            tempPanel.transform.SetParent(ContainerRect, false);
            tempPanel.transform.localScale = Vector3.one;

            Character factionHead = CORE.Instance.GetCharacter(faction.FactionHead.name);
            float precent = (1f*factionHead.PropertiesInCommand.Count) / (1f*CORE.Instance.Locations.Count);
            accumPrecent += precent;
            tempPanel.GetComponent<FactionCityControlUI>().SetInfo(faction, precent, ContainerRect.rect.width);
        }

        GameObject defaultPanel = ResourcesLoader.Instance.GetRecycledObject("FactionControlElement");
        defaultPanel.transform.SetParent(ContainerRect, false);
        defaultPanel.transform.localScale = Vector3.one;

        float defaultPrecent = 1f-accumPrecent;

        defaultPanel.GetComponent<FactionCityControlUI>().SetInfo(CORE.Instance.Database.DefaultFaction, defaultPrecent, ContainerRect.sizeDelta.x);
    }

    void ClearContainer()
    {
        while(ContainerRect.childCount > 0)
        {
            ContainerRect.GetChild(0).gameObject.SetActive(false);
            ContainerRect.GetChild(0).SetParent(transform);
        }
    }

    public void ShowFactionLabel(Faction faction, float precent)
    {
        TextLabel.gameObject.SetActive(true);

        if (faction.name == CORE.Instance.Database.DefaultFaction.name)
        {
            TextLabel.text = "Unowned Territory - " + Mathf.RoundToInt(precent * 100f) + "%";
            return;
        }

        if (faction.Known.IsKnown("Existance", CORE.PC))
        {
            TextLabel.text = faction.name + " - " + Mathf.RoundToInt(precent * 100f) + "%";
        }
        else
        {
            TextLabel.text = "Unknown Faction - " + Mathf.RoundToInt(precent * 100f) + "%";
        }
    }

    public void HideFactionLabel()
    {
        TextLabel.gameObject.SetActive(false);
    }

    public float GetControlPrecentage(Faction faction)
    {

        if (faction.FactionHead == null)
        {
            return 0f;
        }

        Character factionHead = CORE.Instance.GetCharacter(faction.FactionHead.name);

        return ((float)factionHead.PropertiesInCommand.Count) / ((float)CORE.Instance.Locations.Count);
    }
}
