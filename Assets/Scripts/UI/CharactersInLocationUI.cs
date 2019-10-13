using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersInLocationUI : MonoBehaviour
{
    [SerializeField]
    Transform Container;

    [SerializeField]
    WorldPositionLerperUI Lerper;


    private void Start()
    {
        CORE.Instance.SubscribeToEvent("ShowMap", Show);

        CORE.Instance.SubscribeToEvent("HideMap", Hide);
        Hide();
    }

    private void OnDestroy()
    {
        CORE.Instance.UnsubscribeFromEvent("ShowMap", Show);
        CORE.Instance.UnsubscribeFromEvent("HideMap", Hide);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    LocationEntity currentLocation;
    public void SetInfo(LocationEntity location)
    {
        currentLocation = location;

        Lerper.SetTransform(location.transform);

        Refresh();
    }

    void Refresh()
    {
        ClearContainer();

        foreach (Character character in currentLocation.CharactersInLocation)
        {
            if (!character.isImportant)
            {
                continue;
            }

            if(!character.IsKnown("CurrentLocation"))
            {
                continue;
            }

            PortraitUI portrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
            portrait.transform.SetParent(Container, false);
            portrait.transform.localScale = Vector3.one;
            portrait.SetCharacter(character);
        }
    }

    void ClearContainer()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }
    }
}
