using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RebrandWindowUI : MonoBehaviour
{
    public static RebrandWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TextMeshProUGUI Description;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    Transform PossibleTierGrid;

    [SerializeField]
    Transform ActionsGrid;

    [SerializeField]
    PlotTypeUI PlotType;

    [SerializeField]
    GameObject CurrentPropertyPointer;

    [SerializeField]
    GameObject NoPropertiesPanel;

    LocationEntity CurrentLocation;

    public int CurrentIndex = 0;

    private void Awake()
    {
        Instance = this;

        Hide();
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public void Show(LocationEntity currentLocation)
    {
        MouseLook.Instance.CurrentWindow = this.gameObject;

        CurrentIndex = 0;
        CurrentLocation = currentLocation;

        this.gameObject.SetActive(true);

        int propertiesAvailable = 0;
        for(int i=0;i< CORE.PlayerFaction.FactionProperties.Length;i++)
        {
            Property property = CORE.PlayerFaction.FactionProperties[i];
            if (property.PlotType != CurrentLocation.CurrentProperty.PlotType)
            {
                continue;
            }

            if (property.TechRequired != null && !property.TechRequired.IsResearched)
            {
                continue;
            }

            propertiesAvailable++;
        }

        NoPropertiesPanel.gameObject.SetActive(propertiesAvailable < 2);


        SetProperty(CurrentLocation.CurrentProperty);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Next()
    {
        CurrentIndex++;

        if(CurrentIndex >= CORE.PlayerFaction.FactionProperties.Length)
        {
            CurrentIndex = 0;
        }

        Property property = CORE.PlayerFaction.FactionProperties[CurrentIndex];

        if (property.PlotType != CurrentLocation.CurrentProperty.PlotType)
        {
            Next();
            return;
        }

        if (property.TechRequired != null)
        {
            TechTreeItem item = CORE.Instance.TechTree.Find(x => x.name == property.TechRequired.name);

            if(item != null && !item.IsResearched)
            {
                Next();
                return;
            }
        }

        SetProperty(CORE.PlayerFaction.FactionProperties[CurrentIndex]);
    }

    public void Previous()
    {
        CurrentIndex--;

        if (CurrentIndex < 0)
        {
            CurrentIndex = CORE.PlayerFaction.FactionProperties.Length - 1;
        }

        Property property = CORE.PlayerFaction.FactionProperties[CurrentIndex];

        if (property.PlotType != CurrentLocation.CurrentProperty.PlotType)
        {
            Previous();
            return;
        }

        if (property.TechRequired != null)
        {
            TechTreeItem item = CORE.Instance.TechTree.Find(x => x.name == property.TechRequired.name);

            if (item != null && !item.IsResearched)
            {
                Previous();
                return;
            }
        }


        SetProperty(CORE.PlayerFaction.FactionProperties[CurrentIndex]);
    }

    public void SetProperty(Property property)
    {
        CurrentPropertyPointer.gameObject.SetActive(property == CurrentLocation.CurrentProperty);

        Title.text = property.name;
        Description.text = property.Description;
        IconImage.sprite = property.Icon;

        PlotType.SetInfo(property.PlotType);

        ClearContainers();
        for(int i=0;i<property.PropertyLevels.Count;i++)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("PropertyLevelTierUI");
            tempObj.transform.SetParent(PossibleTierGrid, false);
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<PropertyLevelTierUI>().SetInfo(property, property.PropertyLevels[i]);
        }

        foreach(PlayerAction action in property.UniquePlayerActions)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("RightClickMenuItem");
            tempObj.transform.SetParent(ActionsGrid, false);
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<RightClickMenuItemUI>().SetInfo(action.name, () => { }, action.Description, action.Icon, true);
        }
    }

    void ClearContainers()
    {
        while(PossibleTierGrid.childCount > 0)
        {
            PossibleTierGrid.GetChild(0).gameObject.SetActive(false);
            PossibleTierGrid.GetChild(0).transform.SetParent(transform);
        }

        while (ActionsGrid.childCount > 0)
        {
            ActionsGrid.GetChild(0).gameObject.SetActive(false);
            ActionsGrid.GetChild(0).transform.SetParent(transform);
        }
    }

    public void ConfirmProperty()
    {
        CurrentLocation.Rebrand(CORE.PC, CORE.PlayerFaction.FactionProperties[CurrentIndex]);
        Hide();
    }
}
