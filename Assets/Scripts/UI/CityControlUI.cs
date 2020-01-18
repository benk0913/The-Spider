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

    public List<LocationEntity> OwnableLocations
    {
        get
        {
            List<LocationEntity> ownables = CORE.Instance.Locations.FindAll(x =>
            !x.Traits.Contains(CORE.Instance.Database.PublicAreaTrait) && !x.Traits.Contains(CORE.Instance.Database.CentralAreaTrait));

            return ownables;
        }
    }

    public void Refresh()
    {
        ClearContainer();

        float accumPrecent = 0f;
        float ownablesCount = 1f * OwnableLocations.Count;

        foreach (Faction faction in CORE.Instance.Factions)
        {
            if(faction.FactionHead == null)
            {
                continue;
            }

            Character factionHead = CORE.Instance.GetCharacter(faction.FactionHead.name);

            if(factionHead == null)
            {
                continue;
            }

            GameObject tempPanel = ResourcesLoader.Instance.GetRecycledObject("FactionControlElement");
            tempPanel.transform.SetParent(ContainerRect, false);
            tempPanel.transform.localScale = Vector3.one;
            
            float precent = (1f*factionHead.PropertiesInCommand.Count) / ownablesCount;
            accumPrecent += precent;
            tempPanel.GetComponent<FactionCityControlUI>().SetInfo(faction, precent, ContainerRect.rect.width);
        }

        GameObject defaultPanel = ResourcesLoader.Instance.GetRecycledObject("FactionControlElement");
        defaultPanel.transform.SetParent(ContainerRect, false);
        defaultPanel.transform.localScale = Vector3.one;

        float defaultPrecent = 1f-accumPrecent;

        defaultPanel.GetComponent<FactionCityControlUI>().SetInfo(CORE.Instance.Factions.Find(x=>x.name == CORE.Instance.Database.DefaultFaction.name), defaultPrecent, ContainerRect.sizeDelta.x);
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

        return ((float)factionHead.PropertiesInCommand.Count) / ((float)OwnableLocations.Count);
    }
}
