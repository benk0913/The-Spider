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
    Transform PossibleActionsContainer;

    [SerializeField]
    PlotTypeUI PlotType;

    [SerializeField]
    GameObject CurrentPropertyPointer;

    LocationEntity CurrentLocation;

    public int CurrentIndex = 0;

    private void Awake()
    {
        Instance = this;

        Hide();
    }

    public void Show(LocationEntity currentLocation)
    {
        CurrentLocation = currentLocation;

        this.gameObject.SetActive(true);

        SetProperty(CurrentLocation.CurrentProperty);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Next()
    {
        CurrentIndex++;

        if(CurrentIndex >= CORE.Instance.Database.Properties.Count)
        {
            CurrentIndex = 0;
        }

        if(CORE.Instance.Database.Properties[CurrentIndex].PlotType != CurrentLocation.CurrentProperty.PlotType)
        {
            Next();
            return;
        }

        SetProperty(CORE.Instance.Database.Properties[CurrentIndex]);
    }

    public void Previous()
    {
        CurrentIndex--;

        if (CurrentIndex <= 0)
        {
            CurrentIndex = CORE.Instance.Database.Properties.Count - 1;
        }

        if (CORE.Instance.Database.Properties[CurrentIndex].PlotType != CurrentLocation.CurrentProperty.PlotType)
        {
            Previous();
            return;
        }

        SetProperty(CORE.Instance.Database.Properties[CurrentIndex]);
    }

    public void SetProperty(Property property)
    {
        CurrentPropertyPointer.gameObject.SetActive(property == CurrentLocation.CurrentProperty);

        Title.text = property.name;
        Description.text = property.Description;
        IconImage.sprite = property.Icon;

        ClearPossibleActionsContainer();
        for(int i=0;i<property.Actions.Count;i++)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(DEF.ACTION_PREFAB);
            tempObj.transform.SetParent(PossibleActionsContainer, false);
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<ActionUI>().SetInfo(null, property.Actions[i]);
        }

        PlotType.SetInfo(property.PlotType);
    }

    void ClearPossibleActionsContainer()
    {
        while(PossibleActionsContainer.childCount > 0)
        {
            PossibleActionsContainer.GetChild(0).gameObject.SetActive(false);
            PossibleActionsContainer.GetChild(0).transform.SetParent(transform);
        }
    }

    public void ConfirmProperty()
    {
        CurrentLocation.Rebrand(CORE.Instance.Database.Properties[CurrentIndex]);
        Hide();
    }
}
