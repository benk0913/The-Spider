using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBookPropertiesViewUI : MonoBehaviour
{
    [SerializeField]
    Transform PropertiesContainer;

    private void Start()
    {
        PropertyViewPanelUI viewItem;

        foreach(Property property in CORE.Instance.Database.Properties)
        {
            viewItem = ResourcesLoader.Instance.GetRecycledObject("PropertyViewPanelUI").GetComponent<PropertyViewPanelUI>();

            viewItem.transform.SetParent(PropertiesContainer, false);
            viewItem.SetProperty(property);
        }
    }
}
